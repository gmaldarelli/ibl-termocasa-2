using IBLTermocasa.RequestForQuotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace IBLTermocasa.RequestForQuotations
{
    public class RequestForQuotationCreateDto
    {
        [Required]
        public string QuoteNumber { get; set; } = null!;
        public string? WorkSite { get; set; }
        public string? City { get; set; }
        public string? OrganizationProperty { get; set; }
        public string? ContactProperty { get; set; }
        public string? PhoneInfo { get; set; }
        public string? MailInfo { get; set; }
        public decimal Discount { get; set; }
        public string? Description { get; set; }
        public Status Status { get; set; } = ((Status[])Enum.GetValues(typeof(Status)))[0];
        public Guid? AgentId { get; set; }
        public Guid? ContactId { get; set; }
        public Guid? OrganizationId { get; set; }
    }
}