using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Contacts
{
    public abstract class ContactUpdateDtoBase : IHasConcurrencyStamp
    {
        public string? Title { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
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

        public string ConcurrencyStamp { get; set; } = null!;
    }
}