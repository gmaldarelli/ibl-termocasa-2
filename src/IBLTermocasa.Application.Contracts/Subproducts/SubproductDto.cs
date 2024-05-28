using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;

namespace IBLTermocasa.Subproducts
{
    public class SubproductDto : EntityDto<Guid>
    {
        public Guid ProductId { get; set; }
        public int Order { get; set; }
        public string Name { get; set; } = null!;
        public bool IsSingleProduct { get; set; }
        public bool Mandatory { get; set; }
        public Guid? SingleProductId { get; set; }

    }
}