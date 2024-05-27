using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using IBLTermocasa.Components;
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
    }
}