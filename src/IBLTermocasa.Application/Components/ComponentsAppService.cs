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
using IBLTermocasa.Components;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using IBLTermocasa.Shared;

namespace IBLTermocasa.Components
{
    [RemoteService(IsEnabled = false)]
    [Authorize(IBLTermocasaPermissions.Components.Default)]
    public abstract class ComponentsAppServiceBase : IBLTermocasaAppService
    {
        protected IDistributedCache<ComponentExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        protected IComponentRepository _componentRepository;
        protected ComponentManager _componentManager;

        public ComponentsAppServiceBase(IComponentRepository componentRepository, ComponentManager componentManager, IDistributedCache<ComponentExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _componentRepository = componentRepository;
            _componentManager = componentManager;
        }

        public virtual async Task<PagedResultDto<ComponentDto>> GetListAsync(GetComponentsInput input)
        {
            var totalCount = await _componentRepository.GetCountAsync(input.FilterText, input.Name);
            var items = await _componentRepository.GetListAsync(input.FilterText, input.Name, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<ComponentDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Component>, List<ComponentDto>>(items)
            };
        }

        public virtual async Task<ComponentDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Component, ComponentDto>(await _componentRepository.GetAsync(id));
        }

        [Authorize(IBLTermocasaPermissions.Components.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _componentRepository.DeleteAsync(id);
        }

        [Authorize(IBLTermocasaPermissions.Components.Create)]
        public virtual async Task<ComponentDto> CreateAsync(ComponentCreateDto input)
        {

            var component = await _componentManager.CreateAsync(
            input.Name
            );

            return ObjectMapper.Map<Component, ComponentDto>(component);
        }

        [Authorize(IBLTermocasaPermissions.Components.Edit)]
        public virtual async Task<ComponentDto> UpdateAsync(Guid id, ComponentUpdateDto input)
        {

            var component = await _componentManager.UpdateAsync(
            id,
            input.Name, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Component, ComponentDto>(component);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(ComponentExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _componentRepository.GetListAsync(input.FilterText, input.Name);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<Component>, List<ComponentExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Components.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public virtual async Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new ComponentExcelDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new IBLTermocasa.Shared.DownloadTokenResultDto
            {
                Token = token
            };
        }
        [Authorize(IBLTermocasaPermissions.Components.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> componentIds)
        {
            await _componentRepository.DeleteManyAsync(componentIds);
        }

        [Authorize(IBLTermocasaPermissions.Components.Delete)]
        public virtual async Task DeleteAllAsync(GetComponentsInput input)
        {
            await _componentRepository.DeleteAllAsync(input.FilterText, input.Name);
        }
    }
}