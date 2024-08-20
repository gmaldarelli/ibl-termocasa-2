using IBLTermocasa.Types;
using Volo.Abp.Application.Dtos;
using System;

namespace IBLTermocasa.Organizations
{
    public class GetOrganizationsInput : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? Code { get; set; }
        public string? Name { get; set; }
        public OrganizationType? OrganizationType { get; set; }
        public string? MailInfo { get; set; }
        public string? PhoneInfo { get; set; }
        public string? Tags { get; set; }
        public Guid? IndustryId { get; set; }
        public OrganizationType? OrganizationTypePreFilter { get; set; }
        public SourceType? SourceType { get; set; }

        public GetOrganizationsInput()
        {

        }
    }
}