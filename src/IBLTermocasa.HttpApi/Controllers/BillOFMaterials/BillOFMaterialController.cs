using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using IBLTermocasa.BillOFMaterials;
using Volo.Abp.Content;
using IBLTermocasa.Shared;

namespace IBLTermocasa.Controllers.BillOFMaterials
{
    [RemoteService]
    [Area("app")]
    [ControllerName("BillOFMaterial")]
    [Route("api/app/bill-of-materials")]

    public class BillOFMaterialController : AbpController, IBillOFMaterialsAppService
    {
        protected IBillOFMaterialsAppService _billOFMaterialsAppService;

        public BillOFMaterialController(IBillOFMaterialsAppService billOFMaterialsAppService)
        {
            _billOFMaterialsAppService = billOFMaterialsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<BillOFMaterialDto>> GetListAsync(GetBillOFMaterialsInput input)
        {
            return _billOFMaterialsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<BillOFMaterialDto> GetAsync(Guid id)
        {
            return _billOFMaterialsAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<BillOFMaterialDto> CreateAsync(BillOFMaterialCreateDto input)
        {
            return _billOFMaterialsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<BillOFMaterialDto> UpdateAsync(Guid id, BillOFMaterialUpdateDto input)
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
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(BillOFMaterialExcelDownloadDto input)
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
        public virtual Task DeleteAllAsync(GetBillOFMaterialsInput input)
        {
            return _billOFMaterialsAppService.DeleteAllAsync(input);
        }
    }
}