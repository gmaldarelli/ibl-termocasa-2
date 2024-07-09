using System;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Products
{
    public class ProductComponent : Entity<Guid>
    {
        [NotNull]
        public virtual Guid ParentId { get;  set; }
        [NotNull]
        public virtual Guid ComponentId { get;  set; }
        [NotNull]
        public virtual int Order { get; set; }
        [NotNull]
        public virtual string Code { get; set; }
        [NotNull]
        public virtual string Name { get; set; }
        [NotNull]
        public virtual bool Mandatory { get; set; }
        private ProductComponent()
        {

        }

        public ProductComponent(Guid id, Guid parentId, Guid componentId, int order, string code, string name, bool mandatory)
        {
            Check.NotNull(code, nameof(code));
            Check.NotNull(name, nameof(name));
            Check.NotNull(id, nameof(id));
            Check.NotNull(componentId, nameof(componentId));
            Check.NotNull(parentId, nameof(parentId));
            Id = id;
            ParentId = parentId;
            ComponentId = componentId;
            Order = order;
            Code = code;
            Name = name;
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