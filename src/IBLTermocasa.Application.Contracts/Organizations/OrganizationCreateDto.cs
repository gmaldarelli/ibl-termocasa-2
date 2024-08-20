using IBLTermocasa.Types;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using IBLTermocasa.Common;

namespace IBLTermocasa.Organizations
{
    public class OrganizationCreateDto
    {
        [Required]
        public string Code { get; set; } = null!;
        [Required]
        public string Name { get; set; } = null!;
        public OrganizationType OrganizationType { get; set; } = ((OrganizationType[])Enum.GetValues(typeof(OrganizationType)))[0];
        public MailInfoDto MailInfo { get; set; } = new MailInfoDto();
        public PhoneInfoDto PhoneInfo { get; set; } = new PhoneInfoDto();
        public SocialInfoDto SocialInfo { get; set; } = new SocialInfoDto();
        public AddressDto BillingAddress { get; set; } = new AddressDto();
        public AddressDto ShippingAddress { get; set; } = new AddressDto();
        public List<string> Tags { get; set; } = new List<string>();
        public string? Notes { get; set; }
        public string? ImageId { get; set; }
        public Guid? IndustryId { get; set; }
        public List<ContactPropertyDto> ListContacts { get; set; } = new();
        public SourceType SourceType { get; set; }
        public DateTime? FirstSync { get; set; }
        public DateTime? LastSync { get; set; }
    }
}