using IBLTermocasa.Components;
using IBLTermocasa.QuestionTemplates;

using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using IBLTermocasa.Subproducts;

namespace IBLTermocasa.Products
{
    public  class ProductWithNavigationProperties
    {
        public Product Product { get; set; } = null!;
        public List<Component> Components { get; set; } = null!;
        public List<QuestionTemplate> QuestionTemplates { get; set; } = null!;
        public List<Subproduct> Subproducts { get; set; } = null!;
        
    }
}