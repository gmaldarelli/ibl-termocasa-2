using System;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Office2010.Excel;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Products
{
    public  class SubProduct : Entity
    {
        public virtual Guid Id { get; set; }
        public virtual  ICollection<Guid> ProductIds { get; set; } = new List<Guid>();
        [NotNull]
        public virtual int Order { get; set; }
        [NotNull]
        public virtual string Name { get; set; }

        public virtual bool IsSingleProduct { get; set; }

        public virtual bool Mandatory { get; set; }

        private SubProduct()
        {
        }

        public SubProduct( Guid id, ICollection<Guid> productIds, int order, string name, bool isSingleProduct, bool mandatory)
        {

            Check.NotNull(name, nameof(name));
            Check.NotNull(id, nameof(id));
            Check.NotNull(productIds, nameof(productIds));
            Id = id;
            ProductIds = productIds;
            Order = order;
            Name = name;
            IsSingleProduct = isSingleProduct;
            Mandatory = mandatory;
        }

        public override object[] GetKeys()
        {
            return new object[]
            {
                Id
            };
        }
    }
}