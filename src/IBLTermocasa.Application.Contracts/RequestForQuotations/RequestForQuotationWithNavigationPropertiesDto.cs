using Volo.Abp.Identity;
using IBLTermocasa.Contacts;
using IBLTermocasa.Organizations;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace IBLTermocasa.RequestForQuotations
{
    public class RequestForQuotationWithNavigationPropertiesDto
    {
        public RequestForQuotationDto RequestForQuotation { get; set; } = null!;

        public IdentityUserDto IdentityUser { get; set; } = null!;
        public ContactDto Contact { get; set; } = null!;
        public OrganizationDto Organization { get; set; } = null!;

    }
}