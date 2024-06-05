using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IBLTermocasa.QuestionTemplates;
using Volo.Abp.Application.Dtos;

namespace IBLTermocasa.Products
{
    public class ProductQuestionTemplateDto : EntityDto<Guid>
    {
        public Guid QuestionTemplateId { get; set; }
        public int Order { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        public bool Mandatory { get; set; }
    }
}