using IBLTermocasa.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace IBLTermocasa.QuestionTemplates
{
    public class QuestionTemplateManager : DomainService
    {
        protected IQuestionTemplateRepository _questionTemplateRepository;

        public QuestionTemplateManager(IQuestionTemplateRepository questionTemplateRepository)
        {
            _questionTemplateRepository = questionTemplateRepository;
        }

        public virtual async Task<QuestionTemplate> CreateAsync(
        string code, string questionText, AnswerType answerType, string? choiceValue = null)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            Check.NotNullOrWhiteSpace(questionText, nameof(questionText));
            Check.NotNull(answerType, nameof(answerType));

            var questionTemplate = new QuestionTemplate(
             GuidGenerator.Create(),
             code, questionText, answerType, choiceValue
             );

            return await _questionTemplateRepository.InsertAsync(questionTemplate);
        }

        public virtual async Task<QuestionTemplate> UpdateAsync(
            Guid id,
            string code, string questionText, AnswerType answerType, string? choiceValue = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            Check.NotNullOrWhiteSpace(questionText, nameof(questionText));
            Check.NotNull(answerType, nameof(answerType));

            var questionTemplate = await _questionTemplateRepository.GetAsync(id);

            questionTemplate.Code = code;
            questionTemplate.QuestionText = questionText;
            questionTemplate.AnswerType = answerType;
            questionTemplate.ChoiceValue = choiceValue;

            questionTemplate.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _questionTemplateRepository.UpdateAsync(questionTemplate);
        }

    }
}