using IBLTermocasa.Products;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace IBLTermocasa.Subproducts
{
    public class SubproductWithNavigationPropertiesDto
    {
        public SubproductDto Subproduct { get; set; } = null!;

        public ProductDto Product { get; set; } = null!;

    }
}