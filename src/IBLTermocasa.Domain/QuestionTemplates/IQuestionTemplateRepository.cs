using IBLTermocasa.Types;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace IBLTermocasa.QuestionTemplates
{
    public interface IQuestionTemplateRepository : IRepository<QuestionTemplate, Guid>
    {
        Task<List<QuestionTemplate>> GetListAsync(
            string? filterText = null,
            string? code = null,
            string? questionText = null,
            AnswerType? answerType = null,
            string? choiceValue = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? code = null,
            string? questionText = null,
            AnswerType? answerType = null,
            string? choiceValue = null,
            CancellationToken cancellationToken = default);
    }
}