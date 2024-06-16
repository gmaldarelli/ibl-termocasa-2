using System;
using System.Collections.Generic;
using IBLTermocasa.Common;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.RequestForQuotations
{
    public class RequestForQuotationDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string QuoteNumber { get; set; } = null!;
        public string? WorkSite { get; set; }
        public string? City { get; set; }
        public AgentPropertyDto? AgentProperty { get; set; } = new();
        public OrganizationPropertyDto? OrganizationProperty { get; set; }  = new();
        public ContactPropertyDto? ContactProperty { get; set; } = new();
        public PhoneInfoDto? PhoneInfo { get; set; } = new();
        public MailInfoDto? MailInfo { get; set; } = new();
        public decimal? Discount { get; set; }
        public string? Description { get; set; }
        public RfqStatus Status { get; set; }

        public List<RequestForQuotationItemDto>? RequestForQuotationItems { get; set; } = new();
        
        public DateTime? DateDocument { get; set; }
        
        public string Emails => MailInfo != null && MailInfo.MailItems.Count > 0 ? string.Join(", ", MailInfo.MailItems) : string.Empty;
        public string Phones => PhoneInfo != null && PhoneInfo.PhoneItems.Count > 0 ? string.Join(", ", PhoneInfo.PhoneItems) : string.Empty;
        public string ConcurrencyStamp { get; set; } = null!;
        
    }
}