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
        
        public virtual void RemoveAllSubProducts()
        {
            SubProducts.RemoveAll(x => true);
        }
        
        public virtual void RemoveAllComponents()
        {
            ProductComponents.RemoveAll(x  => true);
        }
        public virtual void RemoveAllQuestionTemplates()
        {
            ProductQuestionTemplates.RemoveAll(x => true);
        }

        public virtual void RemoveAllSubproducts()
        {
            SubProducts.RemoveAll(x => true);
        }
    }
}