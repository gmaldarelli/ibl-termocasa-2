using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using IBLTermocasa.Materials;
using Volo.Abp.Content;
using IBLTermocasa.Shared;

namespace IBLTermocasa.Controllers.Materials
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Material")]
    [Route("api/app/materials")]

    public class MaterialController : AbpController, IMaterialsAppService
    {
        protected IMaterialsAppService _materialsAppService;

        public MaterialController(IMaterialsAppService materialsAppService)
        {
            _materialsAppService = materialsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<MaterialDto>> GetListAsync(GetMaterialsInput input)
        {
            return _materialsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<MaterialDto> GetAsync(Guid id)
        {
            return _materialsAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<MaterialDto> CreateAsync(MaterialCreateDto input)
        {
            return _materialsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<MaterialDto> UpdateAsync(Guid id, MaterialUpdateDto input)
        {
            return _materialsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _materialsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(MaterialExcelDownloadDto input)
        {
            return _materialsAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _materialsAppService.GetDownloadTokenAsync();
        }
    }
}