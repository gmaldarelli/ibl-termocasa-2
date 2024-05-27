using IBLTermocasa.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace IBLTermocasa.Catalogs
{
    public class CatalogManager : DomainService
    {
        protected ICatalogRepository _catalogRepository;
        protected IRepository<Product, Guid> _productRepository;

        public CatalogManager(ICatalogRepository catalogRepository,
        IRepository<Product, Guid> productRepository)
        {
            _catalogRepository = catalogRepository;
            _productRepository = productRepository;
        }

        public virtual async Task<Catalog> CreateAsync(
        List<Guid> productIds,
        string name, DateTime from, DateTime to, string? description = null)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(from, nameof(from));
            Check.NotNull(to, nameof(to));

            var catalog = new Catalog(
             GuidGenerator.Create(),
             name, from, to, description
             );

            await SetProductsAsync(catalog, productIds);

            return await _catalogRepository.InsertAsync(catalog);
        }

        public virtual async Task<Catalog> UpdateAsync(
            Guid id,
            List<Guid> productIds,
        string name, DateTime from, DateTime to, string? description = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNull(from, nameof(from));
            Check.NotNull(to, nameof(to));

            var queryable = await _catalogRepository.WithDetailsAsync(x => x.Products);
            var query = queryable.Where(x => x.Id == id);

            var catalog = await AsyncExecuter.FirstOrDefaultAsync(query);

            catalog.Name = name;
            catalog.From = from;
            catalog.To = to;
            catalog.Description = description;

            await SetProductsAsync(catalog, productIds);

            catalog.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _catalogRepository.UpdateAsync(catalog);
        }

        private async Task SetProductsAsync(Catalog catalog, List<Guid> productIds)
        {
            if (productIds == null || !productIds.Any())
            {
                catalog.RemoveAllProducts();
                return;
            }

            var query = (await _productRepository.GetQueryableAsync())
                .Where(x => productIds.Contains(x.Id))
                .Select(x => x.Id);

            var productIdsInDb = await AsyncExecuter.ToListAsync(query);
            if (!productIdsInDb.Any())
            {
                return;
            }

            catalog.RemoveAllProductsExceptGivenIds(productIdsInDb);

            foreach (var productId in productIdsInDb)
            {
                catalog.AddProduct(productId);
            }
        }

    }
}