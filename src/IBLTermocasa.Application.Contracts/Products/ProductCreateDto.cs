using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IBLTermocasa.Common;

namespace IBLTermocasa.Products
{
    public class ProductCreateDto
    {
        [Required]
        public string Code { get; set; } = null!;
        public string? ParentPlaceHolder { get; set; } = null;
        public string PlaceHolder =>  PlaceHolderUtils.GetPlaceHolder(PlaceHolderType.PRODUCT, Code, ParentPlaceHolder);
        [Required]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsAssembled { get; set; } = false;
        public bool IsInternal { get; set; } = false;
        public List<SubProductDto> SubProducts { get; set; } = new();
        public List<ProductComponentDto> ProductComponents { get; set; } = new();
        public List<ProductQuestionTemplateDto> ProductQuestionTemplates { get; set; } = new();
    }
}