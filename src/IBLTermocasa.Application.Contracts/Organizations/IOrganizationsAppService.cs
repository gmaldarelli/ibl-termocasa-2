using IBLTermocasa.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using IBLTermocasa.Shared;
using IBLTermocasa.Types;

namespace IBLTermocasa.Organizations
{
    public interface IOrganizationsAppService : IApplicationService
    {

        Task<PagedResultDto<OrganizationWithNavigationPropertiesDto>> GetListAsync(GetOrganizationsInput input);

        Task<OrganizationWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<OrganizationDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetIndustryLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<OrganizationDto> CreateAsync(OrganizationCreateDto input);

        Task<OrganizationDto> UpdateAsync(Guid id, OrganizationUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(OrganizationExcelDownloadDto input);

        Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();
        
        Task<PagedResultDto<OrganizationDto>> GetFilterTypeAsync(GetOrganizationsInput? input, OrganizationType organizationType);
    }
}