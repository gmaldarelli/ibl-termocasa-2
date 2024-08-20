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

namespace IBLTermocasa.Catalogs
{
    public class MongoCatalogRepository : MongoDbRepository<IBLTermocasaMongoDbContext, Catalog, Guid>, ICatalogRepository
    {
        public MongoCatalogRepository(IMongoDbContextProvider<IBLTermocasaMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public virtual async Task<CatalogWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var catalog = await (await GetMongoQueryableAsync(cancellationToken))
                .FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));

            var productIds = catalog.Products.Select(x => x.ProductId).ToList();
            var products = await (await GetMongoQueryableAsync<Product>(cancellationToken)).Where(e => productIds.Contains(e.Id)).ToListAsync(cancellationToken: cancellationToken);

            return new CatalogWithNavigationProperties
            {
                Catalog = catalog,
                Products = products,

            };
        }
        
        public virtual async Task<List<CatalogWithNavigationProperties>> GetListCatalogWithProducts(
            string? filterText = null,
            string? name = null,
            DateTime? fromMin = null,
            DateTime? fromMax = null,
            DateTime? toMin = null,
            DateTime? toMax = null,
            string? description = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, name, fromMin, fromMax, toMin, toMax, description);
            var catalogs = await query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? CatalogConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                .As<IMongoQueryable<Catalog>>()
                .PageBy<Catalog, IMongoQueryable<Catalog>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            var listCatalogs = new List<CatalogWithNavigationProperties>();
            foreach (var catalog in catalogs)
            {
                var productIds = catalog.Products.Select(x => x.ProductId).ToList();
                var products = await (await GetMongoQueryableAsync<Product>(cancellationToken)).Where(e => productIds.Contains(e.Id)).ToListAsync(cancellationToken: cancellationToken);
                listCatalogs.Add(new CatalogWithNavigationProperties
                {
                    Catalog = catalog,
                    Products = products,
                });
            }
            
            return listCatalogs;
        }
        

        public virtual async Task<List<Catalog>> GetListAsync(
            string? filterText = null,
            string? name = null,
            DateTime? fromMin = null,
            DateTime? fromMax = null,
            DateTime? toMin = null,
            DateTime? toMax = null,
            string? description = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, name, fromMin, fromMax, toMin, toMax, description);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? CatalogConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<Catalog>>()
                .PageBy<Catalog, IMongoQueryable<Catalog>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? name = null,
            DateTime? fromMin = null,
            DateTime? fromMax = null,
            DateTime? toMin = null,
            DateTime? toMax = null,
            string? description = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, name, fromMin, fromMax, toMin, toMax, description);
            return await query.As<IMongoQueryable<Catalog>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Catalog> ApplyFilter(
            IQueryable<Catalog> query,
            string? filterText = null,
            string? name = null,
            DateTime? fromMin = null,
            DateTime? fromMax = null,
            DateTime? toMin = null,
            DateTime? toMax = null,
            string? description = null)
        {
            filterText = filterText?.ToLower();
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Name.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase) || 
                                                                      e.Description != null && e.Description.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase))
                    .WhereIf(!string.IsNullOrWhiteSpace(name), e => name != null && e.Name.Contains(name!, StringComparison.CurrentCultureIgnoreCase))
                    .WhereIf(fromMin.HasValue, e => e.From >= fromMin!.Value)
                    .WhereIf(fromMax.HasValue, e => e.From <= fromMax!.Value)
                    .WhereIf(toMin.HasValue, e => e.To >= toMin!.Value)
                    .WhereIf(toMax.HasValue, e => e.To <= toMax!.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description != null && description != null && e.Description.Contains(description, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}