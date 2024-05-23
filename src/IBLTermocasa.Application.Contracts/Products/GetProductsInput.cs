using Volo.Abp.Application.Dtos;
using System;

namespace IBLTermocasa.Products
{
    public abstract class GetProductsInputBase : PagedAndSortedResultRequestDto
    {

        public string? FilterText { get; set; }

        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool? IsAssembled { get; set; }
        public bool? IsInternal { get; set; }

        public GetProductsInputBase()
        {

        }
    }
}