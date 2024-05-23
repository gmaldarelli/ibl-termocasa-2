using IBLTermocasa.Products;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace IBLTermocasa.Subproducts
{
    public abstract class SubproductBase : Entity<Guid>
    {
        public virtual Guid ProductId { get; set; }

        public virtual int Order { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        public virtual bool IsSingleProduct { get; set; }

        public virtual bool Mandatory { get; set; }
        public Guid? SingleProductId { get; set; }

        protected SubproductBase()
        {

        }

        public SubproductBase(Guid id, Guid productId, Guid? singleProductId, int order, string name, bool isSingleProduct, bool mandatory)
        {

            Id = id;
            Check.NotNull(name, nameof(name));
            ProductId = productId;
            Order = order;
            Name = name;
            IsSingleProduct = isSingleProduct;
            Mandatory = mandatory;
            SingleProductId = singleProductId;
        }

    }
}