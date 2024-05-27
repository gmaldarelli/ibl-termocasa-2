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
        List<Guid> componentIds,
        List<Guid> questionTemplateIds,
        string code, string name, bool isAssembled, bool isInternal, string? description = null)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var product = new Product(
             GuidGenerator.Create(),
             code, name, isAssembled, isInternal, description
             );

            await SetComponentsAsync(product, componentIds);
            await SetQuestionTemplatesAsync(product, questionTemplateIds);

            return await _productRepository.InsertAsync(product);
        }

        public virtual async Task<Product> UpdateAsync(
            Guid id,
            List<Guid> componentIds,
        List<Guid> questionTemplateIds,
        string code, string name, bool isAssembled, bool isInternal, string? description = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var queryable = await _productRepository.WithDetailsAsync(x => x.Components, x => x.QuestionTemplates);
            var query = queryable.Where(x => x.Id == id);

            var product = await AsyncExecuter.FirstOrDefaultAsync(query);

            product.Code = code;
            product.Name = name;
            product.IsAssembled = isAssembled;
            product.IsInternal = isInternal;
            product.Description = description;

            await SetComponentsAsync(product, componentIds);
            await SetQuestionTemplatesAsync(product, questionTemplateIds);

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

        private async Task SetQuestionTemplatesAsync(Product product, List<Guid> questionTemplateIds)
        {
            if (questionTemplateIds == null || !questionTemplateIds.Any())
            {
                product.RemoveAllQuestionTemplates();
                return;
            }

            var query = (await _questionTemplateRepository.GetQueryableAsync())
                .Where(x => questionTemplateIds.Contains(x.Id))
                .Select(x => x.Id);

            var questionTemplateIdsInDb = await AsyncExecuter.ToListAsync(query);
            if (!questionTemplateIdsInDb.Any())
            {
                return;
            }

            product.RemoveAllQuestionTemplatesExceptGivenIds(questionTemplateIdsInDb);

            foreach (var questionTemplateId in questionTemplateIdsInDb)
            {
                product.AddQuestionTemplate(questionTemplateId);
            }
        }

    }
}