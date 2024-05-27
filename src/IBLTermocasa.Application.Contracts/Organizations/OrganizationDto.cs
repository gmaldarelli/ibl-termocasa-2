using IBLTermocasa.Types;
using System;
using System.Collections.Generic;
using IBLTermocasa.Common;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Organizations
{
    public class OrganizationDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public OrganizationType OrganizationType { get; set; }
        public Guid? IndustryId { get; set; }
        public MailInfoDto MailInfo { get; set; } = new MailInfoDto();
        public PhoneInfoDto PhoneInfo { get; set; } = new PhoneInfoDto();
        public SocialInfoDto SocialInfo { get; set; } = new SocialInfoDto();
        public AddressDto BillingAddress { get; set; } = new AddressDto();
        public AddressDto ShippingAddress { get; set; } = new AddressDto();
        public List<string> Tags { get; set; } = new List<string>();
        public string? Notes { get; set; }
        public string? ImageId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
        public string Phones => PhoneInfo.PhoneItems.Count > 0 ? string.Join(", ", PhoneInfo.PhoneItems) : string.Empty;
        public string Emails => MailInfo.MailItems.Count > 0 ? string.Join(", ", MailInfo.MailItems) : string.Empty;

    }
}