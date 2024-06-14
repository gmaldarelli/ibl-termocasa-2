using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using IBLTermocasa.Common;
using IBLTermocasa.MongoDB;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;
using MongoDB.Driver.Linq;
using MongoDB.Driver;

namespace IBLTermocasa.BillOfMaterials
{
    public class MongoBillOFMaterialRepository : MongoDbRepository<IBLTermocasaMongoDbContext, BillOFMaterial, Guid>, IBillOFMaterialRepository
    {
        public MongoBillOFMaterialRepository(IMongoDbContextProvider<IBLTermocasaMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
            string? name = null,
            RequestForQuotationProperty? requestForQuotationProperty = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, name, requestForQuotationProperty);

            var ids = query.Select(x => x.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<BillOFMaterial>> GetListAsync(
            string? filterText = null,
            string? name = null,
            RequestForQuotationProperty? requestForQuotationProperty = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, name, requestForQuotationProperty);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? BillOFMaterialConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<BillOFMaterial>>()
                .PageBy<BillOFMaterial, IMongoQueryable<BillOFMaterial>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? name = null,
            RequestForQuotationProperty? requestForQuotationProperty = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, name, requestForQuotationProperty);
            return await query.As<IMongoQueryable<BillOFMaterial>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<BillOFMaterial> ApplyFilter(
            IQueryable<BillOFMaterial> query,
            string? filterText = null,
            string? name = null,
            RequestForQuotationProperty? requestForQuotationProperty = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Name.Contains(filterText))
                .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name))
                .WhereIf(requestForQuotationProperty != null,
                    e => e.RequestForQuotationProperty.Name.Contains(requestForQuotationProperty.Name));
        }
    }
}