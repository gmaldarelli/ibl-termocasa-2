using System;
using System.Collections.Generic;
using IBLTermocasa.Common;

namespace IBLTermocasa.Contacts
{
    public class ContactExcelDto
    {
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string? ConfidentialName { get; set; }
        public string? JobRole { get; set; }
        public PhoneInfoDto PhoneInfo { get; set; } = new PhoneInfoDto();
        public MailInfoDto MailInfo { get; set; } = new MailInfoDto();
        public SocialInfoDto SocialInfo { get; set; } = new SocialInfoDto();
        public AddressDto AddressInfo { get; set; } = new AddressDto();

        public List<string> Tags { get; set; } = new List<string>();

    }
}