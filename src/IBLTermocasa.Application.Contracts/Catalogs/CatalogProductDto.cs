using System;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Catalogs
{
    public class CatalogProductDto
    {

        public Guid CatalogId { get; protected set; }

        public Guid ProductId { get; protected set; }

        private CatalogProductDto()
        {

        }

        public CatalogProductDto(Guid catalogId, Guid productId)
        {
            CatalogId = catalogId;
            ProductId = productId;
        }

        public object[] GetKeys()
        {
            return new object[]
                {
                    CatalogId,
                    ProductId
                };
        }
    }
}