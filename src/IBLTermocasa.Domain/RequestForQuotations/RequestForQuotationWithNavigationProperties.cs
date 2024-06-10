using Volo.Abp.Identity;
using IBLTermocasa.Contacts;
using IBLTermocasa.Organizations;

using System;
using System.Collections.Generic;

namespace IBLTermocasa.RequestForQuotations
{
    public  class RequestForQuotationWithNavigationProperties
    {
        public RequestForQuotation RequestForQuotation { get; set; } = null!;

        public IdentityUser IdentityUser { get; set; } = null!;
        public Contact Contact { get; set; } = null!;
        public Organization Organization { get; set; } = null!;
    }
}