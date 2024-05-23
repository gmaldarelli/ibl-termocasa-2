using System;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Components
{
    public class ComponentMaterial : Entity
    {

        public Guid ComponentId { get; protected set; }

        public Guid MaterialId { get; protected set; }

        private ComponentMaterial()
        {

        }

        public ComponentMaterial(Guid componentId, Guid materialId)
        {
            ComponentId = componentId;
            MaterialId = materialId;
        }

        public override object[] GetKeys()
        {
            return new object[]
                {
                    ComponentId,
                    MaterialId
                };
        }
    }
}