using IBLTermocasa.RequestForQuotations;
using Volo.Abp.Application.Dtos;
using System;

namespace IBLTermocasa.RequestForQuotations
{
    public class GetRequestForQuotationsInput : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? QuoteNumber { get; set; }
        public string? WorkSite { get; set; }
        public string? City { get; set; }
        public string? OrganizationProperty { get; set; }
        public string? ContactProperty { get; set; }
        public string? PhoneInfo { get; set; }
        public string? MailInfo { get; set; }
        public decimal? DiscountMin { get; set; }
        public decimal? DiscountMax { get; set; }
        public string? Description { get; set; }
        public Status? Status { get; set; }
        public Guid? AgentId { get; set; }
        public Guid? ContactId { get; set; }
        public Guid? OrganizationId { get; set; }

        public GetRequestForQuotationsInput()
        {

        }
    }
}