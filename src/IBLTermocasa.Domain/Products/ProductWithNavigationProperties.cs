using IBLTermocasa.Components;

using System;
using System.Collections.Generic;

namespace IBLTermocasa.Products
{
    public abstract class ProductWithNavigationPropertiesBase
    {
        public Product Product { get; set; } = null!;

        

        public List<Component> Components { get; set; } = null!;
        
    }
}