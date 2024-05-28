using IBLTermocasa.RequestForQuotations;
using Volo.Abp.Application.Dtos;
using System;
using IBLTermocasa.Common;

namespace IBLTermocasa.RequestForQuotations
{
    public class RequestForQuotationExcelDownloadDto
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; }

        public string? QuoteNumber { get; set; }
        public string? WorkSite { get; set; }
        public string? City { get; set; }
        public OrganizationProperty? OrganizationProperty { get; set; }
        public ContactProperty? ContactProperty { get; set; }
        public PhoneInfo? PhoneInfo { get; set; }
        public MailInfo? MailInfo { get; set; }
        public decimal? DiscountMin { get; set; }
        public decimal? DiscountMax { get; set; }
        public string? Description { get; set; }
        public Status? Status { get; set; }
        public Guid? AgentId { get; set; }
        public Guid? ContactId { get; set; }
        public Guid? OrganizationId { get; set; }

        public RequestForQuotationExcelDownloadDto()
        {

        }
    }
}