using IBLTermocasa.Shared;
using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using IBLTermocasa.Organizations;
using Volo.Abp.Content;
using IBLTermocasa.Shared;

namespace IBLTermocasa.Controllers.Organizations
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Organization")]
    [Route("api/app/organizations")]

    public class OrganizationController : AbpController, IOrganizationsAppService
    {
        protected IOrganizationsAppService _organizationsAppService;

        public OrganizationController(IOrganizationsAppService organizationsAppService)
        {
            _organizationsAppService = organizationsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<OrganizationWithNavigationPropertiesDto>> GetListAsync(GetOrganizationsInput input)
        {
            return _organizationsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public virtual Task<OrganizationWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _organizationsAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<OrganizationDto> GetAsync(Guid id)
        {
            return _organizationsAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("industry-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetIndustryLookupAsync(LookupRequestDto input)
        {
            return _organizationsAppService.GetIndustryLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<OrganizationDto> CreateAsync(OrganizationCreateDto input)
        {
            return _organizationsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<OrganizationDto> UpdateAsync(Guid id, OrganizationUpdateDto input)
        {
            return _organizationsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _organizationsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(OrganizationExcelDownloadDto input)
        {
            return _organizationsAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _organizationsAppService.GetDownloadTokenAsync();
        }
    }
}