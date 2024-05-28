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
        public OrganizationPropertyDto? OrganizationPropertyDto { get; set; }  = new();
        public ContactPropertyDto? ContactPropertyDto { get; set; } = new();
        public PhoneInfoDto? PhoneInfo { get; set; } = new();
        public MailInfoDto? MailInfo { get; set; } = new();
        public decimal? Discount { get; set; }
        public string? Description { get; set; }
        public Status Status { get; set; }
        public Guid? AgentId { get; set; }
        public Guid? ContactId { get; set; }
        public Guid? OrganizationId { get; set; }

        public List<RequestForQuotationItemDto>? RequestForQuotationItems { get; set; } = new();
        
        public string Emails => MailInfo != null && MailInfo.MailItems.Count > 0 ? string.Join(", ", MailInfo.MailItems) : string.Empty;
        public string Phones => PhoneInfo != null && PhoneInfo.PhoneItems.Count > 0 ? string.Join(", ", PhoneInfo.PhoneItems) : string.Empty;
        public string ConcurrencyStamp { get; set; } = null!;
        
        public DateTime? CreationTime { get; set; } = DateTime.Now;

    }
}