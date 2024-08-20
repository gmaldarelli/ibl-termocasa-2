using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
using System.Linq;
using System.Linq.Dynamic.Core;

namespace IBLTermocasa.Contacts
{
    [RemoteService(IsEnabled = false)]
    [Authorize(IBLTermocasaPermissions.Contacts.Default)]
    public class ContactsAppService : IBLTermocasaAppService, IContactsAppService
    {
        protected IDistributedCache<ContactExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        protected IContactRepository _contactRepository;
        protected ContactManager _contactManager;

        public ContactsAppService(IContactRepository contactRepository, ContactManager contactManager, IDistributedCache<ContactExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _contactRepository = contactRepository;
            _contactManager = contactManager;
        }

        public virtual async Task<PagedResultDto<ContactDto>> GetListAsync(GetContactsInput input)
        {
            var totalCount = await _contactRepository.GetCountAsync(input.FilterText, input.Title, input.Name, input.Surname, input.ConfidentialName, input.JobRole, input.MailInfo, input.PhoneInfo, input.AddressInfo, input.Tag);
            var items = await _contactRepository.GetListAsync(input.FilterText, input.Title, input.Name, input.Surname, input.ConfidentialName, input.JobRole, input.MailInfo, input.PhoneInfo, input.AddressInfo, input.Tag, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<ContactDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Contact>, List<ContactDto>>(items)
            };
        }

        public virtual async Task<ContactDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Contact, ContactDto>(await _contactRepository.GetAsync(id));
        }

        [Authorize(IBLTermocasaPermissions.Contacts.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _contactRepository.DeleteAsync(id);
        }

        [Authorize(IBLTermocasaPermissions.Contacts.Create)]
        public virtual async Task<ContactDto> CreateAsync(ContactCreateDto input)
        {

            var contact = ObjectMapper.Map<ContactCreateDto, Contact>(input);
            return ObjectMapper.Map<Contact, ContactDto>(await _contactManager.CreateAsync(contact));
        }

        [Authorize(IBLTermocasaPermissions.Contacts.Edit)]
        public virtual async Task<ContactDto> UpdateAsync(Guid id, ContactUpdateDto input)
        {

            var contact = ObjectMapper.Map<ContactUpdateDto, Contact>(input);
            return ObjectMapper.Map<Contact, ContactDto>(await _contactManager.UpdateAsync(id, contact));
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(ContactExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _contactRepository.GetListAsync(input.FilterText, input.Title, input.Name, input.Surname, input.ConfidentialName, input.JobRole, input.MailInfo, input.PhoneInfo, input.AddressInfo, input.Tag);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<Contact>, List<ContactExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Contacts.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public virtual async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new ContactExcelDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new DownloadTokenResultDto
            {
                Token = token
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
    }
}