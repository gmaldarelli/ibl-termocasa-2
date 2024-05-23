using IBLTermocasa.Shared;
using IBLTermocasa.Materials;
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
using IBLTermocasa.ComponentItems;

namespace IBLTermocasa.ComponentItems
{
    [RemoteService(IsEnabled = false)]
    [Authorize(IBLTermocasaPermissions.ComponentItems.Default)]
    public abstract class ComponentItemsAppServiceBase : IBLTermocasaAppService
    {

        protected IComponentItemRepository _componentItemRepository;
        protected ComponentItemManager _componentItemManager;
        protected IRepository<Material, Guid> _materialRepository;

        public ComponentItemsAppServiceBase(IComponentItemRepository componentItemRepository, ComponentItemManager componentItemManager, IRepository<Material, Guid> materialRepository)
        {

            _componentItemRepository = componentItemRepository;
            _componentItemManager = componentItemManager; _materialRepository = materialRepository;
        }

        public virtual async Task<PagedResultDto<ComponentItemDto>> GetListByComponentIdAsync(GetComponentItemListInput input)
        {
            var componentItems = await _componentItemRepository.GetListByComponentIdAsync(
                input.ComponentId,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount);

            return new PagedResultDto<ComponentItemDto>
            {
                TotalCount = await _componentItemRepository.GetCountByComponentIdAsync(input.ComponentId),
                Items = ObjectMapper.Map<List<ComponentItem>, List<ComponentItemDto>>(componentItems)
            };
        }
        public virtual async Task<PagedResultDto<ComponentItemWithNavigationPropertiesDto>> GetListWithNavigationPropertiesByComponentIdAsync(GetComponentItemListInput input)
        {
            var componentItems = await _componentItemRepository.GetListWithNavigationPropertiesByComponentIdAsync(
                input.ComponentId,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount);

            return new PagedResultDto<ComponentItemWithNavigationPropertiesDto>
            {
                TotalCount = await _componentItemRepository.GetCountByComponentIdAsync(input.ComponentId),
                Items = ObjectMapper.Map<List<ComponentItemWithNavigationProperties>, List<ComponentItemWithNavigationPropertiesDto>>(componentItems)
            };
        }

        public virtual async Task<PagedResultDto<ComponentItemWithNavigationPropertiesDto>> GetListAsync(GetComponentItemsInput input)
        {
            var totalCount = await _componentItemRepository.GetCountAsync(input.FilterText, input.IsDefault, input.MaterialId);
            var items = await _componentItemRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.IsDefault, input.MaterialId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<ComponentItemWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<ComponentItemWithNavigationProperties>, List<ComponentItemWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<ComponentItemWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<ComponentItemWithNavigationProperties, ComponentItemWithNavigationPropertiesDto>
                (await _componentItemRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<ComponentItemDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<ComponentItem, ComponentItemDto>(await _componentItemRepository.GetAsync(id));
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

        [Authorize(IBLTermocasaPermissions.ComponentItems.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _componentItemRepository.DeleteAsync(id);
        }

        [Authorize(IBLTermocasaPermissions.ComponentItems.Create)]
        public virtual async Task<ComponentItemDto> CreateAsync(ComponentItemCreateDto input)
        {
            if (input.MaterialId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Material"]]);
            }

            var componentItem = await _componentItemManager.CreateAsync(input.ComponentId,
            input.MaterialId, input.IsDefault
            );

            return ObjectMapper.Map<ComponentItem, ComponentItemDto>(componentItem);
        }

        [Authorize(IBLTermocasaPermissions.ComponentItems.Edit)]
        public virtual async Task<ComponentItemDto> UpdateAsync(Guid id, ComponentItemUpdateDto input)
        {
            if (input.MaterialId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Material"]]);
            }

            var componentItem = await _componentItemManager.UpdateAsync(
            id, input.ComponentId,
            input.MaterialId, input.IsDefault
            );

            return ObjectMapper.Map<ComponentItem, ComponentItemDto>(componentItem);
        }
    }
}