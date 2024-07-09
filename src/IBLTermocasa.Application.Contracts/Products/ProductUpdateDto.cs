using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IBLTermocasa.Common;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Products
{
    public class ProductUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        public string Code { get; set; } = null!;
        public string? ParentPlaceHolder { get; set; } = null;
        public string PlaceHolder=>  PlaceHolderUtils.GetPlaceHolder(PlaceHolderType.PRODUCT, Code, ParentPlaceHolder);
        [Required]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsAssembled { get; set; }
        public bool IsInternal { get; set; }
        public List<SubProductDto> SubProducts { get; set; } = new();
        public List<ProductComponentDto> ProductComponents { get; set; } = new();
        public List<ProductQuestionTemplateDto> ProductQuestionTemplates { get; set; } = new();

        public string ConcurrencyStamp { get; set; } = null!;
    }
}