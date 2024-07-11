using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using IBLTermocasa.ConsumptionEstimations;
using Volo.Abp.Content;
using IBLTermocasa.Shared;

namespace IBLTermocasa.Controllers.ConsumptionEstimations
{
    [RemoteService]
    [Area("app")]
    [ControllerName("ConsumptionEstimation")]
    [Route("api/app/consumption-estimations")]

    public class ConsumptionEstimationController : AbpController, IConsumptionEstimationsAppService
    {
        protected IConsumptionEstimationsAppService _consumptionEstimationsAppService;

        public ConsumptionEstimationController(IConsumptionEstimationsAppService consumptionEstimationsAppService)
        {
            _consumptionEstimationsAppService = consumptionEstimationsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<ConsumptionEstimationDto>> GetListAsync(GetConsumptionEstimationsInput input)
        {
            return _consumptionEstimationsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<ConsumptionEstimationDto> GetAsync(Guid id)
        {
            return _consumptionEstimationsAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<ConsumptionEstimationDto> CreateAsync(ConsumptionEstimationCreateDto input)
        {
            return _consumptionEstimationsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<ConsumptionEstimationDto> UpdateAsync(Guid id, ConsumptionEstimationUpdateDto input)
        {
            return _consumptionEstimationsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _consumptionEstimationsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(ConsumptionEstimationExcelDownloadDto input)
        {
            return _consumptionEstimationsAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _consumptionEstimationsAppService.GetDownloadTokenAsync();
        }
        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> consumptionestimationIds)
        {
            return _consumptionEstimationsAppService.DeleteByIdsAsync(consumptionestimationIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetConsumptionEstimationsInput input)
        {
            return _consumptionEstimationsAppService.DeleteAllAsync(input);
        }
        
        [HttpGet]
        [Route("product/{idProduct}")]
        public virtual Task<ConsumptionEstimationDto> GetAsyncByProduct(Guid idProduct)
        {
            return _consumptionEstimationsAppService.GetAsyncByProduct(idProduct);
        }
    }
}