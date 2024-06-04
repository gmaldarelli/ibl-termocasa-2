using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;

namespace IBLTermocasa.Products
{
    public class SubProductDto : EntityDto<Guid>
    { 
        public int Order { get; set; }
        public string Name { get; set; } = null!;
        public bool IsSingleProduct { get; set; }
        public bool Mandatory { get; set; }
        public Guid ParentId { get; set; }
        public List<ProductDto> Products { get; set; } = new();

    }
}