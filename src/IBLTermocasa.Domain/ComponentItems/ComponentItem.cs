using IBLTermocasa.Materials;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace IBLTermocasa.ComponentItems
{
    public abstract class ComponentItemBase : Entity<Guid>, IMultiTenant
    {
        public virtual Guid ComponentId { get; set; }

        public virtual Guid? TenantId { get; set; }

        public virtual bool IsDefault { get; set; }
        public Guid MaterialId { get; set; }

        protected ComponentItemBase()
        {

        }

        public ComponentItemBase(Guid id, Guid componentId, Guid materialId, bool isDefault)
        {

            Id = id;
            ComponentId = componentId;
            IsDefault = isDefault;
            MaterialId = materialId;
        }

    }
}