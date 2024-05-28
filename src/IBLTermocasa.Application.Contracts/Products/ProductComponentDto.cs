using System;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Products
{
    public class ProductComponentDto : Entity
    {

        public Guid ProductId { get; protected set; }

        public Guid ComponentId { get; protected set; }

        private ProductComponentDto()
        {

        }

        public ProductComponentDto(Guid productId, Guid componentId)
        {
            ProductId = productId;
            ComponentId = componentId;
        }

        public override object[] GetKeys()
        {
            return new object[]
                {
                    ProductId,
                    ComponentId
                };
        }
    }
}