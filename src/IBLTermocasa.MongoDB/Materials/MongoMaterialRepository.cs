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

namespace IBLTermocasa.Materials
{
    public class MongoMaterialRepository : MongoDbRepository<IBLTermocasaMongoDbContext, Material, Guid>, IMaterialRepository
    {
        public MongoMaterialRepository(IMongoDbContextProvider<IBLTermocasaMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public virtual async Task<List<Material>> GetListAsync(
            string? filterText = null,
            string? code = null,
            string? name = null,
            SourceType? sourceType = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, code, name, sourceType);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? MaterialConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<Material>>()
                .PageBy<Material, IMongoQueryable<Material>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? code = null,
            string? name = null,
            SourceType? sourceType = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, code, name, sourceType);
            return await query.As<IMongoQueryable<Material>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Material> ApplyFilter(
            IQueryable<Material> query,
            string? filterText = null,
            string? code = null,
            string? name = null,
            SourceType? sourceType = null)
        {
            filterText = filterText?.ToLower();
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Code!.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase) || e.Name!.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase))
                    .WhereIf(!string.IsNullOrWhiteSpace(code), e => e.Code.Contains(code!, StringComparison.CurrentCultureIgnoreCase))
                    .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name!, StringComparison.CurrentCultureIgnoreCase))
                    .WhereIf(sourceType.HasValue, e => e.SourceType == sourceType);
        }
    }
}