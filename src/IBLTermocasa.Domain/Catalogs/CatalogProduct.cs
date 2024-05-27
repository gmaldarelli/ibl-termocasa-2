using System;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Catalogs
{
    public class CatalogProduct : Entity
    {

        public Guid CatalogId { get; protected set; }

        public Guid ProductId { get; protected set; }

        private CatalogProduct()
        {

        }

        public CatalogProduct(Guid catalogId, Guid productId)
        {
            CatalogId = catalogId;
            ProductId = productId;
        }

        public override object[] GetKeys()
        {
            return new object[]
                {
                    CatalogId,
                    ProductId
                };
        }
    }
}