using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IBLTermocasa.Common;
using IBLTermocasa.QuestionTemplates;
using Volo.Abp.Application.Dtos;

namespace IBLTermocasa.Products
{
    public class ProductQuestionTemplateDto : EntityDto<Guid>
    {
        [Required]
        public Guid QuestionTemplateId { get; set; }
        [Required]
        public int Order { get; set; }
        [Required]
        public  Guid ParentId { get;  set; }
        public string? ParentPlaceHolder { get; set; } = null;
        public string PlaceHolder=>  PlaceHolderUtils.GetPlaceHolder(PlaceHolderType.PRODUCT_QUESTION_TEMPLATE, Code, ParentPlaceHolder);
        [Required]
        public virtual string Code { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        public bool Mandatory { get; set; }        
        public string? ValidationFormula { get; set; }
    }
}