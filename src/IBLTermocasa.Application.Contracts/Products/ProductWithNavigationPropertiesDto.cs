using IBLTermocasa.Components;
using IBLTermocasa.QuestionTemplates;
using System.Collections.Generic;
using IBLTermocasa.Subproducts;

namespace IBLTermocasa.Products
{
    public class ProductWithNavigationPropertiesDto
    {
        public ProductDto Product { get; set; } = null!;
        
        public List<ComponentDto> Components { get; set; } = new();
        public List<QuestionTemplateDto> QuestionTemplates { get; set; } = new();
        public List<SubproductDto> Subproducts { get; set; } = new();
    }
}