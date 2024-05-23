using IBLTermocasa.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace IBLTermocasa.Products
{
    public abstract class ProductManagerBase : DomainService
    {
        protected IProductRepository _productRepository;
        protected IRepository<Component, Guid> _componentRepository;

        public ProductManagerBase(IProductRepository productRepository,
        IRepository<Component, Guid> componentRepository)
        {
            _productRepository = productRepository;
            _componentRepository = componentRepository;
        }

        public virtual async Task<Product> CreateAsync(
        List<Guid> componentIds,
        string code, string name, bool isAssembled, bool isInternal, string? description = null)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var product = new Product(
             GuidGenerator.Create(),
             code, name, isAssembled, isInternal, description
             );

            await SetComponentsAsync(product, componentIds);

            return await _productRepository.InsertAsync(product);
        }

        public virtual async Task<Product> UpdateAsync(
            Guid id,
            List<Guid> componentIds,
        string code, string name, bool isAssembled, bool isInternal, string? description = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var queryable = await _productRepository.WithDetailsAsync(x => x.Components);
            var query = queryable.Where(x => x.Id == id);

            var product = await AsyncExecuter.FirstOrDefaultAsync(query);

            product.Code = code;
            product.Name = name;
            product.IsAssembled = isAssembled;
            product.IsInternal = isInternal;
            product.Description = description;

            await SetComponentsAsync(product, componentIds);

            product.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _productRepository.UpdateAsync(product);
        }

        private async Task SetComponentsAsync(Product product, List<Guid> componentIds)
        {
            if (componentIds == null || !componentIds.Any())
            {
                product.RemoveAllComponents();
                return;
            }

            var query = (await _componentRepository.GetQueryableAsync())
                .Where(x => componentIds.Contains(x.Id))
                .Select(x => x.Id);

            var componentIdsInDb = await AsyncExecuter.ToListAsync(query);
            if (!componentIdsInDb.Any())
            {
                return;
            }

            product.RemoveAllComponentsExceptGivenIds(componentIdsInDb);

            foreach (var componentId in componentIdsInDb)
            {
                product.AddComponent(componentId);
            }
        }

    }
}