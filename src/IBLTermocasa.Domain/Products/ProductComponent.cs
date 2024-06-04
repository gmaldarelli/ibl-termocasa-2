using System;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Products
{
    public class ProductComponent : Entity
    {
        public Guid ProductId { get;  set; }

        public Guid ComponentId { get;  set; }

        public virtual int Order { get; set; }
        public virtual bool Mandatory { get; set; }
        private ProductComponent()
        {

        }

        public ProductComponent(Guid productId, Guid componentId, int order, bool mandatory)
        {
            ProductId = productId;
            ComponentId = componentId;
            Order = order;
            Mandatory = mandatory;
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