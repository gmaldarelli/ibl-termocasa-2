using System;
using System.Collections.Generic;
using IBLTermocasa.Components;
using IBLTermocasa.QuestionTemplates;
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

        public List<SubProductDto> SubProducts { get; set; } = new();
        public List<ProductComponentDto> ProductComponents { get; set; } = new();
        public List<ProductQuestionTemplateDto> ProductQuestionTemplates { get; set; } = new();

    }
}