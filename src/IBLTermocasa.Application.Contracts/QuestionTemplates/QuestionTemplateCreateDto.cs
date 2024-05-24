using IBLTermocasa.Types;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace IBLTermocasa.QuestionTemplates
{
    public class QuestionTemplateCreateDto
    {
        [Required]
        public string Code { get; set; } = null!;
        [Required]
        public string QuestionText { get; set; } = null!;
        public AnswerType AnswerType { get; set; } = ((AnswerType[])Enum.GetValues(typeof(AnswerType)))[0];
        public string? ChoiceValue { get; set; }
    }
}