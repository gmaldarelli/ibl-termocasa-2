using IBLTermocasa.Products;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using IBLTermocasa.Permissions;
using IBLTermocasa.Catalogs;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using IBLTermocasa.Shared;

namespace IBLTermocasa.Catalogs
{
    [RemoteService(IsEnabled = false)]
    [Authorize(IBLTermocasaPermissions.Catalogs.Default)]
    public class CatalogsAppService : IBLTermocasaAppService, ICatalogsAppService
    {
        protected IDistributedCache<CatalogExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        protected ICatalogRepository _catalogRepository;
        protected CatalogManager _catalogManager;
        protected IProductRepository _productRepository;

        public CatalogsAppService(ICatalogRepository catalogRepository, CatalogManager catalogManager, IDistributedCache<CatalogExcelDownloadTokenCacheItem, string> excelDownloadTokenCache, IProductRepository productRepository)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _catalogRepository = catalogRepository;
            _catalogManager = catalogManager; 
            _productRepository = productRepository;
        }

        public virtual async Task<PagedResultDto<CatalogWithNavigationPropertiesDto>> GetListAsync(GetCatalogsInput input)
        {
            var totalCount = await _catalogRepository.GetCountAsync(input.FilterText, input.Name, input.FromMin, input.FromMax, input.ToMin, input.ToMax, input.Description, input.ProductId);
            var items = await _catalogRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Name, input.FromMin, input.FromMax, input.ToMin, input.ToMax, input.Description, input.ProductId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<CatalogWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<CatalogWithNavigationProperties>, List<CatalogWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<CatalogWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<CatalogWithNavigationProperties, CatalogWithNavigationPropertiesDto>
                (await _catalogRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<CatalogDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Catalog, CatalogDto>(await _catalogRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetProductLookupAsync(LookupRequestDto input)
        {
            var query = (await _productRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null &&
                         x.Name.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Product>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Product>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(IBLTermocasaPermissions.Catalogs.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _catalogRepository.DeleteAsync(id);
        }

        [Authorize(IBLTermocasaPermissions.Catalogs.Create)]
        public virtual async Task<CatalogDto> CreateAsync(CatalogCreateDto input)
        {

            var catalog = await _catalogManager.CreateAsync(
            input.ProductIds, input.Name, input.From, input.To, input.Description
            );

            return ObjectMapper.Map<Catalog, CatalogDto>(catalog);
        }

        [Authorize(IBLTermocasaPermissions.Catalogs.Edit)]
        public virtual async Task<CatalogDto> UpdateAsync(Guid id, CatalogUpdateDto input)
        {

            var catalog = await _catalogManager.UpdateAsync(
            id,
            input.ProductIds, input.Name, input.From, input.To, input.Description, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Catalog, CatalogDto>(catalog);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(CatalogExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var catalogs = await _catalogRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Name, input.FromMin, input.FromMax, input.ToMin, input.ToMax, input.Description, input.ProductId);
            var items = catalogs.Select(item => new
            {
                Name = item.Catalog.Name,
                From = item.Catalog.From,
                To = item.Catalog.To,
                Description = item.Catalog.Description,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Catalogs.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public virtual async Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new CatalogExcelDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new IBLTermocasa.Shared.DownloadTokenResultDto
            {
                Token = token
            };
        }

        public virtual async Task<PagedResultDto<CatalogWithNavigationPropertiesDto>> GetListCatalogWithProducts(GetCatalogsInput input)
        {
            var totalCount = await _catalogRepository.GetCountAsync(input.FilterText, input.Name, input.FromMin, input.FromMax, input.ToMin, input.ToMax, input.Description, input.ProductId);
            var items = await _catalogRepository.GetListCatalogWithProducts(input.FilterText, input.Name, input.FromMin, input.FromMax, input.ToMin, input.ToMax, input.Description, input.ProductId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<CatalogWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<CatalogWithNavigationProperties>, List<CatalogWithNavigationPropertiesDto>>(items)
            };
        }
    }
}