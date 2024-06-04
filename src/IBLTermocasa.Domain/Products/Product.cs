using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace IBLTermocasa.Products
{
    public class Product : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Code { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        [CanBeNull]
        public virtual string? Description { get; set; }

        public virtual bool IsAssembled { get; set; }

        public virtual bool IsInternal { get; set; }

        public ICollection<ProductComponent> Components { get; private set; }
        public ICollection<ProductQuestionTemplate> QuestionTemplates { get; private set; }
        public ICollection<SubProduct> SubProducts { get;  set; }

        protected Product()
        {
            Components = new Collection<ProductComponent>();
            QuestionTemplates = new Collection<ProductQuestionTemplate>();
            SubProducts = new Collection<SubProduct>();
        }

        public Product(Guid id, string code, string name, bool isAssembled, bool isInternal, List<SubProduct>? subProducts, string? description = null)
        {
            Id = id;
            Check.NotNull(code, nameof(code));
            Check.NotNull(name, nameof(name));
            Code = code;
            Name = name;
            IsAssembled = isAssembled;
            IsInternal = isInternal;
            Description = description;
            Components = new Collection<ProductComponent>();
            QuestionTemplates = new Collection<ProductQuestionTemplate>();
            SubProducts = (subProducts is null) ? new Collection<SubProduct>() : new Collection<SubProduct>(subProducts);
        }
        public virtual void AddComponent(Guid componentId, int order, bool mandatory)
        {
            Check.NotNull(componentId, nameof(componentId));

            if (IsInComponents(componentId))
            {
                return;
            }

            Components.Add(new ProductComponent(Id, componentId, order, mandatory));
        }
        

        public virtual void RemoveComponent(Guid componentId)
        {
            Check.NotNull(componentId, nameof(componentId));

            if (!IsInComponents(componentId))
            {
                return;
            }

            Components.RemoveAll(x => x.ComponentId == componentId);
        }

        public virtual void RemoveAllComponentsExceptGivenIds(List<Guid> componentIds)
        {
            Check.NotNullOrEmpty(componentIds, nameof(componentIds));

            Components.RemoveAll(x => !componentIds.Contains(x.ComponentId));
        }

        public virtual void RemoveAllSubProducts()
        {
            SubProducts.RemoveAll(x => x.ParentId == Id);
        }
        
        public virtual void RemoveAllComponents()
        {
            Components.RemoveAll(x => x.ProductId == Id);
        }

        private bool IsInComponents(Guid componentId)
        {
            return Components.Any(x => x.ComponentId == componentId);
        }

        public virtual void AddQuestionTemplate(ProductQuestionTemplate questionTemplate)
        {
            Check.NotNull(questionTemplate.QuestionTemplateId, nameof(questionTemplate.QuestionTemplateId));
            Check.NotNull(questionTemplate.ProductId, nameof(questionTemplate.ProductId));

            if (IsInQuestionTemplates(questionTemplate.QuestionTemplateId))
            {
                return;
            }

            QuestionTemplates.Add(questionTemplate);
        }

        public virtual void RemoveQuestionTemplate(Guid questionTemplateId)
        {
            Check.NotNull(questionTemplateId, nameof(questionTemplateId));

            if (!IsInQuestionTemplates(questionTemplateId))
            {
                return;
            }

            QuestionTemplates.RemoveAll(x => x.QuestionTemplateId == questionTemplateId);
        }

        public virtual void RemoveAllQuestionTemplatesExceptGivenIds(List<Guid> questionTemplateIds)
        {
            Check.NotNullOrEmpty(questionTemplateIds, nameof(questionTemplateIds));

            QuestionTemplates.RemoveAll(x => !questionTemplateIds.Contains(x.QuestionTemplateId));
        }

        public virtual void RemoveAllQuestionTemplates()
        {
            QuestionTemplates.RemoveAll(x => x.ProductId == Id);
        }

        private bool IsInQuestionTemplates(Guid questionTemplateId)
        {
            return QuestionTemplates.Any(x => x.QuestionTemplateId == questionTemplateId);
        }
        
        public virtual void RemoveAllSubproducts()
        {
            SubProducts.RemoveAll(x => x.ParentId == Id);
        }
        
        public virtual void AddSubproduct(SubProduct subProduct)
        {
            Check.NotNull(subProduct, nameof(subProduct));

            if (IsInSubproducts(subProduct))
            {
                return;
            }

            SubProducts.Add(subProduct);
        }
        
        public virtual void RemoveSubproduct(SubProduct subProduct)
        {
            Check.NotNull(subProduct, nameof(subProduct));

            if (!IsInSubproducts(subProduct))
            {
                return;
            }

            SubProducts.Remove(subProduct);
        }
        
        private bool IsInSubproducts(SubProduct subProduct)
        {
            var inputOrdered =subProduct.ProductIds.Order();
            var entityOrderd = SubProducts.Select(x => x.ProductIds).ToList().Order();
            return entityOrderd.Any(x => x.SequenceEqual(inputOrdered));
        }
    }
}