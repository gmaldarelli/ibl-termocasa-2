using Volo.Abp.Application.Dtos;
using System;

namespace IBLTermocasa.Subproducts
{
    public class GetSubproductListInput : PagedAndSortedResultRequestDto
    {
        public Guid ProductId { get; set; }
    }
}