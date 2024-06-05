using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using IBLTermocasa.Materials;
using IBLTermocasa.Permissions;
using IBLTermocasa.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using MiniExcelLibs;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Content;

namespace IBLTermocasa.Components
{
    [RemoteService(IsEnabled = false)]
    [Authorize(IBLTermocasaPermissions.Components.Default)]
    public class ComponentsAppService : IBLTermocasaAppService, IComponentsAppService
    {
        protected IDistributedCache<ComponentExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        protected IComponentRepository _componentRepository;
        protected ComponentManager _componentManager;
        protected IMaterialRepository _materialRepository;

        public ComponentsAppService(IComponentRepository componentRepository, ComponentManager componentManager, IDistributedCache<ComponentExcelDownloadTokenCacheItem, string> excelDownloadTokenCache, IMaterialRepository materialRepository)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _componentRepository = componentRepository;
            _componentManager = componentManager;
            _materialRepository = materialRepository;
        }

        public virtual async Task<PagedResultDto<ComponentDto>> GetListAsync(GetComponentsInput input)
        {
            var totalCount = await _componentRepository.GetCountAsync(input.FilterText, input.Name);
            var items = await _componentRepository.GetListAsync(input.FilterText, input.Name, input.Sorting, input.MaxResultCount, input.SkipCount);

            List<ComponentDto> componentDtos = new List<ComponentDto>();
            items.ForEach(x =>
            {
                var dto = ObjectMapper.Map<Component, ComponentDto>(x);
                dto.ComponentItems = ObjectMapper.Map<List<ComponentItem>, List<ComponentItemDto>>(x.ComponentItems);
                componentDtos.Add(dto);
            });
            var temp = ObjectMapper.Map<List<Component>, List<ComponentDto>>(items);
            List<Guid> materialIds = componentDtos.SelectMany(x => x.ComponentItems.Select(y => y.MaterialId)).ToList();
            var materials = await _materialRepository.GetListAsync(x => materialIds.Contains(x.Id));
            componentDtos.ForEach(x => x.ComponentItems.ForEach(y =>
            {
                y.MaterialCode = materials.FirstOrDefault(z => z.Id == y.MaterialId)?.Code;
                y.MaterialName = materials.FirstOrDefault(z => z.Id == y.MaterialId)?.Name;
            }));
            
            return new PagedResultDto<ComponentDto>
            {
                TotalCount = totalCount,
                Items = componentDtos
            };
        }

        public virtual async Task<ComponentDto> GetAsync(Guid id)
        {
            var entity =await _componentRepository.GetAsync(id);
            var  componentDto =  ObjectMapper.Map<Component, ComponentDto>(entity);
            componentDto.ComponentItems = ObjectMapper.Map<List<ComponentItem>, List<ComponentItemDto>>(entity.ComponentItems);
            this.FillMaterialNameAndCode(componentDto);
            return componentDto;
        }

        [Authorize(IBLTermocasaPermissions.Components.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _componentRepository.DeleteAsync(id);
        }

        [Authorize(IBLTermocasaPermissions.Components.Create)]
        public virtual async Task<ComponentDto> CreateAsync(ComponentCreateDto input)
        {
            var entityInput = ObjectMapper.Map<ComponentCreateDto, Component>(input);
            var component = await _componentManager.CreateAsync(
                entityInput
            );

            return ObjectMapper.Map<Component, ComponentDto>(component);
        }

        [Authorize(IBLTermocasaPermissions.Components.Edit)]
        public virtual async Task<ComponentDto> UpdateAsync(Guid id, ComponentUpdateDto input)
        {
            var entityInput = ObjectMapper.Map<ComponentUpdateDto, Component>(input);
            var entity = await _componentManager.UpdateAsync(
                entityInput
            );
            var  dto =  ObjectMapper.Map<Component, ComponentDto>(entity);
            dto.ComponentItems = ObjectMapper.Map<List<ComponentItem>, List<ComponentItemDto>>(entity.ComponentItems);
            return dto;
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

            return new RemoteStreamContent(memoryStream, "ProductComponents.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public virtual async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new ComponentExcelDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new DownloadTokenResultDto
            {
                Token = token
            };
        }

        public virtual async Task<ComponentDto> DeleteComponentItemAsync(Guid componentId, Guid componentItemId)
        {
            var component = await _componentRepository.GetAsync(componentId);
            if(component == null)
            {
                throw new UserFriendlyException(L["NoComponentFound"]);
            }
            component.ComponentItems = component.ComponentItems.Where(x => x.Id != componentItemId).ToList();
            var entityResult = await _componentRepository.UpdateAsync(component);
            var  dto =  ObjectMapper.Map<Component, ComponentDto>(entityResult);
            dto.ComponentItems = ObjectMapper.Map<List<ComponentItem>, List<ComponentItemDto>>(entityResult.ComponentItems);
            this.FillMaterialNameAndCode(dto);
            return dto;
        }

        public virtual async Task<ComponentDto> UpdateComponentItemAsync(Guid componentId, List<ComponentItemDto> componentItems)
        {
            var entity = await _componentRepository.GetAsync(componentId);
            if(entity == null)
            {
                throw new UserFriendlyException(L["NoComponentFound"]);
            }
            
            componentItems.ForEach(componentItem =>
            {
                if(entity.ComponentItems.All(x => x.Id != componentItem.Id))
                {
                    throw new UserFriendlyException(L["NoComponentItemFound"]);
                }
                entity.ComponentItems.ForEach(x =>
                {
                    if (x.Id == componentItem.Id)
                    {
                        x.IsDefault = componentItem.IsDefault;
                        x.MaterialId = componentItem.MaterialId;
                    }
                });
            });
            var entityResult = await _componentRepository.UpdateAsync(entity);
            var  dto =  ObjectMapper.Map<Component, ComponentDto>(entityResult);
            dto.ComponentItems = ObjectMapper.Map<List<ComponentItem>, List<ComponentItemDto>>(entityResult.ComponentItems);
            this.FillMaterialNameAndCode(dto);
            return dto;
        }

        public virtual async Task<ComponentDto> CreateComponentItemAsync(Guid componentId, List<ComponentItemDto> componentItems)
        {
            var component = await _componentRepository.GetAsync(componentId);
            if(component == null)
            {
                throw new UserFriendlyException(L["NoComponentFound"]);
            }
            componentItems.ForEach(componentItem =>
            {
                component.ComponentItems.Add(ObjectMapper.Map<ComponentItemDto, ComponentItem>(componentItem));
            });
            var entityResult = await _componentRepository.UpdateAsync(component);
            var dto = ObjectMapper.Map<Component, ComponentDto>(entityResult);
            dto.ComponentItems = ObjectMapper.Map<List<ComponentItem>, List<ComponentItemDto>>(entityResult.ComponentItems);
            this.FillMaterialNameAndCode(dto);
            return dto;
        }

        private void FillMaterialNameAndCode(ComponentDto component)
        {
            List<Guid> materialIds = component.ComponentItems.Select(x => x.MaterialId).ToList();
            var materials = _materialRepository.GetListAsync(x => materialIds.Contains(x.Id)).Result;
            component.ComponentItems.ForEach(
                y =>
                {
                    y.MaterialCode = materials.FirstOrDefault(z => z.Id == y.MaterialId)?.Code;
                    y.MaterialName = materials.FirstOrDefault(z => z.Id == y.MaterialId)?.Name;
                });
        }
        
        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetMaterialLookupAsync(LookupRequestDto input)
        {
            var query = (await _materialRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null &&
                         x.Name.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Material>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Material>, List<LookupDto<Guid>>>(lookupData)
            };
        }
    }
}