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
        public SourceType SourceType { get; set; }
        public DateTime? FirstSync { get; set; }
        public DateTime? LastSync { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
        
        public string Phones => PhoneToString();

        public string Emails => MailToString();
        
        private string PhoneToString()
        {
            switch (PhoneInfo.PhoneItems.Count)
            {
                case 0:
                    return string.Empty;
                case 1:
                    return PhoneInfo.PhoneItems[0].ToString();
                default:
                {
                    var count = PhoneInfo.PhoneItems.Count -1; 
                    return $"{PhoneInfo.PhoneItems[0].ToString()} (+{count})";
                }
            }
        }
        
        private string MailToString()
        {
            switch (MailInfo.MailItems.Count)
            {
                case 0:
                    return string.Empty;
                case 1:
                    return MailInfo.MailItems[0].ToString();
                default:
                {
                    var count = MailInfo.MailItems.Count -1; 
                    return $"{MailInfo.MailItems[0].ToString()} (+{count})";
                }
            }
        }

    }
}