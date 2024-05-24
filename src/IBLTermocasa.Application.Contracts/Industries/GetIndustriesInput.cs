using Volo.Abp.Application.Dtos;
using System;

namespace IBLTermocasa.Industries
{
    public class GetIndustriesInput : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? Code { get; set; }
        public string? Description { get; set; }

        public GetIndustriesInput()
        {

        }
    }
}