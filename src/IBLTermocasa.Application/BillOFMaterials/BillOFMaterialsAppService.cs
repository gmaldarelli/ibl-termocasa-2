using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using IBLTermocasa.Permissions;
using IBLTermocasa.BillOFMaterials;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using IBLTermocasa.Shared;

namespace IBLTermocasa.BillOFMaterials
{
    [RemoteService(IsEnabled = false)]
    [Authorize(IBLTermocasaPermissions.BillOFMaterials.Default)]
    public class BillOFMaterialsAppService : IBLTermocasaAppService, IBillOFMaterialsAppService
    {
        protected IDistributedCache<BillOFMaterialExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        protected IBillOFMaterialRepository _billOFMaterialRepository;
        protected BillOFMaterialManager _billOFMaterialManager;

        public BillOFMaterialsAppService(IBillOFMaterialRepository billOFMaterialRepository, BillOFMaterialManager billOFMaterialManager, IDistributedCache<BillOFMaterialExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _billOFMaterialRepository = billOFMaterialRepository;
            _billOFMaterialManager = billOFMaterialManager;
        }

        public virtual async Task<PagedResultDto<BillOFMaterialDto>> GetListAsync(GetBillOFMaterialsInput input)
        {
            var totalCount = await _billOFMaterialRepository.GetCountAsync(input.FilterText, input.Name, input.RequestForQuotationProperty);
            var items = await _billOFMaterialRepository.GetListAsync(input.FilterText, input.Name, input.RequestForQuotationProperty, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<BillOFMaterialDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<BillOFMaterial>, List<BillOFMaterialDto>>(items)
            };
        }

        public virtual async Task<BillOFMaterialDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<BillOFMaterial, BillOFMaterialDto>(await _billOFMaterialRepository.GetAsync(id));
        }

        [Authorize(IBLTermocasaPermissions.BillOFMaterials.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _billOFMaterialRepository.DeleteAsync(id);
        }

        [Authorize(IBLTermocasaPermissions.BillOFMaterials.Create)]
        public virtual async Task<BillOFMaterialDto> CreateAsync(BillOFMaterialCreateDto input)
        {
            var billOFMaterial = ObjectMapper.Map<BillOFMaterialCreateDto, BillOFMaterial>(input);
            return ObjectMapper.Map<BillOFMaterial, BillOFMaterialDto>(await _billOFMaterialManager.CreateAsync(billOFMaterial));
        }

        [Authorize(IBLTermocasaPermissions.BillOFMaterials.Edit)]
        public virtual async Task<BillOFMaterialDto> UpdateAsync(Guid id, BillOFMaterialUpdateDto input)
        {
            var billOFMaterial = ObjectMapper.Map<BillOFMaterialUpdateDto, BillOFMaterial>(input);
            return ObjectMapper.Map<BillOFMaterial, BillOFMaterialDto>(await _billOFMaterialManager.UpdateAsync(id, billOFMaterial));
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(BillOFMaterialExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _billOFMaterialRepository.GetListAsync(input.FilterText, input.Name, input.RequestForQuotationId);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<BillOFMaterial>, List<BillOFMaterialExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "BillOFMaterials.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public virtual async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new BillOFMaterialExcelDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new IBLTermocasa.Shared.DownloadTokenResultDto
            {
                Token = token
            };
        }
        [Authorize(IBLTermocasaPermissions.BillOFMaterials.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> billofmaterialIds)
        {
            await _billOFMaterialRepository.DeleteManyAsync(billofmaterialIds);
        }

        [Authorize(IBLTermocasaPermissions.BillOFMaterials.Delete)]
        public virtual async Task DeleteAllAsync(GetBillOFMaterialsInput input)
        {
            await _billOFMaterialRepository.DeleteAllAsync(input.FilterText, input.Name, input.RequestForQuotationProperty);
        }
    }
}