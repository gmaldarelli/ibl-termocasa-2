using IBLTermocasa.Shared;
using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using IBLTermocasa.RequestForQuotations;
using Volo.Abp.Content;
using IBLTermocasa.Shared;

namespace IBLTermocasa.Controllers.RequestForQuotations
{
    [RemoteService]
    [Area("app")]
    [ControllerName("RequestForQuotation")]
    [Route("api/app/request-for-quotations")]

    public class RequestForQuotationController : AbpController, IRequestForQuotationsAppService
    {
        protected IRequestForQuotationsAppService _requestForQuotationsAppService;

        public RequestForQuotationController(IRequestForQuotationsAppService requestForQuotationsAppService)
        {
            _requestForQuotationsAppService = requestForQuotationsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<RequestForQuotationWithNavigationPropertiesDto>> GetListAsync(GetRequestForQuotationsInput input)
        {
            return _requestForQuotationsAppService.GetListAsync(input);
        }
        
        [HttpGet]
        [Route("rfq-list")]
        public virtual Task<PagedResultDto<RequestForQuotationDto>> GetListRFQAsync(GetRequestForQuotationsInput input)
        {
            return _requestForQuotationsAppService.GetListRFQAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public virtual Task<RequestForQuotationWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _requestForQuotationsAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<RequestForQuotationDto> GetAsync(Guid id)
        {
            return _requestForQuotationsAppService.GetAsync(id);
        }
        
        [HttpGet]
        [Route("identity-user-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetIdentityUserLookupAsync(LookupRequestDto input)
        {
            return _requestForQuotationsAppService.GetIdentityUserLookupAsync(input);
        }

        [HttpGet]
        [Route("contact-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetContactLookupAsync(LookupRequestDto input)
        {
            return _requestForQuotationsAppService.GetContactLookupAsync(input);
        }
        
        [HttpGet]
        [Route("rfq-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetRequestForQuotationLookupAsync(LookupRequestDto input)
        {
            return _requestForQuotationsAppService.GetRequestForQuotationLookupAsync(input);
        }

        [HttpGet]
        [Route("organization-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetOrganizationLookupAsync(LookupRequestDto input)
        {
            return _requestForQuotationsAppService.GetOrganizationLookupAsync(input);
        }
        
        [HttpGet]
        [Route("organization-customer-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetOrganizationLookupCustomerAsync(LookupRequestDto input)
        {
            return _requestForQuotationsAppService.GetOrganizationLookupCustomerAsync(input);
        }

        [HttpPost]
        public virtual Task<RequestForQuotationDto> CreateAsync(RequestForQuotationCreateDto input)
        {
            return _requestForQuotationsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<RequestForQuotationDto> UpdateAsync(Guid id, RequestForQuotationUpdateDto input)
        {
            return _requestForQuotationsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _requestForQuotationsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(RequestForQuotationExcelDownloadDto input)
        {
            return _requestForQuotationsAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _requestForQuotationsAppService.GetDownloadTokenAsync();
        }
        
        [HttpGet]
        [Route("rfq-product-and-question/{id}")]
        public virtual Task<IEnumerable<RFQProductAndQuestionDto>> GetRfqProductAndQuestionsAsync(Guid id)
        {
            return _requestForQuotationsAppService.GetRfqProductAndQuestionsAsync(id);
        }
    }
}