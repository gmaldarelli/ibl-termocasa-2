using Volo.Abp.Application.Dtos;
using System;

namespace IBLTermocasa.Subproducts
{
    public abstract class GetSubproductsInputBase : PagedAndSortedResultRequestDto
    {
        public Guid? ProductId { get; set; }

        public string? FilterText { get; set; }

        public int? OrderMin { get; set; }
        public int? OrderMax { get; set; }
        public string? Name { get; set; }
        public bool? IsSingleProduct { get; set; }
        public bool? Mandatory { get; set; }

        public GetSubproductsInputBase()
        {

        }
    }
}