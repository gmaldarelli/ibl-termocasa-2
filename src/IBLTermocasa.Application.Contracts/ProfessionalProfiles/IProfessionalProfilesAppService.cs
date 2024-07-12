using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using IBLTermocasa.Shared;

namespace IBLTermocasa.ProfessionalProfiles
{
    public interface IProfessionalProfilesAppService : IApplicationService
    {

        Task<PagedResultDto<ProfessionalProfileDto>> GetListAsync(GetProfessionalProfilesInput input);

        Task<ProfessionalProfileDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<ProfessionalProfileDto> CreateAsync(ProfessionalProfileCreateDto input);

        Task<ProfessionalProfileDto> UpdateAsync(Guid id, ProfessionalProfileUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(ProfessionalProfileExcelDownloadDto input);
        Task<PagedResultDto<LookupDto<Guid>>> GetProfessionalProfileLookupAsync(LookupRequestDto input);

        Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync(); Task DeleteByIdsAsync(List<Guid> professionalprofileIds);

        Task DeleteAllAsync(GetProfessionalProfilesInput input);
    }
}