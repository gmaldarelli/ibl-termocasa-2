using IBLTermocasa.Shared;
using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using IBLTermocasa.Products;
using Volo.Abp.Content;
using IBLTermocasa.Shared;

namespace IBLTermocasa.Controllers.Products
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Product")]
    [Route("api/app/products")]

    public class ProductController : AbpController, IProductsAppService
    {
        protected IProductsAppService _productsAppService;

        public ProductController(IProductsAppService productsAppService)
        {
            _productsAppService = productsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<ProductDto>> GetListAsync(GetProductsInput input, [FromQuery(Name = "includeDetails")] bool includeDetails = false)
        {
            return _productsAppService.GetListAsync(input, includeDetails);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<ProductDto> GetAsync(Guid id, [FromQuery(Name = "includeDetails")] bool includeDetails = false)
        {
            return _productsAppService.GetAsync(id, includeDetails);
        }

        [HttpGet]
        [Route("component-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetComponentLookupAsync(LookupRequestDto input)
        {
            return _productsAppService.GetComponentLookupAsync(input);
        }

        [HttpGet]
        [Route("question-template-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetQuestionTemplateLookupAsync(LookupRequestDto input)
        {
            return _productsAppService.GetQuestionTemplateLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<ProductDto> CreateAsync(ProductCreateDto input)
        {
            return _productsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<ProductDto> UpdateAsync(Guid id, ProductUpdateDto input)
        {
            return _productsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _productsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(ProductExcelDownloadDto input)
        {
            return _productsAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _productsAppService.GetDownloadTokenAsync();
        }
        [HttpDelete]
        [Route("")]
        public virtual Task DeleteByIdsAsync(List<Guid> productIds)
        {
            return _productsAppService.DeleteByIdsAsync(productIds);
        }

        [HttpDelete]
        [Route("all")]
        public virtual Task DeleteAllAsync(GetProductsInput input)
        {
            return _productsAppService.DeleteAllAsync(input);
        }
        
        [HttpGet]
        [Route("product-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetProductLookupAsync(LookupRequestDto input)
        {
            return _productsAppService.GetProductLookupAsync(input);
        }
        
        [HttpPost]
        [Route("ids")]
        public virtual Task<List<ProductDto>> GetListByIdAsync(List<Guid> ids)
        {
            return _productsAppService.GetListByIdAsync(ids);
        }
        
    }
}