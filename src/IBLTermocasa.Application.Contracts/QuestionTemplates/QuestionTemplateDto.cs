using IBLTermocasa.Types;
using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.QuestionTemplates
{
    public class QuestionTemplateDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Code { get; set; } = null!;
        public string QuestionText { get; set; } = null!;
        public AnswerType AnswerType { get; set; }
        public string? ChoiceValue { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

        public List<string> ChoiceValues => string.IsNullOrEmpty(ChoiceValue) ? new List<string>() : new List<string>(ChoiceValue.Split(','));
    }
}