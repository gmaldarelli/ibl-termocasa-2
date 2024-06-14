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
using IBLTermocasa.BillOfMaterials;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using IBLTermocasa.Shared;

namespace IBLTermocasa.BillOfMaterials
{
    [RemoteService(IsEnabled = false)]
    [Authorize(IBLTermocasaPermissions.BillOFMaterials.Default)]
    public class BillOFMaterialsAppService : IBLTermocasaAppService, IBillOfMaterialsAppService
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

        public virtual async Task<PagedResultDto<BillOfMaterialDto>> GetListAsync(GetBillOfMaterialsInput input)
        {
            var totalCount = await _billOFMaterialRepository.GetCountAsync(input.FilterText, input.Name, input.RequestForQuotationProperty);
            var items = await _billOFMaterialRepository.GetListAsync(input.FilterText, input.Name, input.RequestForQuotationProperty, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<BillOfMaterialDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<BillOFMaterial>, List<BillOfMaterialDto>>(items)
            };
        }

        public virtual async Task<BillOfMaterialDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<BillOFMaterial, BillOfMaterialDto>(await _billOFMaterialRepository.GetAsync(id));
        }

        [Authorize(IBLTermocasaPermissions.BillOFMaterials.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _billOFMaterialRepository.DeleteAsync(id);
        }

        [Authorize(IBLTermocasaPermissions.BillOFMaterials.Create)]
        public virtual async Task<BillOfMaterialDto> CreateAsync(BillOfMaterialCreateDto input)
        {
            var billOFMaterial = ObjectMapper.Map<BillOfMaterialCreateDto, BillOFMaterial>(input);
            return ObjectMapper.Map<BillOFMaterial, BillOfMaterialDto>(await _billOFMaterialManager.CreateAsync(billOFMaterial));
        }

        [Authorize(IBLTermocasaPermissions.BillOFMaterials.Edit)]
        public virtual async Task<BillOfMaterialDto> UpdateAsync(Guid id, BillOfMaterialUpdateDto input)
        {
            var billOFMaterial = ObjectMapper.Map<BillOfMaterialUpdateDto, BillOFMaterial>(input);
            return ObjectMapper.Map<BillOFMaterial, BillOfMaterialDto>(await _billOFMaterialManager.UpdateAsync(id, billOFMaterial));
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(BillOfMaterialExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _billOFMaterialRepository.GetListAsync(input.FilterText, input.Name, input.RequestForQuotationId);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<BillOFMaterial>, List<BillOfMaterialExcelDto>>(items));
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
        public virtual async Task DeleteAllAsync(GetBillOfMaterialsInput input)
        {
            await _billOFMaterialRepository.DeleteAllAsync(input.FilterText, input.Name, input.RequestForQuotationProperty);
        }
    }
}