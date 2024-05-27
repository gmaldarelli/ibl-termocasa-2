using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using IBLTermocasa.Industries;
using IBLTermocasa.Permissions;
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

namespace IBLTermocasa.Organizations
{
    [RemoteService(IsEnabled = false)]
    [Authorize(IBLTermocasaPermissions.Organizations.Default)]
    public class OrganizationsAppService : IBLTermocasaAppService, IOrganizationsAppService
    {
        protected IDistributedCache<OrganizationExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        protected IOrganizationRepository _organizationRepository;
        protected OrganizationManager _organizationManager;
        protected IRepository<Industry, Guid> _industryRepository;

        public OrganizationsAppService(IOrganizationRepository organizationRepository,
            OrganizationManager organizationManager,
            IDistributedCache<OrganizationExcelDownloadTokenCacheItem, string> excelDownloadTokenCache,
            IRepository<Industry, Guid> industryRepository)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _organizationRepository = organizationRepository;
            _organizationManager = organizationManager;
            _industryRepository = industryRepository;
        }

        public virtual async Task<PagedResultDto<OrganizationWithNavigationPropertiesDto>> GetListAsync(
            GetOrganizationsInput input)
        {
            var totalCount = await _organizationRepository.GetCountAsync(input.FilterText, input.Code, input.Name,
                input.OrganizationType, input.MailInfo, input.PhoneInfo, input.Tags, input.IndustryId);
            var items = await _organizationRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.Code,
                input.Name, input.OrganizationType, input.MailInfo, input.PhoneInfo, input.Tags, input.IndustryId,
                input.OrganizationTypePreFiilter, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<OrganizationWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper
                    .Map<List<OrganizationWithNavigationProperties>, List<OrganizationWithNavigationPropertiesDto>>(
                        items)
            };
        }

        public virtual async Task<OrganizationWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<OrganizationWithNavigationProperties, OrganizationWithNavigationPropertiesDto>
                (await _organizationRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<OrganizationDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Organization, OrganizationDto>(await _organizationRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetIndustryLookupAsync(LookupRequestDto input)
        {
            var query = (await _industryRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Description != null &&
                         x.Description.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Industry>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Industry>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(IBLTermocasaPermissions.Organizations.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _organizationRepository.DeleteAsync(id);
        }

        [Authorize(IBLTermocasaPermissions.Organizations.Create)]
        public virtual async Task<OrganizationDto> CreateAsync(OrganizationCreateDto input)
        {
            if (input.IndustryId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Industry"]]);
            }

            var organization = ObjectMapper.Map<OrganizationCreateDto, Organization>(input);
            return ObjectMapper.Map<Organization, OrganizationDto>(
                await _organizationManager.CreateAsync(organization));
        }

        [Authorize(IBLTermocasaPermissions.Organizations.Edit)]
        public virtual async Task<OrganizationDto> UpdateAsync(Guid id, OrganizationUpdateDto input)
        {
            if (input.IndustryId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["Industry"]]);
            }

            var organization = ObjectMapper.Map<OrganizationUpdateDto, Organization>(input);
            return ObjectMapper.Map<Organization, OrganizationDto>(
                await _organizationManager.UpdateAsync(id, organization));
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(OrganizationExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var organizations = await _organizationRepository.GetListWithNavigationPropertiesAsync(input.FilterText,
                input.Code, input.Name, input.OrganizationType, input.MailInfo, input.PhoneInfo, input.Tags,
                input.IndustryId);
            var items = organizations.Select(item => new
            {
                Code = item.Organization.Code,
                Name = item.Organization.Name,
                OrganizationType = item.Organization.OrganizationType,
                MailInfo = item.Organization.MailInfo,
                PhoneInfo = item.Organization.PhoneInfo,
                SocialInfo = item.Organization.SocialInfo,
                BillingAddress = item.Organization.BillingAddress,
                ShippingAddress = item.Organization.ShippingAddress,
                Tags = item.Organization.Tags,
                Notes = item.Organization.Notes,
                ImageId = item.Organization.ImageId,

                Industry = item.Industry?.Description,
            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Organizations.xlsx",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public virtual async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new OrganizationExcelDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new DownloadTokenResultDto
            {
                Token = token
            };
        }
    }
}