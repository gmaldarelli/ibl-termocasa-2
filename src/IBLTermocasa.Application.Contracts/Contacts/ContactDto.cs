using System;
using System.Collections.Generic;
using IBLTermocasa.Common;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Contacts
{
    public class ContactDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string? Title { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string? ConfidentialName { get; set; }
        public string? JobRole { get; set; }
        public DateTime? BirthDate { get; set; }
        public PhoneInfoDto PhoneInfo { get; set; } = new PhoneInfoDto();
        public MailInfoDto MailInfo { get; set; } = new MailInfoDto();
        public SocialInfoDto SocialInfo { get; set; } = new SocialInfoDto();
        public AddressDto AddressInfo { get; set; } = new AddressDto();

        public List<string> Tags { get; set; } = new List<string>();
        public string? Notes { get; set; }
        public Guid? ImageId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
        public string Phones => PhoneInfo.PhoneItems.Count > 0 ? string.Join(", ", PhoneInfo.PhoneItems) : string.Empty;
        public string Emails => MailInfo.MailItems.Count > 0 ? string.Join(", ", MailInfo.MailItems) : string.Empty;

    }
}