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
using IBLTermocasa.Materials;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using IBLTermocasa.Shared;

namespace IBLTermocasa.Materials
{
    [RemoteService(IsEnabled = false)]
    [Authorize(IBLTermocasaPermissions.Materials.Default)]
    public abstract class MaterialsAppServiceBase : IBLTermocasaAppService
    {
        protected IDistributedCache<MaterialExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        protected IMaterialRepository _materialRepository;
        protected MaterialManager _materialManager;

        public MaterialsAppServiceBase(IMaterialRepository materialRepository, MaterialManager materialManager, IDistributedCache<MaterialExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _materialRepository = materialRepository;
            _materialManager = materialManager;
        }

        public virtual async Task<PagedResultDto<MaterialDto>> GetListAsync(GetMaterialsInput input)
        {
            var totalCount = await _materialRepository.GetCountAsync(input.FilterText, input.Code, input.Name);
            var items = await _materialRepository.GetListAsync(input.FilterText, input.Code, input.Name, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<MaterialDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Material>, List<MaterialDto>>(items)
            };
        }

        public virtual async Task<MaterialDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Material, MaterialDto>(await _materialRepository.GetAsync(id));
        }

        [Authorize(IBLTermocasaPermissions.Materials.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _materialRepository.DeleteAsync(id);
        }

        [Authorize(IBLTermocasaPermissions.Materials.Create)]
        public virtual async Task<MaterialDto> CreateAsync(MaterialCreateDto input)
        {

            var material = await _materialManager.CreateAsync(
            input.Code, input.Name, input.MeasureUnit, input.Quantity, input.Lifo, input.StandardPrice, input.AveragePrice, input.LastPrice, input.AveragePriceSecond
            );

            return ObjectMapper.Map<Material, MaterialDto>(material);
        }

        [Authorize(IBLTermocasaPermissions.Materials.Edit)]
        public virtual async Task<MaterialDto> UpdateAsync(Guid id, MaterialUpdateDto input)
        {

            var material = await _materialManager.UpdateAsync(
            id,
            input.Code, input.Name, input.MeasureUnit, input.Quantity, input.Lifo, input.StandardPrice, input.AveragePrice, input.LastPrice, input.AveragePriceSecond, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Material, MaterialDto>(material);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(MaterialExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _materialRepository.GetListAsync(input.FilterText, input.Code, input.Name);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<Material>, List<MaterialExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Materials.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public virtual async Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new MaterialExcelDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new IBLTermocasa.Shared.DownloadTokenResultDto
            {
                Token = token
            };
        }
        [Authorize(IBLTermocasaPermissions.Materials.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> materialIds)
        {
            await _materialRepository.DeleteManyAsync(materialIds);
        }

        [Authorize(IBLTermocasaPermissions.Materials.Delete)]
        public virtual async Task DeleteAllAsync(GetMaterialsInput input)
        {
            await _materialRepository.DeleteAllAsync(input.FilterText, input.Code, input.Name);
        }
    }
}