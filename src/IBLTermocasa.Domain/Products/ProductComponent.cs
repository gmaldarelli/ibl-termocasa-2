using System;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Products
{
    public class ProductComponent : Entity
    {

        public Guid ProductId { get; protected set; }

        public Guid ComponentId { get; protected set; }

        private ProductComponent()
        {

        }

        public ProductComponent(Guid productId, Guid componentId)
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