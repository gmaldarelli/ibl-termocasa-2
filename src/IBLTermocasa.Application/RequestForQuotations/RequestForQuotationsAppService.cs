using IBLTermocasa.Organizations;
using IBLTermocasa.Contacts;
using Volo.Abp.Identity;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using IBLTermocasa.Common;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using IBLTermocasa.Permissions;
using IBLTermocasa.Products;
using IBLTermocasa.QuestionTemplates;
using IBLTermocasa.RequestForQuotations;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using IBLTermocasa.Shared;
using IBLTermocasa.Types;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.RequestForQuotations
{
    [RemoteService(IsEnabled = false)]
    [Authorize(IBLTermocasaPermissions.RequestForQuotations.Default)]
    public class RequestForQuotationsAppService : IBLTermocasaAppService, IRequestForQuotationsAppService
    {
        protected IDistributedCache<RequestForQuotationExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        protected IRequestForQuotationRepository _requestForQuotationRepository;
        protected RequestForQuotationManager _requestForQuotationManager;
        protected IRepository<IdentityUser, Guid> _identityUserRepository;
        protected IRepository<Contact, Guid> _contactRepository;
        protected IRepository<Organization, Guid> _organizationRepository;
        protected IProductRepository _productRepository;
        protected IQuestionTemplateRepository _questionTemplateRepository;

        public RequestForQuotationsAppService(IRequestForQuotationRepository requestForQuotationRepository, RequestForQuotationManager requestForQuotationManager, IDistributedCache<RequestForQuotationExcelDownloadTokenCacheItem, string> excelDownloadTokenCache, IRepository<IdentityUser, Guid> identityUserRepository, IRepository<Contact, Guid> contactRepository, IRepository<Organization, Guid> organizationRepository, IProductRepository productRepository, IQuestionTemplateRepository questionTemplateRepository)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _requestForQuotationRepository = requestForQuotationRepository;
            _requestForQuotationManager = requestForQuotationManager;
            _identityUserRepository = identityUserRepository;
            _contactRepository = contactRepository;
            _organizationRepository = organizationRepository;
            _productRepository = productRepository;
            _questionTemplateRepository = questionTemplateRepository;
        }

        public virtual async Task<PagedResultDto<RequestForQuotationWithNavigationPropertiesDto>> GetListAsync(GetRequestForQuotationsInput input)
        {
            var totalCount = await _requestForQuotationRepository.GetCountAsync(input.FilterText, input.QuoteNumber, input.WorkSite, input.City, input.AgentProperty, input.OrganizationProperty, input.ContactProperty, input.PhoneInfo, input.MailInfo, input.DiscountMin, input.DiscountMax, input.Description, input.Status);
            var items = await _requestForQuotationRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.QuoteNumber, input.WorkSite, input.City, input.AgentProperty ,input.OrganizationProperty, input.ContactProperty, input.PhoneInfo, input.MailInfo, input.DiscountMin, input.DiscountMax, input.Description, input.Status, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<RequestForQuotationWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<RequestForQuotationWithNavigationProperties>, List<RequestForQuotationWithNavigationPropertiesDto>>(items)
            };
        }
        
        public virtual async Task<PagedResultDto<RequestForQuotationDto>> GetListRFQAsync(GetRequestForQuotationsInput input)
        {
            var totalCount = await _requestForQuotationRepository.GetCountAsync(input.FilterText, input.QuoteNumber, input.WorkSite, input.City, input.AgentProperty, input.OrganizationProperty, input.ContactProperty, input.PhoneInfo, input.MailInfo, input.DiscountMin, input.DiscountMax, input.Description, input.Status);
            var items = await _requestForQuotationRepository.GetListAsync(input.FilterText, input.QuoteNumber, input.WorkSite, input.City, input.AgentProperty ,input.OrganizationProperty, input.ContactProperty, input.PhoneInfo, input.MailInfo, input.DiscountMin, input.DiscountMax, input.Description, input.Status, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<RequestForQuotationDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<RequestForQuotation>, List<RequestForQuotationDto>>(items)
            };
        }
        
        public virtual async Task<RequestForQuotationWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<RequestForQuotationWithNavigationProperties, RequestForQuotationWithNavigationPropertiesDto>
                (await _requestForQuotationRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<RequestForQuotationDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<RequestForQuotation, RequestForQuotationDto>(await _requestForQuotationRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetIdentityUserLookupAsync(LookupRequestDto input)
        {
            var query = (await _identityUserRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.UserName != null &&
                         x.UserName.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<IdentityUser>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<IdentityUser>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetContactLookupAsync(LookupRequestDto input)
        {
            var query = (await _contactRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null &&
                         x.Name.Contains(input.Filter) || x.Surname != null && x.Surname.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Contact>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Contact>, List<LookupDto<Guid>>>(lookupData)
            };
        }
        
        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetRequestForQuotationLookupAsync(LookupRequestDto input)
        {
            var query = (await _requestForQuotationRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.QuoteNumber != null &&
                        x.QuoteNumber.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<RequestForQuotation>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<RequestForQuotation>, List<LookupDto<Guid>>>(lookupData)
            };
        }
        
        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetOrganizationLookupAsync(LookupRequestDto input)
        {
            var query = (await _organizationRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null &&
                         x.Name.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Organization>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Organization>, List<LookupDto<Guid>>>(lookupData)
            };
        }
        
        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetOrganizationLookupCustomerAsync(LookupRequestDto input)
        {
            var query = (await _organizationRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null &&
                         x.Name.Contains(input.Filter) && x.OrganizationType == OrganizationType.CUSTOMER);

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Organization>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Organization>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(IBLTermocasaPermissions.RequestForQuotations.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _requestForQuotationRepository.DeleteAsync(id);
        }

        [Authorize(IBLTermocasaPermissions.RequestForQuotations.Create)]
        public virtual async Task<RequestForQuotationDto> CreateAsync(RequestForQuotationCreateDto input)
        {
            var requestForQuotation = ObjectMapper.Map<RequestForQuotationCreateDto, RequestForQuotation>(input);
            return ObjectMapper.Map<RequestForQuotation, RequestForQuotationDto>(await _requestForQuotationManager.CreateAsync(requestForQuotation));
        }

        [Authorize(IBLTermocasaPermissions.RequestForQuotations.Edit)]
        public virtual async Task<RequestForQuotationDto> UpdateAsync(Guid id, RequestForQuotationUpdateDto input)
        {
            var requestForQuotationDto = ObjectMapper.Map<RequestForQuotationUpdateDto, RequestForQuotation>(input);
            return ObjectMapper.Map<RequestForQuotation, RequestForQuotationDto>(await _requestForQuotationManager.UpdateAsync(id, requestForQuotationDto));
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(RequestForQuotationExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var requestForQuotations = await _requestForQuotationRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.QuoteNumber, input.WorkSite, input.City, input.AgentProperty, input.OrganizationProperty, input.ContactProperty, input.PhoneInfo, input.MailInfo, input.DiscountMin, input.DiscountMax, input.Description, input.Status);
            var items = requestForQuotations.Select(item => new
            {
                QuoteNumber = item.RequestForQuotation.QuoteNumber,
                WorkSite = item.RequestForQuotation.WorkSite,
                City = item.RequestForQuotation.City,
                AgentProperty = item.RequestForQuotation.AgentProperty,
                OrganizationProperty = item.RequestForQuotation.OrganizationProperty,
                ContactProperty = item.RequestForQuotation.ContactProperty,
                PhoneInfo = item.RequestForQuotation.PhoneInfo,
                MailInfo = item.RequestForQuotation.MailInfo,
                Discount = item.RequestForQuotation.Discount,
                Description = item.RequestForQuotation.Description,
                Status = item.RequestForQuotation.Status
            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "RequestForQuotations.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public virtual async Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new RequestForQuotationExcelDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new IBLTermocasa.Shared.DownloadTokenResultDto
            {
                Token = token
            };
        }
        
        public virtual async Task<IEnumerable<RFQProductAndQuestionDto>> GetRfqProductAndQuestionsAsync(Guid id)
        {
            // Ottengo il prodotto principale
            var productPrincipal = await _productRepository.GetAsync(id);
            if (productPrincipal == null)
            {
                throw new EntityNotFoundException($"Product with ID {id} not found");
            }

            // Prendo tutti gli ID dei sotto-prodotti
            var listProductIds = productPrincipal.SubProducts.Select(sp => sp.ProductIds).SelectMany(p => p).ToList();

            // Prendo tutti i prodotti associati ai sotto-prodotti
            var products = await _productRepository.GetListAsync(p => listProductIds.Contains(p.Id));
            products.Add(productPrincipal);
            
            var listProduct = new List<RFQProductAndQuestionDto>();

            // Per ogni prodotto, prendo le domande associate
            foreach (var product in products)
            {
                var questionTemplateIds = product.ProductQuestionTemplates.Select(qt => qt.QuestionTemplateId).ToList();
                
                // Prendo tutte le domande associate ai sotto-prodotti e le mappo in DTO
                var questionTemplates = ObjectMapper.Map<List<QuestionTemplate>, List<QuestionTemplateDto>>(
                    await _questionTemplateRepository.GetListAsync(q => questionTemplateIds.Contains(q.Id))
                    );
                // Mappo il prodotto in DTO e creo un oggetto RFQProductAndQuestionDto con il prodotto e le domande associate
                var rfqProductAndQuestionDto = new RFQProductAndQuestionDto(ObjectMapper.Map<Product, ProductDto>(product), questionTemplates);
                listProduct.Add(rfqProductAndQuestionDto);
            }
            
            return listProduct;
        }
    }
}