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

namespace IBLTermocasa.ConsumptionEstimations
{
    public class MongoConsumptionEstimationRepository : MongoDbRepository<IBLTermocasaMongoDbContext, ConsumptionEstimation, Guid>, IConsumptionEstimationRepository
    {
        public MongoConsumptionEstimationRepository(IMongoDbContextProvider<IBLTermocasaMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        string? consumptionProduct = null,
            string? consumptionWork = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, consumptionProduct, consumptionWork);

            var ids = query.Select(x => x.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<ConsumptionEstimation>> GetListAsync(
            string? filterText = null,
            string? consumptionProduct = null,
            string? consumptionWork = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, consumptionProduct, consumptionWork);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ConsumptionEstimationConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<ConsumptionEstimation>>()
                .PageBy<ConsumptionEstimation, IMongoQueryable<ConsumptionEstimation>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? consumptionProduct = null,
            string? consumptionWork = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, consumptionProduct, consumptionWork);
            return await query.As<IMongoQueryable<ConsumptionEstimation>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<ConsumptionEstimation> ApplyFilter(
            IQueryable<ConsumptionEstimation> query,
            string? filterText = null,
            string? consumptionProduct = null,
            string? consumptionWork = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.ConsumptionProduct!.Contains(filterText!) || e.ConsumptionWork!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(consumptionProduct), e => e.ConsumptionProduct.Contains(consumptionProduct))
                    .WhereIf(!string.IsNullOrWhiteSpace(consumptionWork), e => e.ConsumptionWork.Contains(consumptionWork));
        }
    }
}