using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using IBLTermocasa.MongoDB;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;
using MongoDB.Driver.Linq;
using MongoDB.Driver;

namespace IBLTermocasa.Components
{
    public class MongoComponentRepository : MongoDbRepository<IBLTermocasaMongoDbContext, Component, Guid>, IComponentRepository
    {
        public MongoComponentRepository(IMongoDbContextProvider<IBLTermocasaMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public virtual async Task<List<Component>> GetListAsync(
            string? filterText = null,
            string? code = null,
            string? name = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, code, name);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ComponentConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<Component>>()
                .PageBy<Component, IMongoQueryable<Component>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? code = null,
            string? name = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, code, name);
            return await query.As<IMongoQueryable<Component>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Component> ApplyFilter(
            IQueryable<Component> query,
            string? filterText = null,
            string? code = null,
            string? name = null)
        {
            filterText = filterText?.ToLower();
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Name.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase) || e.Code!.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase))
                    .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name!, StringComparison.CurrentCultureIgnoreCase))
                .WhereIf(!string.IsNullOrWhiteSpace(code), e => e.Code.Contains(code!, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}