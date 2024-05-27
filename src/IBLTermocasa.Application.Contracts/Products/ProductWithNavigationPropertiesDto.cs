using IBLTermocasa.Components;
using IBLTermocasa.QuestionTemplates;

using System;
using Volo.Abp.Application.Dtos;
using System.Collections.Generic;

namespace IBLTermocasa.Products
{
    public class ProductWithNavigationPropertiesDto
    {
        public ProductDto Product { get; set; } = null!;

        public List<ComponentDto> Components { get; set; } = new List<ComponentDto>();
        public List<QuestionTemplateDto> QuestionTemplates { get; set; } = new List<QuestionTemplateDto>();

    }
}