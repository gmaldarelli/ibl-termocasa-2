using IBLTermocasa.Types;
using System;

namespace IBLTermocasa.Organizations
{
    public class OrganizationExcelDto
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public OrganizationType OrganizationType { get; set; }
        public string? MailInfo { get; set; }
        public string? PhoneInfo { get; set; }
        public string? Tags { get; set; }
    }
}