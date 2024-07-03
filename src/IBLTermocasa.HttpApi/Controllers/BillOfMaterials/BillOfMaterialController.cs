using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using IBLTermocasa.BillOfMaterials;
using IBLTermocasa.Common;
using Volo.Abp.Content;
using IBLTermocasa.Shared;

namespace IBLTermocasa.Controllers.BillOfMaterials
{
    [RemoteService]
    [Area("app")]
    [ControllerName("BillOfMaterial")]
    [Route("api/app/bill-of-materials")]

    public class BillOfMaterialController : AbpController, IBillOfMaterialsAppService
    {
        protected IBillOfMaterialsAppService _billOfMaterialsAppService;

        public BillOfMaterialController(IBillOfMaterialsAppService billOfMaterialsAppService)
        {
            _billOfMaterialsAppService = billOfMaterialsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<BillOfMaterialDto>> GetListAsync(GetBillOfMaterialsInput input)
        {
            return _billOfMaterialsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<BillOfMaterialDto> GetAsync(Guid id)
        {
            return _billOfMaterialsAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<BillOfMaterialDto> CreateAsync(BillOfMaterialCreateDto input)
        {
            return _billOfMaterialsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<BillOfMaterialDto> UpdateAsync(Guid id, BillOfMaterialUpdateDto input)
        {
            return _billOfMaterialsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _billOfMaterialsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(BillOfMaterialExcelDownloadDto input)
        {
            return _billOfMaterialsAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _billOfMaterialsAppService.GetDownloadTokenAsync();
        }
        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> billofmaterialIds)
        {
            return _billOfMaterialsAppService.DeleteByIdsAsync(billofmaterialIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetBillOfMaterialsInput input)
        {
            return _billOfMaterialsAppService.DeleteAllAsync(input);
        }
        [HttpGet]
        [Route("generate-bill-of-material/{id}")]
        public virtual  Task<List<ViewElementPropertyDto<object>>> GenerateBillOfMaterial(Guid id)
        {
            if (id == Guid.Empty)
                throw new UserFriendlyException("RequestForQuotationItem-id is required");
            return _billOfMaterialsAppService.GenerateBillOfMaterial(id);
        }
        [HttpPost]
        [Route("calculate-consumption/{id}")]
        public virtual  Task<List<BomItemDto>> CalculateConsumption(Guid id, List<BomItemDto> listItems)
        {
            return _billOfMaterialsAppService.CalculateConsumption(id, listItems);
        }
        [HttpGet]
        [Route("calculate-consumption/{id}")]
        public virtual  Task<List<BomItemDto>> CalculateConsumption(Guid id)
        {
            return _billOfMaterialsAppService.CalculateConsumption(id);
        }
    }
}