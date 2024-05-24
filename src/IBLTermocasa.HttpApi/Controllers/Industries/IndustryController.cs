using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using IBLTermocasa.Industries;

namespace IBLTermocasa.Controllers.Industries
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Industry")]
    [Route("api/app/industries")]

    public class IndustryController : AbpController, IIndustriesAppService
    {
        protected IIndustriesAppService _industriesAppService;

        public IndustryController(IIndustriesAppService industriesAppService)
        {
            _industriesAppService = industriesAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<IndustryDto>> GetListAsync(GetIndustriesInput input)
        {
            return _industriesAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<IndustryDto> GetAsync(Guid id)
        {
            return _industriesAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<IndustryDto> CreateAsync(IndustryCreateDto input)
        {
            return _industriesAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<IndustryDto> UpdateAsync(Guid id, IndustryUpdateDto input)
        {
            return _industriesAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _industriesAppService.DeleteAsync(id);
        }
        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> industryIds)
        {
            return _industriesAppService.DeleteByIdsAsync(industryIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetIndustriesInput input)
        {
            return _industriesAppService.DeleteAllAsync(input);
        }
    }
}