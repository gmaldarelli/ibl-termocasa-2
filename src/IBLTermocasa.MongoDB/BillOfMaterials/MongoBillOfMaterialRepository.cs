using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using IBLTermocasa.Common;
using IBLTermocasa.MongoDB;
using IBLTermocasa.Types;
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
            string? bomNumber = null,
            RequestForQuotationProperty? requestForQuotationProperty = null,
            BomStatusType? status = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, bomNumber, requestForQuotationProperty, status);

            var ids = query.Select(x => x.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<BillOfMaterial>> GetListAsync(
            string? filterText = null,
            string? bomNumber = null,
            RequestForQuotationProperty? requestForQuotationProperty = null,
            BomStatusType? status = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, bomNumber, requestForQuotationProperty, status);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? BillOfMaterialConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<BillOfMaterial>>()
                .PageBy<BillOfMaterial, IMongoQueryable<BillOfMaterial>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? bomNumber = null,
            RequestForQuotationProperty? requestForQuotationProperty = null,
            BomStatusType? status = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, bomNumber, requestForQuotationProperty, status);
            return await query.As<IMongoQueryable<BillOfMaterial>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<BillOfMaterial> ApplyFilter(
            IQueryable<BillOfMaterial> query,
            string? filterText = null,
            string? bomNumber = null,
            RequestForQuotationProperty? requestForQuotationProperty = null,
            BomStatusType? status = null)
        {
            filterText = filterText?.ToLower();
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.BomNumber.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase) || 
                                                                      e.RequestForQuotationProperty.Name != null && e.RequestForQuotationProperty.Name.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase) ||
                                                                      e.RequestForQuotationProperty.OrganizationName != null && e.RequestForQuotationProperty.OrganizationName.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase))
                .WhereIf(!string.IsNullOrWhiteSpace(bomNumber), e => bomNumber != null && e.BomNumber.Contains(bomNumber!, StringComparison.CurrentCultureIgnoreCase))
                .WhereIf(!string.IsNullOrWhiteSpace(requestForQuotationProperty?.Name), e => requestForQuotationProperty != null && requestForQuotationProperty.Name != null && e.RequestForQuotationProperty.Name!.Contains(requestForQuotationProperty.Name, StringComparison.CurrentCultureIgnoreCase))
                .WhereIf(!string.IsNullOrWhiteSpace(requestForQuotationProperty?.OrganizationName), e => requestForQuotationProperty != null && requestForQuotationProperty.OrganizationName != null && e.RequestForQuotationProperty.OrganizationName!.Contains(requestForQuotationProperty.OrganizationName, StringComparison.CurrentCultureIgnoreCase))
                .WhereIf(status!= null, e => e.Status == status);
        }
    }
}