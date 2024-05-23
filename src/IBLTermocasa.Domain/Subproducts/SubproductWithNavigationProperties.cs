using IBLTermocasa.Products;

using System;
using System.Collections.Generic;

namespace IBLTermocasa.Subproducts
{
    public abstract class SubproductWithNavigationPropertiesBase
    {
        public Subproduct Subproduct { get; set; } = null!;

        public Product Product { get; set; } = null!;
        

        
    }
}