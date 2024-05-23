using IBLTermocasa.Shared;
using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using IBLTermocasa.Subproducts;

namespace IBLTermocasa.Controllers.Subproducts
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Subproduct")]
    [Route("api/app/subproducts")]

    public abstract class SubproductControllerBase : AbpController
    {
        protected ISubproductsAppService _subproductsAppService;

        public SubproductControllerBase(ISubproductsAppService subproductsAppService)
        {
            _subproductsAppService = subproductsAppService;
        }

        [HttpGet]
        [Route("by-product")]
        public virtual Task<PagedResultDto<SubproductDto>> GetListByProductIdAsync(GetSubproductListInput input)
        {
            return _subproductsAppService.GetListByProductIdAsync(input);
        }
        [HttpGet]
        [Route("detailed/by-product")]
        public virtual Task<PagedResultDto<SubproductWithNavigationPropertiesDto>> GetListWithNavigationPropertiesByProductIdAsync(GetSubproductListInput input)
        {
            return _subproductsAppService.GetListWithNavigationPropertiesByProductIdAsync(input);
        }

        [HttpGet]
        public virtual Task<PagedResultDto<SubproductWithNavigationPropertiesDto>> GetListAsync(GetSubproductsInput input)
        {
            return _subproductsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public virtual Task<SubproductWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _subproductsAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<SubproductDto> GetAsync(Guid id)
        {
            return _subproductsAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("product-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetProductLookupAsync(LookupRequestDto input)
        {
            return _subproductsAppService.GetProductLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<SubproductDto> CreateAsync(SubproductCreateDto input)
        {
            return _subproductsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<SubproductDto> UpdateAsync(Guid id, SubproductUpdateDto input)
        {
            return _subproductsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _subproductsAppService.DeleteAsync(id);
        }
    }
}