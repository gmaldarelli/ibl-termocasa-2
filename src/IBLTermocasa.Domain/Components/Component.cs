using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using IBLTermocasa.ComponentItems;

using Volo.Abp;

namespace IBLTermocasa.Components
{
    public class Component : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        public ICollection<ComponentItem> ComponentItems { get; private set; }

        protected Component()
        {

        }

        public Component(Guid id, string name)
        {

            Id = id;
            Check.NotNull(name, nameof(name));
            Name = name;
            ComponentItems = new Collection<ComponentItem>();
        }

    }
}