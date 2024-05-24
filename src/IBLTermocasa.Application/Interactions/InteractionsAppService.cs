using Volo.Abp.Identity;
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
using IBLTermocasa.Interactions;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using IBLTermocasa.Shared;

namespace IBLTermocasa.Interactions
{
    [RemoteService(IsEnabled = false)]
    [Authorize(IBLTermocasaPermissions.Interactions.Default)]
    public class InteractionsAppService : IBLTermocasaAppService, IInteractionsAppService
    {
        protected IDistributedCache<InteractionExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        protected IInteractionRepository _interactionRepository;
        protected InteractionManager _interactionManager;
        protected IRepository<IdentityUser, Guid> _identityUserRepository;
        protected IRepository<OrganizationUnit, Guid> _organizationUnitRepository;

        public InteractionsAppService(IInteractionRepository interactionRepository, InteractionManager interactionManager, IDistributedCache<InteractionExcelDownloadTokenCacheItem, string> excelDownloadTokenCache, IRepository<IdentityUser, Guid> identityUserRepository, IRepository<OrganizationUnit, Guid> organizationUnitRepository)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _interactionRepository = interactionRepository;
            _interactionManager = interactionManager; _identityUserRepository = identityUserRepository;
            _organizationUnitRepository = organizationUnitRepository;
        }

        public virtual async Task<PagedResultDto<InteractionWithNavigationPropertiesDto>> GetListAsync(GetInteractionsInput input)
        {
            var totalCount = await _interactionRepository.GetCountAsync(input.FilterText, input.InteractionType, input.InteractionDateMin, input.InteractionDateMax, input.Content, input.ReferenceObject, input.WriterNotes, input.WriterUserId, input.IdentityUserId);
            var items = await _interactionRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.InteractionType, input.InteractionDateMin, input.InteractionDateMax, input.Content, input.ReferenceObject, input.WriterNotes, input.WriterUserId, input.IdentityUserId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<InteractionWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<InteractionWithNavigationProperties>, List<InteractionWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<InteractionWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<InteractionWithNavigationProperties, InteractionWithNavigationPropertiesDto>
                (await _interactionRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<InteractionDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Interaction, InteractionDto>(await _interactionRepository.GetAsync(id));
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

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetOrganizationUnitLookupAsync(LookupRequestDto input)
        {
            var query = (await _organizationUnitRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.DisplayName != null &&
                         x.DisplayName.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<OrganizationUnit>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<OrganizationUnit>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(IBLTermocasaPermissions.Interactions.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _interactionRepository.DeleteAsync(id);
        }

        [Authorize(IBLTermocasaPermissions.Interactions.Create)]
        public virtual async Task<InteractionDto> CreateAsync(InteractionCreateDto input)
        {
            if (input.WriterUserId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["IdentityUser"]]);
            }

            var interaction = await _interactionManager.CreateAsync(
            input.WriterUserId, input.NotificationOrganizationUnitId, input.IdentityUserId, input.InteractionType, input.InteractionDate, input.Content, input.ReferenceObject, input.WriterNotes
            );

            return ObjectMapper.Map<Interaction, InteractionDto>(interaction);
        }

        [Authorize(IBLTermocasaPermissions.Interactions.Edit)]
        public virtual async Task<InteractionDto> UpdateAsync(Guid id, InteractionUpdateDto input)
        {
            if (input.WriterUserId == default)
            {
                throw new UserFriendlyException(L["The {0} field is required.", L["IdentityUser"]]);
            }

            var interaction = await _interactionManager.UpdateAsync(
            id,
            input.WriterUserId, input.NotificationOrganizationUnitId, input.IdentityUserId, input.InteractionType, input.InteractionDate, input.Content, input.ReferenceObject, input.WriterNotes, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Interaction, InteractionDto>(interaction);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(InteractionExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var interactions = await _interactionRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.InteractionType, input.InteractionDateMin, input.InteractionDateMax, input.Content, input.ReferenceObject, input.WriterNotes, input.WriterUserId, input.IdentityUserId);
            var items = interactions.Select(item => new
            {
                InteractionType = item.Interaction.InteractionType,
                InteractionDate = item.Interaction.InteractionDate,
                Content = item.Interaction.Content,
                ReferenceObject = item.Interaction.ReferenceObject,
                WriterNotes = item.Interaction.WriterNotes,

                WriterUser = item.IdentityUser?.UserName,
                NotificationOrganizationUnit = item.OrganizationUnit?.DisplayName,
                IdentityUser = item.IdentityUser1?.UserName,

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Interactions.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public virtual async Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new InteractionExcelDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new IBLTermocasa.Shared.DownloadTokenResultDto
            {
                Token = token
            };
        }
    }
}