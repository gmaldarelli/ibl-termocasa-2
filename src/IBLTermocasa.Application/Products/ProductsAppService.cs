using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using IBLTermocasa.Components;
using IBLTermocasa.Permissions;
using IBLTermocasa.QuestionTemplates;
using IBLTermocasa.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using MiniExcelLibs;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Content;
using Volo.Abp.Domain.Repositories;

namespace IBLTermocasa.Products
{
    [RemoteService(IsEnabled = false)]
    [Authorize(IBLTermocasaPermissions.Products.Default)]
    public class ProductsAppService : IBLTermocasaAppService, IProductsAppService
    {
        protected IDistributedCache<ProductExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        protected IProductRepository _productRepository;
        protected ProductManager _productManager;
        protected IRepository<Component, Guid> _componentRepository;
        protected IRepository<QuestionTemplate, Guid> _questionTemplateRepository;

        public ProductsAppService(IProductRepository productRepository, ProductManager productManager, IDistributedCache<ProductExcelDownloadTokenCacheItem, string> excelDownloadTokenCache, IRepository<Component, Guid> componentRepository, IRepository<QuestionTemplate, Guid> questionTemplateRepository)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _productRepository = productRepository;
            _productManager = productManager; _componentRepository = componentRepository;
            _questionTemplateRepository = questionTemplateRepository;
        }

        public virtual async Task<PagedResultDto<ProductDto>> GetListAsync(GetProductsInput input, bool includeDetails = false)
        {
            var totalCount = await _productRepository.GetCountAsync(input.FilterText, input.Code, input.Name, input.Description, input.IsAssembled, input.IsInternal);
            var fullEntityItems = await _productRepository.GetListAsync(input.FilterText, input.Code, input.Name, input.Description, input.IsAssembled, input.IsInternal, input.Sorting, input.MaxResultCount, input.SkipCount);
            var dtos = MapFullItemList(fullEntityItems);
            return new PagedResultDto<ProductDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Product>, List<ProductDto>>(fullEntityItems)
                    //await MapFullItemList(fullEntityItems, includeDetails)
            };
        }

        public virtual async Task<ProductDto> GetAsync(Guid id, bool includeDetails = false)
        {
            var fullEntity = await _productRepository.GetAsync(id);
            var dto = ObjectMapper.Map<Product, ProductDto>(fullEntity);
                //await MapFullItem(fullEntity, includeDetails);
            return dto;
        }

        private  async Task<ProductDto> MapFullItem(Product fullEntity, bool includeDetails = false)
        {
            List<Product> fullEntityList = new List<Product> { fullEntity };
            return (await MapFullItemList(fullEntityList, includeDetails)).First();
        }
        private async Task<List<ProductDto>> MapFullItemList(List<Product> fullEntity, bool includeDetails = false)
        {
            var dtos = ObjectMapper.Map<List<Product>, List<ProductDto>>(fullEntity).ToList();
            List<Guid> questionTemplateIds =  fullEntity.SelectMany(x => x.ProductQuestionTemplates).Select(x => x.QuestionTemplateId).ToList();
            var componentIds = fullEntity.SelectMany(x => x.ProductComponents).Select(x => x.ComponentId).ToList();
            if (includeDetails)
            {
                var questionTemplates = await _questionTemplateRepository.GetListAsync(x => questionTemplateIds.Contains(x.Id));
                var components = await _componentRepository.GetListAsync(x => componentIds.Contains(x.Id));
                dtos.ForEach(dto =>
                {
                    foreach (var componentId in componentIds)
                    {
                        fullEntity.ForEach(entity =>
                        {
                            var productComponent = entity.ProductComponents.First(x => x.ComponentId == componentId);
                            var component = components.First(x => x.Id == productComponent.ComponentId);
                            var productComponentDto =
                                ObjectMapper.Map<ProductComponent, ProductComponentDto>(productComponent);
                           // productComponentDto.Component = ObjectMapper.Map<Component, ComponentDto>(component);
                            dto.ProductComponents.Add(productComponentDto);
                        });
                    }

                    foreach (var questionTemplateId in questionTemplateIds)
                    {
                        fullEntity.ForEach(entity =>
                        {
                            var productQuestionTemplate =
                                entity.ProductQuestionTemplates.First(x => x.QuestionTemplateId == questionTemplateId);
                            var questionTemplate =
                                questionTemplates.First(x => x.Id == productQuestionTemplate.QuestionTemplateId);
                            var productQuestionTemplateDto =
                                ObjectMapper.Map<ProductQuestionTemplate, ProductQuestionTemplateDto>(
                                    productQuestionTemplate);
                            //productQuestionTemplateDto.QuestionTemplate =
                                ObjectMapper.Map<QuestionTemplate, QuestionTemplateDto>(questionTemplate);
                            dto.ProductQuestionTemplates.Add(productQuestionTemplateDto);
                        });
                    } 
                });
            }
            return dtos;
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

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetQuestionTemplateLookupAsync(LookupRequestDto input)
        {
            var query = (await _questionTemplateRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.QuestionText != null &&
                         x.QuestionText.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<QuestionTemplate>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<QuestionTemplate>, List<LookupDto<Guid>>>(lookupData)
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
            var productComponents = ObjectMapper.Map<List<ProductComponentDto>, List<ProductComponent>>(input.ProductComponents);
            var productQuestionTemplates = ObjectMapper.Map<List<ProductQuestionTemplateDto>, List<ProductQuestionTemplate>>(input.ProductQuestionTemplates);
            var subProducts = ObjectMapper.Map<List<SubProductDto>, List<SubProduct>>(input.SubProducts);
            var product = await _productManager.CreateAsync( subProducts, 
                productComponents, productQuestionTemplates, input.Code, input.Name, input.IsAssembled, input.IsInternal, input.Description
            );

            return ObjectMapper.Map<Product, ProductDto>(product);
        }

        [Authorize(IBLTermocasaPermissions.Products.Edit)]
        public virtual async Task<ProductDto> UpdateAsync(Guid id, ProductUpdateDto input)
        {
            var productComponents = ObjectMapper.Map<List<ProductComponentDto>, List<ProductComponent>>(input.ProductComponents);
            var productQuestionTemplates = ObjectMapper.Map<List<ProductQuestionTemplateDto>, List<ProductQuestionTemplate>>(input.ProductQuestionTemplates);
            var subProducts = ObjectMapper.Map<List<SubProductDto>, List<SubProduct>>(input.SubProducts);
            productComponents.ForEach(x => x.ProductId = id);
            productQuestionTemplates.ForEach(x => x.ProductId = id);
            var product = await _productManager.UpdateAsync(
            id, subProducts, productComponents, 
            productQuestionTemplates, input.Code, 
            input.Name, input.IsAssembled, input.IsInternal, 
            input.Description, input.ConcurrencyStamp);

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

        public virtual async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new ProductExcelDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new DownloadTokenResultDto
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
        
        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetProductLookupAsync(LookupRequestDto input)
        {
            var query = (await _productRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null &&
                        x.Name.Contains(input.Filter) || x.Code != null && x.Code.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Product>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Product>, List<LookupDto<Guid>>>(lookupData)
            };
        }
    }
}