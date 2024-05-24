using Asp.Versioning;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using IBLTermocasa.Contacts;

namespace IBLTermocasa.Controllers.Contacts
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Contact")]
    [Route("api/app/contacts")]

    public class ContactController : ContactControllerBase, IContactsAppService
    {
        public ContactController(IContactsAppService contactsAppService) : base(contactsAppService)
        {
        }
    }
}