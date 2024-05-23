using IBLTermocasa.Components;
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
using IBLTermocasa.Products;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using IBLTermocasa.Shared;

namespace IBLTermocasa.Products
{
    [RemoteService(IsEnabled = false)]
    [Authorize(IBLTermocasaPermissions.Products.Default)]
    public abstract class ProductsAppServiceBase : IBLTermocasaAppService
    {
        protected IDistributedCache<ProductExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        protected IProductRepository _productRepository;
        protected ProductManager _productManager;
        protected IRepository<Component, Guid> _componentRepository;

        public ProductsAppServiceBase(IProductRepository productRepository, ProductManager productManager, IDistributedCache<ProductExcelDownloadTokenCacheItem, string> excelDownloadTokenCache, IRepository<Component, Guid> componentRepository)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _productRepository = productRepository;
            _productManager = productManager; _componentRepository = componentRepository;
        }

        public virtual async Task<PagedResultDto<ProductWithNavigationPropertiesDto>> GetListAsync(GetProductsInput input)
        {
            var totalCount = await _productRepository.GetCountAsync(input.FilterText, input.Code, input.Name, input.Description, input.IsAssembled, input.IsInternal);
            var items = await _productRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Code, input.Name, input.Description, input.IsAssembled, input.IsInternal, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<ProductWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<ProductWithNavigationProperties>, List<ProductWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<ProductWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<ProductWithNavigationProperties, ProductWithNavigationPropertiesDto>
                (await _productRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<ProductDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Product, ProductDto>(await _productRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetComponentLookupAsync(LookupRequestDto input)
        {
            var query = (await _componentRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null &&
                         x.Name.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Component>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Component>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(IBLTermocasaPermissions.Products.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _productRepository.DeleteAsync(id);
        }

        [Authorize(IBLTermocasaPermissions.Products.Create)]
        public virtual async Task<ProductDto> CreateAsync(ProductCreateDto input)
        {

            var product = await _productManager.CreateAsync(
            input.ComponentIds, input.Code, input.Name, input.IsAssembled, input.IsInternal, input.Description
            );

            return ObjectMapper.Map<Product, ProductDto>(product);
        }

        [Authorize(IBLTermocasaPermissions.Products.Edit)]
        public virtual async Task<ProductDto> UpdateAsync(Guid id, ProductUpdateDto input)
        {

            var product = await _productManager.UpdateAsync(
            id,
            input.ComponentIds, input.Code, input.Name, input.IsAssembled, input.IsInternal, input.Description, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Product, ProductDto>(product);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(ProductExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var products = await _productRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Code, input.Name, input.Description, input.IsAssembled, input.IsInternal);
            var items = products.Select(item => new
            {
                Code = item.Product.Code,
                Name = item.Product.Name,
                Description = item.Product.Description,
                IsAssembled = item.Product.IsAssembled,
                IsInternal = item.Product.IsInternal,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Products.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public virtual async Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new ProductExcelDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new IBLTermocasa.Shared.DownloadTokenResultDto
            {
                Token = token
            };
        }
        [Authorize(IBLTermocasaPermissions.Products.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> productIds)
        {
            await _productRepository.DeleteManyAsync(productIds);
        }

        [Authorize(IBLTermocasaPermissions.Products.Delete)]
        public virtual async Task DeleteAllAsync(GetProductsInput input)
        {
            await _productRepository.DeleteAllAsync(input.FilterText, input.Code, input.Name, input.Description, input.IsAssembled, input.IsInternal);
        }
    }
}