using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using IBLTermocasa.Common;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Contacts
{
    public class ContactUpdateDto : IHasConcurrencyStamp
    {
        public string? Title { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Surname { get; set; } = null!;
        public string? ConfidentialName { get; set; }
        public string? JobRole { get; set; }
        public DateTime BirthDate { get; set; }
        
        public Guid? ImageId { get; set; }
        public PhoneInfoDto PhoneInfo { get; set; } = new PhoneInfoDto();
        public MailInfoDto MailInfo { get; set; } = new MailInfoDto();
        public SocialInfoDto SocialInfo { get; set; } = new SocialInfoDto();
        public AddressDto AddressInfo { get; set; } = new AddressDto();

        public List<string> Tags { get; set; } = new List<string>();

        public string? Notes { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}