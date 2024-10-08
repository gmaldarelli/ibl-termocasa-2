using IBLTermocasa.QuestionTemplates;
using IBLTermocasa.Components;
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

namespace IBLTermocasa.Products
{
    public class MongoProductRepository : MongoDbRepository<IBLTermocasaMongoDbContext, Product, Guid>, IProductRepository
    {
        public MongoProductRepository(IMongoDbContextProvider<IBLTermocasaMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        string? code = null,
            string? name = null,
            string? description = null,
            bool? isAssembled = null,
            bool? isInternal = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, code, name, description, isAssembled, isInternal);

            var ids = query.Select(x => x.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<ProductWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var product = await (await GetMongoQueryableAsync(cancellationToken))
                .FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));

            var componentIds = product.ProductComponents.Select(x => x.ComponentId).ToList();
            var components = await (await GetMongoQueryableAsync<Component>(cancellationToken)).Where(e => componentIds.Contains(e.Id)).ToListAsync(cancellationToken: cancellationToken);
            var questionTemplateIds = product.ProductQuestionTemplates.Select(x => x.QuestionTemplateId).ToList();
            var questionTemplates = await (await GetMongoQueryableAsync<QuestionTemplate>(cancellationToken)).Where(e => questionTemplateIds.Contains(e.Id)).ToListAsync(cancellationToken: cancellationToken);
            List<Guid> subProductIds = new List<Guid>();
            foreach (var subProduct in product.SubProducts)
            {
                subProductIds.Add(subProduct.ProductId);
            }
            var products = await (await GetMongoQueryableAsync<Product>(cancellationToken)).Where(e => subProductIds.Contains(e.Id)).ToListAsync(cancellationToken: cancellationToken);
            return new ProductWithNavigationProperties
            {
                Product = product,
                Components = components,
                QuestionTemplates = questionTemplates,
                Products = products
            };
        }

        public virtual async Task<List<ProductWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? code = null,
            string? name = null,
            string? description = null,
            bool? isAssembled = null,
            bool? isInternal = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, code, name, description, isAssembled, isInternal);
            var products = await query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ProductConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                .As<IMongoQueryable<Product>>()
                .PageBy<Product, IMongoQueryable<Product>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            var dbContext = await GetDbContextAsync(cancellationToken);
            return products.Select(s => new ProductWithNavigationProperties
            {
                Product = s,
                Components = new List<Component>(),
                QuestionTemplates = new List<QuestionTemplate>(),
                //Subproducts = new List<Subproduct>(),
            }).ToList();
        }

        public virtual async Task<List<Product>> GetListAsync(
            string? filterText = null,
            string? code = null,
            string? name = null,
            string? description = null,
            bool? isAssembled = null,
            bool? isInternal = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, code, name, description, isAssembled, isInternal);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ProductConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<Product>>()
                .PageBy<Product, IMongoQueryable<Product>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? code = null,
            string? name = null,
            string? description = null,
            bool? isAssembled = null,
            bool? isInternal = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, code, name, description, isAssembled, isInternal);
            return await query.As<IMongoQueryable<Product>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Product> ApplyFilter(
            IQueryable<Product> query,
            string? filterText = null,
            string? code = null,
            string? name = null,
            string? description = null,
            bool? isAssembled = null,
            bool? isInternal = null)
        {
            filterText = filterText?.ToLower();
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Code!.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase) || e.Name!.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase) || e.Description!.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase))
                    .WhereIf(!string.IsNullOrWhiteSpace(code), e => e.Code.Contains(code!, StringComparison.CurrentCultureIgnoreCase))
                    .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name!, StringComparison.CurrentCultureIgnoreCase))
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description != null && e.Description.Contains(description!, StringComparison.CurrentCultureIgnoreCase))
                    .WhereIf(isAssembled.HasValue, e => e.IsAssembled == isAssembled)
                    .WhereIf(isInternal.HasValue, e => e.IsInternal == isInternal);
        }
    }
}