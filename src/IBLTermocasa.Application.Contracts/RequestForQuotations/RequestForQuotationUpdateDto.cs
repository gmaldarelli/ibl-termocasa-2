using IBLTermocasa.RequestForQuotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using IBLTermocasa.Common;
using IBLTermocasa.Types;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.RequestForQuotations
{
    public class RequestForQuotationUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        public string QuoteNumber { get; set; } = null!;
        public string? WorkSite { get; set; }
        public string? City { get; set; }
        public AgentPropertyDto? AgentProperty { get; set; }
        public OrganizationPropertyDto? OrganizationProperty { get; set; }
        public ContactPropertyDto? ContactProperty { get; set; }
        public PhoneInfoDto? PhoneInfo { get; set; }
        public MailInfoDto? MailInfo { get; set; }
        public decimal? Discount { get; set; }
        public string? Description { get; set; }
        public RfqStatus Status { get; set; }
        public List<RequestForQuotationItemDto>? RequestForQuotationItems { get; set; } = new();
        public DateTime? DateDocument { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}