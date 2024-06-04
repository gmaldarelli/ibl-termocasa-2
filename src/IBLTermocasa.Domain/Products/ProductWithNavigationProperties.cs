using IBLTermocasa.Components;
using IBLTermocasa.QuestionTemplates;

using System;
using System.Collections.Generic;

namespace IBLTermocasa.Products
{
    public  class ProductWithNavigationProperties
    {
        public Product Product { get; set; } = null!;
        public List<Component> Components { get; set; } = null!;
        public List<QuestionTemplate> QuestionTemplates { get; set; } = null!;
        public List<Product> Products { get; set; } = null!;
        
    }
}