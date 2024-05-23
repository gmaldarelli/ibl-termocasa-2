using IBLTermocasa.Products;
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

namespace IBLTermocasa.Subproducts
{
    public abstract class MongoSubproductRepositoryBase : MongoDbRepository<IBLTermocasaMongoDbContext, Subproduct, Guid>
    {
        public MongoSubproductRepositoryBase(IMongoDbContextProvider<IBLTermocasaMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public virtual async Task<List<Subproduct>> GetListByProductIdAsync(
                   Guid productId,
                   string? sorting = null,
                   int maxResultCount = int.MaxValue,
                   int skipCount = 0,
                   CancellationToken cancellationToken = default)
        {
            IQueryable<Subproduct> query = (await GetMongoQueryableAsync(cancellationToken)).Where(x => x.ProductId == productId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? SubproductConsts.GetDefaultSorting(false) : sorting);

            return await query
                .As<IMongoQueryable<Subproduct>>()
                .PageBy<Subproduct, IMongoQueryable<Subproduct>>(skipCount, maxResultCount)
                .ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync(cancellationToken)).Where(x => x.ProductId == productId).CountAsync(cancellationToken);
        }

        public virtual async Task<List<SubproductWithNavigationProperties>> GetListWithNavigationPropertiesByProductIdAsync(
    Guid productId,
    string? sorting = null,
    int maxResultCount = int.MaxValue,
    int skipCount = 0,
    CancellationToken cancellationToken = default)
        {
            var query = (await GetMongoQueryableAsync(cancellationToken)).Where(x => x.ProductId == productId);
            var subproducts = await query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? SubproductConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                .As<IMongoQueryable<Subproduct>>()
                .PageBy<Subproduct, IMongoQueryable<Subproduct>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            var dbContext = await GetDbContextAsync(cancellationToken);
            return subproducts.Select(s => new SubproductWithNavigationProperties
            {
                Subproduct = s,
                Product = ApplyDataFilters<IMongoQueryable<Product>, Product>(dbContext.Collection<Product>().AsQueryable()).FirstOrDefault(e => e.Id == s.SingleProductId),

            }).ToList();
        }

        public virtual async Task<SubproductWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var subproduct = await (await GetMongoQueryableAsync(cancellationToken))
                .FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));

            var product = await (await GetMongoQueryableAsync<Product>(cancellationToken)).FirstOrDefaultAsync(e => e.Id == subproduct.SingleProductId, cancellationToken: cancellationToken);

            return new SubproductWithNavigationProperties
            {
                Subproduct = subproduct,
                Product = product,

            };
        }

        public virtual async Task<List<SubproductWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            int? orderMin = null,
            int? orderMax = null,
            string? name = null,
            bool? isSingleProduct = null,
            bool? mandatory = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, orderMin, orderMax, name, isSingleProduct, mandatory);
            var subproducts = await query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? SubproductConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                .As<IMongoQueryable<Subproduct>>()
                .PageBy<Subproduct, IMongoQueryable<Subproduct>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            var dbContext = await GetDbContextAsync(cancellationToken);
            return subproducts.Select(s => new SubproductWithNavigationProperties
            {
                Subproduct = s,
                Product = ApplyDataFilters<IMongoQueryable<Product>, Product>(dbContext.Collection<Product>().AsQueryable()).FirstOrDefault(e => e.Id == s.SingleProductId),

            }).ToList();
        }

        public virtual async Task<List<Subproduct>> GetListAsync(
            string? filterText = null,
            int? orderMin = null,
            int? orderMax = null,
            string? name = null,
            bool? isSingleProduct = null,
            bool? mandatory = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, orderMin, orderMax, name, isSingleProduct, mandatory);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? SubproductConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<Subproduct>>()
                .PageBy<Subproduct, IMongoQueryable<Subproduct>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            int? orderMin = null,
            int? orderMax = null,
            string? name = null,
            bool? isSingleProduct = null,
            bool? mandatory = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, orderMin, orderMax, name, isSingleProduct, mandatory);
            return await query.As<IMongoQueryable<Subproduct>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Subproduct> ApplyFilter(
            IQueryable<Subproduct> query,
            string? filterText = null,
            int? orderMin = null,
            int? orderMax = null,
            string? name = null,
            bool? isSingleProduct = null,
            bool? mandatory = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Name!.Contains(filterText!))
                    .WhereIf(orderMin.HasValue, e => e.Order >= orderMin!.Value)
                    .WhereIf(orderMax.HasValue, e => e.Order <= orderMax!.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name))
                    .WhereIf(isSingleProduct.HasValue, e => e.IsSingleProduct == isSingleProduct)
                    .WhereIf(mandatory.HasValue, e => e.Mandatory == mandatory);
        }
    }
}