using IBLTermocasa.RequestForQuotations;
using System;

namespace IBLTermocasa.RequestForQuotations
{
    public class RequestForQuotationExcelDto
    {
        public string QuoteNumber { get; set; } = null!;
        public string? WorkSite { get; set; }
        public string? City { get; set; }
        public string? OrganizationProperty { get; set; }
        public string? ContactProperty { get; set; }
        public string? PhoneInfo { get; set; }
        public string? MailInfo { get; set; }
        public decimal Discount { get; set; }
        public string? Description { get; set; }
        public Status Status { get; set; }
    }
}