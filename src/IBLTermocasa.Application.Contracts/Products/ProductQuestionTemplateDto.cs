using System;
using System.Collections.Generic;
using IBLTermocasa.QuestionTemplates;
using Volo.Abp.Application.Dtos;

namespace IBLTermocasa.Products
{
    public class ProductQuestionTemplateDto : EntityDto<Guid>
    {
        public int Order { get; set; }
        public bool Mandatory { get; set; }
        public QuestionTemplateDto QuestionTemplate { get; set; }
    }
}