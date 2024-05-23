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

    public abstract class ProductControllerBase : AbpController
    {
        protected IProductsAppService _productsAppService;

        public ProductControllerBase(IProductsAppService productsAppService)
        {
            _productsAppService = productsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<ProductWithNavigationPropertiesDto>> GetListAsync(GetProductsInput input)
        {
            return _productsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public virtual Task<ProductWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _productsAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<ProductDto> GetAsync(Guid id)
        {
            return _productsAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("component-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetComponentLookupAsync(LookupRequestDto input)
        {
            return _productsAppService.GetComponentLookupAsync(input);
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
    }
}