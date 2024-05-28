using IBLTermocasa.Products;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace IBLTermocasa.Catalogs
{
    public class CatalogWithNavigationPropertiesDto
    {
        public CatalogDto Catalog { get; set; } = null!;

        public List<ProductDto> Products { get; set; } = new();

    }
}