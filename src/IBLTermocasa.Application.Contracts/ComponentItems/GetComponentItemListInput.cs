using Volo.Abp.Application.Dtos;
using System;

namespace IBLTermocasa.ComponentItems
{
    public class GetComponentItemListInput : PagedAndSortedResultRequestDto
    {
        public Guid ComponentId { get; set; }
    }
}