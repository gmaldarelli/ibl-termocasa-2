using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using IBLTermocasa.Subproducts;

using Volo.Abp;

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
        public ICollection<Subproduct> Subproducts { get; private set; }

        protected Product()
        {

        }

        public Product(Guid id, string code, string name, bool isAssembled, bool isInternal, string? description = null)
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
            Subproducts = new Collection<Subproduct>();
        }
        public virtual void AddComponent(Guid componentId)
        {
            Check.NotNull(componentId, nameof(componentId));

            if (IsInComponents(componentId))
            {
                return;
            }

            Components.Add(new ProductComponent(Id, componentId));
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

        public virtual void RemoveAllComponents()
        {
            Components.RemoveAll(x => x.ProductId == Id);
        }

        private bool IsInComponents(Guid componentId)
        {
            return Components.Any(x => x.ComponentId == componentId);
        }

        public virtual void AddQuestionTemplate(Guid questionTemplateId)
        {
            Check.NotNull(questionTemplateId, nameof(questionTemplateId));

            if (IsInQuestionTemplates(questionTemplateId))
            {
                return;
            }

            QuestionTemplates.Add(new ProductQuestionTemplate(Id, questionTemplateId));
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
    }
}