using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace IBLTermocasa.Catalogs
{
    public class Catalog : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        public virtual DateTime From { get; set; }

        public virtual DateTime To { get; set; }

        [CanBeNull]
        public virtual string? Description { get; set; }

        public ICollection<CatalogProduct> Products { get; private set; }

        protected Catalog()
        {

        }

        public Catalog(Guid id, string name, DateTime from, DateTime to, string? description = null)
        {

            Id = id;
            Check.NotNull(name, nameof(name));
            Name = name;
            From = from;
            To = to;
            Description = description;
            Products = new Collection<CatalogProduct>();
        }
        public virtual void AddProduct(Guid productId)
        {
            Check.NotNull(productId, nameof(productId));

            if (IsInProducts(productId))
            {
                return;
            }

            Products.Add(new CatalogProduct(Id, productId));
        }

        public virtual void RemoveProduct(Guid productId)
        {
            Check.NotNull(productId, nameof(productId));

            if (!IsInProducts(productId))
            {
                return;
            }

            Products.RemoveAll(x => x.ProductId == productId);
        }

        public virtual void RemoveAllProductsExceptGivenIds(List<Guid> productIds)
        {
            Check.NotNullOrEmpty(productIds, nameof(productIds));

            Products.RemoveAll(x => !productIds.Contains(x.ProductId));
        }

        public virtual void RemoveAllProducts()
        {
            Products.RemoveAll(x => x.CatalogId == Id);
        }

        private bool IsInProducts(Guid productId)
        {
            return Products.Any(x => x.ProductId == productId);
        }
    }
}