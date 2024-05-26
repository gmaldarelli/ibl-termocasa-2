using IBLTermocasa.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using IBLTermocasa.Shared;

namespace IBLTermocasa.RequestForQuotations
{
    public interface IRequestForQuotationsAppService : IApplicationService
    {

        Task<PagedResultDto<RequestForQuotationWithNavigationPropertiesDto>> GetListAsync(GetRequestForQuotationsInput input);

        Task<RequestForQuotationWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<RequestForQuotationDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetIdentityUserLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetContactLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetOrganizationLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<RequestForQuotationDto> CreateAsync(RequestForQuotationCreateDto input);

        Task<RequestForQuotationDto> UpdateAsync(Guid id, RequestForQuotationUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(RequestForQuotationExcelDownloadDto input);

        Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}