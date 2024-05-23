using IBLTermocasa.Components;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace IBLTermocasa.Products
{
    public abstract class ProductWithNavigationPropertiesDtoBase
    {
        public ProductDto Product { get; set; } = null!;

        public List<ComponentDto> Components { get; set; } = new List<ComponentDto>();

    }
}