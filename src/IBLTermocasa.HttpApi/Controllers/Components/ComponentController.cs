using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using IBLTermocasa.Components;
using IBLTermocasa.Materials;
using Volo.Abp.Content;
using IBLTermocasa.Shared;

namespace IBLTermocasa.Controllers.Components
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Component")]
    [Route("api/app/components")]

    public class ComponentController : AbpController, IComponentsAppService
    {
        protected IComponentsAppService _componentsAppService;

        public ComponentController(IComponentsAppService componentsAppService)
        {
            _componentsAppService = componentsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<ComponentDto>> GetListAsync(GetComponentsInput input)
        {
            return _componentsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<ComponentDto> GetAsync(Guid id)
        {
            return _componentsAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<ComponentDto> CreateAsync(ComponentCreateDto input)
        {
            return _componentsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<ComponentDto> UpdateAsync(Guid id, ComponentUpdateDto input)
        {
            return _componentsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _componentsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(ComponentExcelDownloadDto input)
        {
            return _componentsAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _componentsAppService.GetDownloadTokenAsync();
        }
        [HttpGet]
        [Route("component-items/{componentId}/delete/{componentItemId}")]
        public virtual Task<ComponentDto> DeleteComponentItemAsync(Guid componentId, Guid componentItemId)
        {
            return _componentsAppService.DeleteComponentItemAsync(componentId, componentItemId);
        }

        [HttpPut]
        [Route("component-items/{componentId}/update")]
        public virtual Task<ComponentDto> UpdateComponentItemAsync(Guid componentId, List<ComponentItemDto> componentItems)
        {
            return _componentsAppService.UpdateComponentItemAsync(componentId, componentItems);
        }

        [HttpPut]
        [Route("component-items/{componentId}/add")]
        public virtual Task<ComponentDto> CreateComponentItemAsync(Guid componentId, List<ComponentItemDto> componentItems)
        {
            return _componentsAppService.CreateComponentItemAsync(componentId, componentItems);
        }
        
        [HttpGet]
        [Route("material-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetMaterialLookupAsync(LookupRequestDto input)
        {
            return _componentsAppService.GetMaterialLookupAsync(input);
        }
        [HttpGet]
        [Route("component-materials")]
        public Task<Dictionary<Guid, List<MaterialDto>>> GetMaterialDictionaryAsync(List<Guid> componentIds)
        {
            return _componentsAppService.GetMaterialDictionaryAsync(componentIds);
        }
    }
}