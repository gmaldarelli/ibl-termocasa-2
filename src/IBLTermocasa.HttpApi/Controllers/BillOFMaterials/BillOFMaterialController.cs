using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using IBLTermocasa.BillOfMaterials;
using Volo.Abp.Content;
using IBLTermocasa.Shared;

namespace IBLTermocasa.Controllers.BillOFMaterials
{
    [RemoteService]
    [Area("app")]
    [ControllerName("BillOFMaterial")]
    [Route("api/app/bill-of-materials")]

    public class BillOFMaterialController : AbpController, IBillOfMaterialsAppService
    {
        protected IBillOfMaterialsAppService _billOFMaterialsAppService;

        public BillOFMaterialController(IBillOfMaterialsAppService billOFMaterialsAppService)
        {
            _billOFMaterialsAppService = billOFMaterialsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<BillOfMaterialDto>> GetListAsync(GetBillOfMaterialsInput input)
        {
            return _billOFMaterialsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<BillOfMaterialDto> GetAsync(Guid id)
        {
            return _billOFMaterialsAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<BillOfMaterialDto> CreateAsync(BillOfMaterialCreateDto input)
        {
            return _billOFMaterialsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<BillOfMaterialDto> UpdateAsync(Guid id, BillOfMaterialUpdateDto input)
        {
            return _billOFMaterialsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _billOFMaterialsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(BillOfMaterialExcelDownloadDto input)
        {
            return _billOFMaterialsAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _billOFMaterialsAppService.GetDownloadTokenAsync();
        }
        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> billofmaterialIds)
        {
            return _billOFMaterialsAppService.DeleteByIdsAsync(billofmaterialIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetBillOfMaterialsInput input)
        {
            return _billOFMaterialsAppService.DeleteAllAsync(input);
        }
    }
}