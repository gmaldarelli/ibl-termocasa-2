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
    public class MongoBillOfMaterialRepository : MongoDbRepository<IBLTermocasaMongoDbContext, BillOfMaterial, Guid>, IBillOfMaterialRepository
    {
        public MongoBillOfMaterialRepository(IMongoDbContextProvider<IBLTermocasaMongoDbContext> dbContextProvider)
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

        public virtual async Task<List<BillOfMaterial>> GetListAsync(
            string? filterText = null,
            string? name = null,
            RequestForQuotationProperty? requestForQuotationProperty = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, name, requestForQuotationProperty);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? BillOfMaterialConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<BillOfMaterial>>()
                .PageBy<BillOfMaterial, IMongoQueryable<BillOfMaterial>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? name = null,
            RequestForQuotationProperty? requestForQuotationProperty = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, name, requestForQuotationProperty);
            return await query.As<IMongoQueryable<BillOfMaterial>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<BillOfMaterial> ApplyFilter(
            IQueryable<BillOfMaterial> query,
            string? filterText = null,
            string? name = null,
            RequestForQuotationProperty? requestForQuotationProperty = null)
        {
            if(!filterText.IsNullOrWhiteSpace())
            {
                query = query.Where(e => e.BomNumber.Contains(filterText));
                query = query.Where(e => e.RequestForQuotationProperty.Name != null && e.RequestForQuotationProperty.Name.Contains(filterText));
            }
            if(!name.IsNullOrWhiteSpace())
            {
                query = query.Where(e => e.BomNumber.Contains(name));
                query = query.Where(e => e.RequestForQuotationProperty.Name != null && e.RequestForQuotationProperty.Name.Contains(name));
            }
            if(requestForQuotationProperty is { Name: not null })
            {
                query = query.Where(e => requestForQuotationProperty.Name != null && e.RequestForQuotationProperty.Name != null && e.RequestForQuotationProperty.Name.Contains(requestForQuotationProperty.Name));
            }
            
            return query;
        }
    }
}