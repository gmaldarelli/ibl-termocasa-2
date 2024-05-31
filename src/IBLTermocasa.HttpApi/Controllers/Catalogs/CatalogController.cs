using IBLTermocasa.Shared;
using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using IBLTermocasa.Catalogs;
using Volo.Abp.Content;
using IBLTermocasa.Shared;

namespace IBLTermocasa.Controllers.Catalogs
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Catalog")]
    [Route("api/app/catalogs")]

    public class CatalogController : AbpController, ICatalogsAppService
    {
        protected ICatalogsAppService _catalogsAppService;

        public CatalogController(ICatalogsAppService catalogsAppService)
        {
            _catalogsAppService = catalogsAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<CatalogWithNavigationPropertiesDto>> GetListAsync(GetCatalogsInput input)
        {
            return _catalogsAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public virtual Task<CatalogWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _catalogsAppService.GetWithNavigationPropertiesAsync(id);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<CatalogDto> GetAsync(Guid id)
        {
            return _catalogsAppService.GetAsync(id);
        }

        [HttpGet]
        [Route("product-lookup")]
        public virtual Task<PagedResultDto<LookupDto<Guid>>> GetProductLookupAsync(LookupRequestDto input)
        {
            return _catalogsAppService.GetProductLookupAsync(input);
        }

        [HttpPost]
        public virtual Task<CatalogDto> CreateAsync(CatalogCreateDto input)
        {
            return _catalogsAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<CatalogDto> UpdateAsync(Guid id, CatalogUpdateDto input)
        {
            return _catalogsAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _catalogsAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(CatalogExcelDownloadDto input)
        {
            return _catalogsAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _catalogsAppService.GetDownloadTokenAsync();
        }
        
        [HttpGet]
        [Route("list-catalog-with-products")]
        public Task<PagedResultDto<CatalogWithNavigationPropertiesDto>> GetListCatalogWithProducts(GetCatalogsInput input)
        {
            return _catalogsAppService.GetListCatalogWithProducts(input);
        }
    }
}