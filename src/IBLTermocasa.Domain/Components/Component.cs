using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;


using Volo.Abp;

namespace IBLTermocasa.Components
{
    public class Component : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        public List<ComponentItem> ComponentItems { get;  set; }

        protected Component()
        {

        }

        public Component(Guid id, string name, List<ComponentItem> componentItems)
        {
            Id = id;
            Check.NotNull(name, nameof(name));
            Name = name;
            ComponentItems = componentItems;
        }

    }
}