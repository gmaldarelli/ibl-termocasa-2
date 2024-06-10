using IBLTermocasa.Products;

using System;
using System.Collections.Generic;

namespace IBLTermocasa.Catalogs
{
    public  class CatalogWithNavigationProperties
    {
        public Catalog Catalog { get; set; } = null!;
        public List<Product> Products { get; set; } = null!;
        
    }
}