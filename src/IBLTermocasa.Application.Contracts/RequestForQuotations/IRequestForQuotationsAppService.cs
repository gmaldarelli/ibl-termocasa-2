using IBLTermocasa.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IBLTermocasa.Common;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using IBLTermocasa.Shared;

namespace IBLTermocasa.RequestForQuotations
{
    public interface IRequestForQuotationsAppService : IApplicationService
    {

        Task<PagedResultDto<RequestForQuotationWithNavigationPropertiesDto>> GetListAsync(GetRequestForQuotationsInput input);
        Task<PagedResultDto<RequestForQuotationDto>> GetListRFQAsync(GetRequestForQuotationsInput input);

        Task<RequestForQuotationWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<RequestForQuotationDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetIdentityUserLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetContactLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetRequestForQuotationLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetOrganizationLookupAsync(LookupRequestDto input);
        
        Task<PagedResultDto<LookupDto<Guid>>> GetOrganizationLookupCustomerAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<RequestForQuotationDto> CreateAsync(RequestForQuotationCreateDto input);

        Task<RequestForQuotationDto> UpdateAsync(Guid id, RequestForQuotationUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(RequestForQuotationExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
        Task<IEnumerable<RFQProductAndQuestionDto>> GetRfqProductAndQuestionsAsync(Guid id);
        Task<ViewElementPropertyDto<long>>  GetNewRequestForQuotationCountAsync();
    }
}