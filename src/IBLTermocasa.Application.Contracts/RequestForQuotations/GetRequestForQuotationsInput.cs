using IBLTermocasa.RequestForQuotations;
using Volo.Abp.Application.Dtos;
using System;
using IBLTermocasa.Common;
using IBLTermocasa.Types;

namespace IBLTermocasa.RequestForQuotations
{
    public class GetRequestForQuotationsInput : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? QuoteNumber { get; set; }
        public string? WorkSite { get; set; }
        public string? City { get; set; }
        public AgentProperty? AgentProperty { get; set; }
        public OrganizationProperty? OrganizationProperty { get; set; }
        public ContactProperty? ContactProperty { get; set; }
        public PhoneInfo? PhoneInfo { get; set; }
        public MailInfo? MailInfo { get; set; }
        public decimal? DiscountMin { get; set; }
        public decimal? DiscountMax { get; set; }
        public string? Description { get; set; }
        public RfqStatus? Status { get; set; }

        public GetRequestForQuotationsInput()
        {

        }
    }
}