using IBLTermocasa.Types;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.QuestionTemplates
{
    public class QuestionTemplateUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        public string Code { get; set; } = null!;
        [Required]
        public string QuestionText { get; set; } = null!;
        public AnswerType AnswerType { get; set; }
        public string? ChoiceValue { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}