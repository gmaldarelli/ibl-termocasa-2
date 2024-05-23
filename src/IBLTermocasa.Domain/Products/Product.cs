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
    public abstract class ProductBase : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual string Code { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        [CanBeNull]
        public virtual string? Description { get; set; }

        public virtual bool IsAssembled { get; set; }

        public virtual bool IsInternal { get; set; }

        public ICollection<ProductComponent> Components { get; private set; }
        public ICollection<Subproduct> Subproducts { get; private set; }

        protected ProductBase()
        {

        }

        public ProductBase(Guid id, string code, string name, bool isAssembled, bool isInternal, string? description = null)
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
    }
}