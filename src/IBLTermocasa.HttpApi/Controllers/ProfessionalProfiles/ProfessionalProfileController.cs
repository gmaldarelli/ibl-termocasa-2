using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using IBLTermocasa.ProfessionalProfiles;
using Volo.Abp.Content;
using IBLTermocasa.Shared;

namespace IBLTermocasa.Controllers.ProfessionalProfiles
{
    [RemoteService]
    [Area("app")]
    [ControllerName("ProfessionalProfile")]
    [Route("api/app/professional-profiles")]

    public class ProfessionalProfileController : AbpController, IProfessionalProfilesAppService
    {
        protected IProfessionalProfilesAppService _professionalProfilesAppService;

        public ProfessionalProfileController(IProfessionalProfilesAppService professionalProfilesAppService)
        {
            _professionalProfilesAppService = professionalProfilesAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<ProfessionalProfileDto>> GetListAsync(GetProfessionalProfilesInput input)
        {
            return _professionalProfilesAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<ProfessionalProfileDto> GetAsync(Guid id)
        {
            return _professionalProfilesAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<ProfessionalProfileDto> CreateAsync(ProfessionalProfileCreateDto input)
        {
            return _professionalProfilesAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<ProfessionalProfileDto> UpdateAsync(Guid id, ProfessionalProfileUpdateDto input)
        {
            return _professionalProfilesAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _professionalProfilesAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(ProfessionalProfileExcelDownloadDto input)
        {
            return _professionalProfilesAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _professionalProfilesAppService.GetDownloadTokenAsync();
        }
        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> professionalprofileIds)
        {
            return _professionalProfilesAppService.DeleteByIdsAsync(professionalprofileIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetProfessionalProfilesInput input)
        {
            return _professionalProfilesAppService.DeleteAllAsync(input);
        }
        
        [HttpGet]
        [Route("professional-profile-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetProfessionalProfileLookupAsync(LookupRequestDto input)
        {
            return _professionalProfilesAppService.GetProfessionalProfileLookupAsync(input);
        }
    }
}