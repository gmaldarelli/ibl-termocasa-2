using IBLTermocasa.Components;
using IBLTermocasa.QuestionTemplates;
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
    public class ProductManager : DomainService
    {
        protected IProductRepository _productRepository;
        protected IRepository<Component, Guid> _componentRepository;
        protected IRepository<QuestionTemplate, Guid> _questionTemplateRepository;

        public ProductManager(IProductRepository productRepository,
            IRepository<Component, Guid> componentRepository,
            IRepository<QuestionTemplate, Guid> questionTemplateRepository)
        {
            _productRepository = productRepository;
            _componentRepository = componentRepository;
            _questionTemplateRepository = questionTemplateRepository;
        }

        public virtual async Task<Product> CreateAsync(
            List<SubProduct> subProducts,
            List<ProductComponent> productComponents,
            List<ProductQuestionTemplate> questionTemplates,
            string code, string name, bool isAssembled, bool isInternal, string? description = null)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var product = new Product(
                GuidGenerator.Create(),
                code, name, isAssembled, isInternal, subProducts, description
            );
            await SetComponentsAsync(product, productComponents);
            await SetQuestionTemplatesAsync(product, questionTemplates);

            return await _productRepository.InsertAsync(product);
        }

        public virtual async Task<Product> UpdateAsync(
            Guid id,
            List<SubProduct> subProducts,
            List<ProductComponent> productComponents,
            List<ProductQuestionTemplate> questionTemplates,
            string code, string name, bool isAssembled, bool isInternal, string? description = null,
            [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var queryable = await _productRepository.WithDetailsAsync(x => x.ProductComponents, x => x.ProductQuestionTemplates,x => x.SubProducts);
            var query = queryable.Where(x => x.Id == id);

            var product = await AsyncExecuter.FirstOrDefaultAsync(query);

            product.Code = code;
            product.Name = name;
            product.IsAssembled = isAssembled;
            product.IsInternal = isInternal;
            product.Description = description;
            await SetSubProductsAsync(product, subProducts);
            await SetComponentsAsync(product, productComponents);
            await SetQuestionTemplatesAsync(product, questionTemplates);
            product.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _productRepository.UpdateAsync(product);
        }

        private async Task SetSubProductsAsync(Product product, List<SubProduct> subProducts)
        {
            if (subProducts == null || !subProducts.Any())
            {
                product.RemoveAllSubproducts();
                return;
            }
            var pruductIds = subProducts.SelectMany(x => x.ProductIds).ToList();

            var query = (await _productRepository.GetQueryableAsync())
                .Where(x => pruductIds.Contains(x.Id))
                .Select(x => x.Id);

            if (!pruductIds.Any())
            {
                return;
            }
            product.RemoveAllSubProductssExceptGivenIds(pruductIds);

            product.RemoveAllSubProducts();

            product.RemoveAllSubproducts();

            foreach (var subProduct in subProducts)
            {
                product.AddSubproduct(subProduct);
            }
        }

        private async Task SetComponentsAsync(Product product, List<ProductComponent> productComponents)
        {
            if (productComponents == null || !productComponents.Any())
            {
                product.RemoveAllComponents();
                return;
            }
            var componentIds = productComponents.Select(x => x.ComponentId).ToList();

            var query = (await _componentRepository.GetQueryableAsync())
                .Where(x => componentIds.Contains(x.Id))
                .Select(x => x.Id);

            if (!componentIds.Any())
            {
                return;
            }
            product.RemoveAllComponentsExceptGivenIds(componentIds);

            product.RemoveAllComponents();

            foreach (var productComponent in productComponents)
            {
                product.AddComponent(productComponent.ComponentId, productComponent.Order, productComponent.Code, productComponent.Name, productComponent.Mandatory, productComponent.ConsumptionCalculation);
            }
        }

        private async Task SetQuestionTemplatesAsync(Product product, List<ProductQuestionTemplate> questionTemplates)
        {
            if (questionTemplates == null || !questionTemplates.Any())
            {
                product.RemoveAllQuestionTemplates();
                return;
            }
            var questionTemplateIds = questionTemplates.Select(x => x.QuestionTemplateId).ToList();

            var query = (await _questionTemplateRepository.GetQueryableAsync())
                .Where(x => questionTemplateIds.Contains(x.Id))
                .Select(x => x.Id);

            var questionTemplateIdsInDb = await AsyncExecuter.ToListAsync(query);
            if (!questionTemplateIdsInDb.Any())
            {
                return;
            }

            product.RemoveAllQuestionTemplatesExceptGivenIds(questionTemplateIdsInDb);
            product.RemoveAllQuestionTemplates();

            foreach (var questionTemplateId in questionTemplateIdsInDb)
            {
                var entity = questionTemplates.Where(x => x.QuestionTemplateId == questionTemplateId).First();
                product.AddQuestionTemplate(entity);
            }
        }
    }
}