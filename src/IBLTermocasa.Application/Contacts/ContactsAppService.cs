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
using IBLTermocasa.Contacts;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using IBLTermocasa.Shared;

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

            var contact = await _contactManager.CreateAsync(
            input.Name, input.Surname, input.BirthDate, input.Title, input.ConfidentialName, input.JobRole, input.MailInfo, input.PhoneInfo, input.SocialInfo, input.AddressInfo, input.Tag, input.Notes
            );

            return ObjectMapper.Map<Contact, ContactDto>(contact);
        }

        [Authorize(IBLTermocasaPermissions.Contacts.Edit)]
        public virtual async Task<ContactDto> UpdateAsync(Guid id, ContactUpdateDto input)
        {

            var contact = await _contactManager.UpdateAsync(
            id,
            input.Name, input.Surname, input.BirthDate, input.Title, input.ConfidentialName, input.JobRole, input.MailInfo, input.PhoneInfo, input.SocialInfo, input.AddressInfo, input.Tag, input.Notes, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Contact, ContactDto>(contact);
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

        public virtual async Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new ContactExcelDownloadTokenCacheItem { Token = token },
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