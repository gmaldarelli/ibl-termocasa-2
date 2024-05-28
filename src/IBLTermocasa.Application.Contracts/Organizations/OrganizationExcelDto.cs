using IBLTermocasa.Types;
using System;
using System.Collections.Generic;
using IBLTermocasa.Common;

namespace IBLTermocasa.Organizations
{
    public class OrganizationExcelDto
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public OrganizationType OrganizationType { get; set; }
        public MailInfoDto? MailInfo { get; set; }
        public PhoneInfoDto? PhoneInfo { get; set; }
        public SocialInfoDto? SocialInfo { get; set; }
        public AddressDto? BillingAddress { get; set; }
        public AddressDto? ShippingAddress { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public SourceType SourceType { get; set; }
        public DateTime? FirstSync { get; set; }
        public DateTime? LastSync { get; set; }
    }
}