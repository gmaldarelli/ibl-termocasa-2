using IBLTermocasa.Shared;
using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using IBLTermocasa.Interactions;
using Volo.Abp.Content;
using IBLTermocasa.Shared;

namespace IBLTermocasa.Controllers.Interactions
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Interaction")]
    [Route("api/app/interactions")]

    public class InteractionController : AbpController, IInteractionsAppService
    {
        protected IInteractionsAppService _interactionsAppService;

        public InteractionController(IInteractionsAppService interactionsAppService)
        {
            _interactionsAppService = interactionsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<InteractionWithNavigationPropertiesDto>> GetListAsync(GetInteractionsInput input)
        {
            return _interactionsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public virtual Task<InteractionWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _interactionsAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<InteractionDto> GetAsync(Guid id)
        {
            return _interactionsAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("identity-user-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetIdentityUserLookupAsync(LookupRequestDto input)
        {
            return _interactionsAppService.GetIdentityUserLookupAsync(input);
        }

        [HttpGet]
        [Route("organization-unit-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetOrganizationUnitLookupAsync(LookupRequestDto input)
        {
            return _interactionsAppService.GetOrganizationUnitLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<InteractionDto> CreateAsync(InteractionCreateDto input)
        {
            return _interactionsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<InteractionDto> UpdateAsync(Guid id, InteractionUpdateDto input)
        {
            return _interactionsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _interactionsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(InteractionExcelDownloadDto input)
        {
            return _interactionsAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _interactionsAppService.GetDownloadTokenAsync();
        }
    }
}