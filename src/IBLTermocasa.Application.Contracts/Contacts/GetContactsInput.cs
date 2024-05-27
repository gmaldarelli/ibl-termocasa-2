using Volo.Abp.Application.Dtos;
using System;

namespace IBLTermocasa.Contacts
{
    public class GetContactsInput : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? Title { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? ConfidentialName { get; set; }
        public string? JobRole { get; set; }
        public string? MailInfo { get; set; }
        public string? PhoneInfo { get; set; }
        public string? AddressInfo { get; set; }
        public string? Tag { get; set; }

        public GetContactsInput()
        {

        }
    }
}