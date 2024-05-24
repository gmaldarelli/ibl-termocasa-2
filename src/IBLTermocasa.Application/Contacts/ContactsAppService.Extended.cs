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
    public class ContactsAppService : ContactsAppServiceBase, IContactsAppService
    {
        //<suite-custom-code-autogenerated>
        public ContactsAppService(IContactRepository contactRepository, ContactManager contactManager, IDistributedCache<ContactExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
            : base(contactRepository, contactManager, excelDownloadTokenCache)
        {
        }
        //</suite-custom-code-autogenerated>

        //Write your custom code...
    }
}