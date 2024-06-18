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

        public ICollection<ProductComponent> ProductComponents { get; private set; }
        public ICollection<ProductQuestionTemplate> ProductQuestionTemplates { get; private set; }
        public ICollection<SubProduct> SubProducts { get;  set; }

        protected Product()
        {
            ProductComponents = new Collection<ProductComponent>();
            ProductQuestionTemplates = new Collection<ProductQuestionTemplate>();
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
            ProductComponents = new Collection<ProductComponent>();
            ProductQuestionTemplates = new Collection<ProductQuestionTemplate>();
            SubProducts = (subProducts is null) ? new Collection<SubProduct>() : new Collection<SubProduct>(subProducts);
        }
        public virtual void AddComponent(Guid componentId, int order, string code, string name, bool mandatory)
        {
            Check.NotNull(componentId, nameof(componentId));

            if (IsInComponents(componentId))
            {
                return;
            }

            ProductComponents.Add(new ProductComponent(Id, componentId, order, code, name, mandatory));
        }
        

        public virtual void RemoveComponent(Guid componentId)
        {
            Check.NotNull(componentId, nameof(componentId));

            if (!IsInComponents(componentId))
            {
                return;
            }

            ProductComponents.RemoveAll(x => x.ComponentId == componentId);
        }

        public virtual void RemoveAllComponentsExceptGivenIds(List<Guid> componentIds)
        {
            Check.NotNullOrEmpty(componentIds, nameof(componentIds));

            ProductComponents.RemoveAll(x => !componentIds.Contains(x.ComponentId));
        }

        public virtual void RemoveAllSubProducts()
        {
            SubProducts.RemoveAll(x => true);
        }
        
        public virtual void RemoveAllComponents()
        {
            ProductComponents.RemoveAll(x => x.ProductId == Id);
        }

        private bool IsInComponents(Guid componentId)
        {
            return ProductComponents.Any(x => x.ComponentId == componentId);
        }

        public virtual void AddQuestionTemplate(ProductQuestionTemplate questionTemplate)
        {
            Check.NotNull(questionTemplate.QuestionTemplateId, nameof(questionTemplate.QuestionTemplateId));
            Check.NotNull(questionTemplate.ProductId, nameof(questionTemplate.ProductId));
            Check.NotNull(questionTemplate.Order, nameof(questionTemplate.Order));
            Check.NotNull(questionTemplate.Name, nameof(questionTemplate.Name));
            if (IsInQuestionTemplates(questionTemplate.QuestionTemplateId))
            {
                return;
            }

            ProductQuestionTemplates.Add(questionTemplate);
        }

        public virtual void RemoveQuestionTemplate(Guid questionTemplateId)
        {
            Check.NotNull(questionTemplateId, nameof(questionTemplateId));

            if (!IsInQuestionTemplates(questionTemplateId))
            {
                return;
            }

            ProductQuestionTemplates.RemoveAll(x => x.QuestionTemplateId == questionTemplateId);
        }

        public virtual void RemoveAllQuestionTemplatesExceptGivenIds(List<Guid> questionTemplateIds)
        {
            Check.NotNullOrEmpty(questionTemplateIds, nameof(questionTemplateIds));

            ProductQuestionTemplates.RemoveAll(x => !questionTemplateIds.Contains(x.QuestionTemplateId));
        }

        public virtual void RemoveAllQuestionTemplates()
        {
            ProductQuestionTemplates.RemoveAll(x => x.ProductId == Id);
        }

        private bool IsInQuestionTemplates(Guid questionTemplateId)
        {
            return ProductQuestionTemplates.Any(x => x.QuestionTemplateId == questionTemplateId);
        }
        
        public virtual void RemoveAllSubproducts()
        {
            SubProducts.RemoveAll(x => true);
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

        public void RemoveAllSubProductssExceptGivenIds(List<Guid> pruductIds)
        {
            Check.NotNullOrEmpty(pruductIds, nameof(pruductIds));
            List<Guid> toRemonveGuids = new List<Guid>();
            foreach (var subProduct in SubProducts)
            {
                subProduct.ProductIds.RemoveAll(x => !pruductIds.Contains(x));
                if (subProduct.ProductIds.Count == 0)
                {
                    toRemonveGuids.Add(subProduct.Id);
                }
            }
            SubProducts.RemoveAll(x => !toRemonveGuids.Contains(x.Id));
        }
    }
}