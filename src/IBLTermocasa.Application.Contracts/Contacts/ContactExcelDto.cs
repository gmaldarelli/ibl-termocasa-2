using System;

namespace IBLTermocasa.Contacts
{
    public abstract class ContactExcelDtoBase
    {
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string? ConfidentialName { get; set; }
        public string? JobRole { get; set; }
        public string? MailInfo { get; set; }
        public string? PhoneInfo { get; set; }
        public string? AddressInfo { get; set; }
        public string? Tag { get; set; }
    }
}