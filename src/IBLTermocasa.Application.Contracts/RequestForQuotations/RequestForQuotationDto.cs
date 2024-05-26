using IBLTermocasa.RequestForQuotations;
using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.RequestForQuotations
{
    public class RequestForQuotationDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
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
        public Guid? AgentId { get; set; }
        public Guid? ContactId { get; set; }
        public Guid? OrganizationId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

    }
}