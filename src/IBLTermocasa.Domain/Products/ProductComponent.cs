using System;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Products
{
    public class ProductComponent : Entity
    {
        public Guid ProductId { get;  set; }

        public Guid ComponentId { get;  set; }

        public virtual int Order { get; set; }
        
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }
        public virtual bool Mandatory { get; set; }
        public virtual string? ConsumptionCalculation { get; set; }
        private ProductComponent()
        {

        }

        public ProductComponent(Guid productId, Guid componentId, int order, string code, string name, bool mandatory, string consumptionCalculation)
        {
            ProductId = productId;
            ComponentId = componentId;
            Order = order;
            Code = code;
            Name = name;
            Mandatory = mandatory;
            ConsumptionCalculation = consumptionCalculation;
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