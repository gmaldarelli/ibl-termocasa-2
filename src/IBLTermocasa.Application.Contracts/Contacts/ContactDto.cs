using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Contacts
{
    public abstract class ContactDtoBase : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string? Title { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string? ConfidentialName { get; set; }
        public string? JobRole { get; set; }
        public DateTime BirthDate { get; set; }
        public string? MailInfo { get; set; }
        public string? PhoneInfo { get; set; }
        public string? SocialInfo { get; set; }
        public string? AddressInfo { get; set; }
        public string? Tag { get; set; }
        public string? Notes { get; set; }
        public string? ImageId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}