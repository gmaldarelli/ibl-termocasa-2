using IBLTermocasa.Types;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace IBLTermocasa.QuestionTemplates
{
    public class QuestionTemplate : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Code { get; set; }

        [NotNull]
        public virtual string QuestionText { get; set; }

        public virtual AnswerType AnswerType { get; set; }

        [CanBeNull]
        public virtual string? ChoiceValue { get; set; }

        protected QuestionTemplate()
        {

        }

        public QuestionTemplate(Guid id, string code, string questionText, AnswerType answerType, string? choiceValue = null)
        {

            Id = id;
            Check.NotNull(code, nameof(code));
            Check.NotNull(questionText, nameof(questionText));
            Code = code;
            QuestionText = questionText;
            AnswerType = answerType;
            ChoiceValue = choiceValue;
        }

    }
}