using IBLTermocasa.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using IBLTermocasa.MongoDB;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;
using MongoDB.Driver.Linq;
using MongoDB.Driver;

namespace IBLTermocasa.QuestionTemplates
{
    public class MongoQuestionTemplateRepository : MongoDbRepository<IBLTermocasaMongoDbContext, QuestionTemplate, Guid>, IQuestionTemplateRepository
    {
        public MongoQuestionTemplateRepository(IMongoDbContextProvider<IBLTermocasaMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public virtual async Task<List<QuestionTemplate>> GetListAsync(
            string? filterText = null,
            string? code = null,
            string? questionText = null,
            AnswerType? answerType = null,
            string? choiceValue = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, code, questionText, answerType, choiceValue);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? QuestionTemplateConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<QuestionTemplate>>()
                .PageBy<QuestionTemplate, IMongoQueryable<QuestionTemplate>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? code = null,
            string? questionText = null,
            AnswerType? answerType = null,
            string? choiceValue = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, code, questionText, answerType, choiceValue);
            return await query.As<IMongoQueryable<QuestionTemplate>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<QuestionTemplate> ApplyFilter(
            IQueryable<QuestionTemplate> query,
            string? filterText = null,
            string? code = null,
            string? questionText = null,
            AnswerType? answerType = null,
            string? choiceValue = null)
        {
            filterText = filterText?.ToLower();
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Code!.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase) || e.QuestionText!.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase) || e.ChoiceValue!.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase))
                    .WhereIf(!string.IsNullOrWhiteSpace(code), e => e.Code.Contains(code!, StringComparison.CurrentCultureIgnoreCase))
                    .WhereIf(!string.IsNullOrWhiteSpace(questionText), e => e.QuestionText.Contains(questionText!, StringComparison.CurrentCultureIgnoreCase))
                    .WhereIf(answerType.HasValue, e => e.AnswerType == answerType)
                    .WhereIf(!string.IsNullOrWhiteSpace(choiceValue), e => e.ChoiceValue != null && e.ChoiceValue.Contains(choiceValue!, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}