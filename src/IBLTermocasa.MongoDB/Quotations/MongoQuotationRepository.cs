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

namespace IBLTermocasa.Quotations
{
    public class MongoQuotationRepository : MongoDbRepository<IBLTermocasaMongoDbContext, Quotation, Guid>, IQuotationRepository
    {
        public MongoQuotationRepository(IMongoDbContextProvider<IBLTermocasaMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public virtual async Task<List<Quotation>> GetListAsync(
            string? filterText = null,
            string? code = null,
            string? name = null,
            DateTime? sentDateMin = null,
            DateTime? sentDateMax = null,
            DateTime? quotationValidDateMin = null,
            DateTime? quotationValidDateMax = null,
            DateTime? confirmedDateMin = null,
            DateTime? confirmedDateMax = null,
            QuotationStatus? status = null,
            bool? depositRequired = null,
            double? depositRequiredValueMin = null,
            double? depositRequiredValueMax = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, code, name, sentDateMin, sentDateMax, quotationValidDateMin, quotationValidDateMax, confirmedDateMin, confirmedDateMax, status, depositRequired, depositRequiredValueMin, depositRequiredValueMax);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? QuotationConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<Quotation>>()
                .PageBy<Quotation, IMongoQueryable<Quotation>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? code = null,
            string? name = null,
            DateTime? sentDateMin = null,
            DateTime? sentDateMax = null,
            DateTime? quotationValidDateMin = null,
            DateTime? quotationValidDateMax = null,
            DateTime? confirmedDateMin = null,
            DateTime? confirmedDateMax = null,
            QuotationStatus? status = null,
            bool? depositRequired = null,
            double? depositRequiredValueMin = null,
            double? depositRequiredValueMax = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, code, name, sentDateMin, sentDateMax, quotationValidDateMin, quotationValidDateMax, confirmedDateMin, confirmedDateMax, status, depositRequired, depositRequiredValueMin, depositRequiredValueMax);
            return await query.As<IMongoQueryable<Quotation>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Quotation> ApplyFilter(
            IQueryable<Quotation> query,
            string? filterText = null,
            string? code = null,
            string? name = null,
            DateTime? sentDateMin = null,
            DateTime? sentDateMax = null,
            DateTime? quotationValidDateMin = null,
            DateTime? quotationValidDateMax = null,
            DateTime? confirmedDateMin = null,
            DateTime? confirmedDateMax = null,
            QuotationStatus? status = null,
            bool? depositRequired = null,
            double? depositRequiredValueMin = null,
            double? depositRequiredValueMax = null)
        {
            filterText = filterText?.ToLower();
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => 
                    (e.Code != null && e.Code.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase)) ||
                    (e.Name != null && e.Name.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase)))
                .WhereIf(!string.IsNullOrWhiteSpace(code), e => e.Code.Contains(code!, StringComparison.CurrentCultureIgnoreCase))
                .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name!, StringComparison.CurrentCultureIgnoreCase))
                .WhereIf(sentDateMin.HasValue, e => e.SentDate >= sentDateMin!.Value)
                .WhereIf(sentDateMax.HasValue, e => e.SentDate <= sentDateMax!.Value)
                .WhereIf(quotationValidDateMin.HasValue, e => e.QuotationValidDate >= quotationValidDateMin!.Value)
                .WhereIf(quotationValidDateMax.HasValue, e => e.QuotationValidDate <= quotationValidDateMax!.Value)
                .WhereIf(confirmedDateMin.HasValue, e => e.ConfirmedDate >= confirmedDateMin!.Value)
                .WhereIf(confirmedDateMax.HasValue, e => e.ConfirmedDate <= confirmedDateMax!.Value)
                .WhereIf(status.HasValue, e => e.Status == status)
                .WhereIf(depositRequired.HasValue, e => e.DepositRequired == depositRequired)
                .WhereIf(depositRequiredValueMin.HasValue, e => e.DepositRequiredValue >= depositRequiredValueMin!.Value)
                .WhereIf(depositRequiredValueMax.HasValue, e => e.DepositRequiredValue <= depositRequiredValueMax!.Value);
        }
                    
                
    }
}