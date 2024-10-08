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
        public List<ContactPropertyDto> ListContacts { get; set; } = new();
        public MailInfoDto MailInfo { get; set; } = new();
        public PhoneInfoDto PhoneInfo { get; set; } = new();
        public SocialInfoDto SocialInfo { get; set; } = new();
        public AddressDto BillingAddress { get; set; } = new();
        public AddressDto ShippingAddress { get; set; } = new();
        public List<string> Tags { get; set; } = new();
        public string? Notes { get; set; }
        public string? ImageId { get; set; }
        public SourceType SourceType { get; set; }
        public DateTime? FirstSync { get; set; }
        public DateTime? LastSync { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
        public string Contacts => ContactToString();
        
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
                    return $"{PhoneInfo.PhoneItems[0]} (+{count})";
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
                    return $"{MailInfo.MailItems[0]} (+{count})";
                }
            }
        }
        
        private string ContactToString()
        {
            switch (ListContacts.Count)
            {
                case 0:
                    return string.Empty;
                case 1:
                    return ListContacts[0].ToString();
                default:
                {
                    var count = ListContacts.Count -1; 
                    return $"{ListContacts[0].ToString()} (+{count})";
                }
            }
        }

    }
}