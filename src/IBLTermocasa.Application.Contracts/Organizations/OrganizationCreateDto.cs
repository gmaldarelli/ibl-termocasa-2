using IBLTermocasa.Types;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace IBLTermocasa.Organizations
{
    public class OrganizationCreateDto
    {
        [Required]
        public string Code { get; set; } = null!;
        [Required]
        public string Name { get; set; } = null!;
        public OrganizationType OrganizationType { get; set; } = ((OrganizationType[])Enum.GetValues(typeof(OrganizationType)))[0];
        public string? MailInfo { get; set; }
        public string? PhoneInfo { get; set; }
        public string? SocialInfo { get; set; }
        public string? BillingAddress { get; set; }
        public string? ShippingAddress { get; set; }
        public string? Tags { get; set; }
        public string? Notes { get; set; }
        public string? ImageId { get; set; }
        public Guid IndustryId { get; set; }
    }
}