using System;
using System.Collections.Generic;
using IBLTermocasa.Subproducts;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Products
{
    public class ProductDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsAssembled { get; set; }
        public bool IsInternal { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

        public List<SubproductWithNavigationPropertiesDto> Subproducts { get; set; } = new();
        public List<ProductComponentDto> Components { get; set; } = new();
        public List<ProductQuestionTemplateDto> QuestionTemplates { get; set; } = new();
    }
}