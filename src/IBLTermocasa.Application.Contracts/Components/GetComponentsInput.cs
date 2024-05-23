using Volo.Abp.Application.Dtos;
using System;

namespace IBLTermocasa.Components
{
    public abstract class GetComponentsInputBase : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? Name { get; set; }

        public GetComponentsInputBase()
        {

        }
    }
}