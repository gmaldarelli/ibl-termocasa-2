using Volo.Abp.Application.Dtos;
using System;

namespace IBLTermocasa.ComponentItems
{
    public abstract class GetComponentItemsInputBase : PagedAndSortedResultRequestDto
    {
        public Guid? ComponentId { get; set; }

        public string? FilterText { get; set; }

        public bool? IsDefault { get; set; }
        public Guid? MaterialId { get; set; }

        public GetComponentItemsInputBase()
        {

        }
    }
}