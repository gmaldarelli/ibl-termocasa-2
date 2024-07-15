using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using IBLTermocasa.Quotations;
using Volo.Abp.Content;
using IBLTermocasa.Shared;

namespace IBLTermocasa.Controllers.Quotations
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Quotation")]
    [Route("api/app/quotations")]

    public class QuotationController : AbpController, IQuotationsAppService
    {
        protected IQuotationsAppService _quotationsAppService;

        public QuotationController(IQuotationsAppService quotationsAppService)
        {
            _quotationsAppService = quotationsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<QuotationDto>> GetListAsync(GetQuotationsInput input)
        {
            return _quotationsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<QuotationDto> GetAsync(Guid id)
        {
            return _quotationsAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<QuotationDto> CreateAsync(QuotationCreateDto input)
        {
            return _quotationsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<QuotationDto> UpdateAsync(Guid id, QuotationUpdateDto input)
        {
            return _quotationsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _quotationsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(QuotationExcelDownloadDto input)
        {
            return _quotationsAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _quotationsAppService.GetDownloadTokenAsync();
        }
    }
}