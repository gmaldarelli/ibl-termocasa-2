using Volo.Abp.Application.Dtos;
using System;

namespace IBLTermocasa.Components
{
    public class GetComponentsInput : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? Name { get; set; }

        public GetComponentsInput()
        {

        }
    }
}