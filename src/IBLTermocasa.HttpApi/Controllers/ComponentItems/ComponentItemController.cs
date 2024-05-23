using IBLTermocasa.Shared;
using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using IBLTermocasa.ComponentItems;

namespace IBLTermocasa.Controllers.ComponentItems
{
    [RemoteService]
    [Area("app")]
    [ControllerName("ComponentItem")]
    [Route("api/app/component-items")]

    public abstract class ComponentItemControllerBase : AbpController
    {
        protected IComponentItemsAppService _componentItemsAppService;

        public ComponentItemControllerBase(IComponentItemsAppService componentItemsAppService)
        {
            _componentItemsAppService = componentItemsAppService;
        }

        [HttpGet]
        [Route("by-component")]
        public virtual Task<PagedResultDto<ComponentItemDto>> GetListByComponentIdAsync(GetComponentItemListInput input)
        {
            return _componentItemsAppService.GetListByComponentIdAsync(input);
        }
        [HttpGet]
        [Route("detailed/by-component")]
        public virtual Task<PagedResultDto<ComponentItemWithNavigationPropertiesDto>> GetListWithNavigationPropertiesByComponentIdAsync(GetComponentItemListInput input)
        {
            return _componentItemsAppService.GetListWithNavigationPropertiesByComponentIdAsync(input);
        }

        [HttpGet]
        public virtual Task<PagedResultDto<ComponentItemWithNavigationPropertiesDto>> GetListAsync(GetComponentItemsInput input)
        {
            return _componentItemsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public virtual Task<ComponentItemWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _componentItemsAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<ComponentItemDto> GetAsync(Guid id)
        {
            return _componentItemsAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("material-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetMaterialLookupAsync(LookupRequestDto input)
        {
            return _componentItemsAppService.GetMaterialLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<ComponentItemDto> CreateAsync(ComponentItemCreateDto input)
        {
            return _componentItemsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<ComponentItemDto> UpdateAsync(Guid id, ComponentItemUpdateDto input)
        {
            return _componentItemsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _componentItemsAppService.DeleteAsync(id);
        }
    }
}