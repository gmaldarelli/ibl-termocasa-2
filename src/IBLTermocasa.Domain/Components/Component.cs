using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
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
        public virtual string Code { get; set; }
        [NotNull]
        public virtual string Name { get; set; }

        public List<ComponentItem> ComponentItems { get;  set; }

        protected Component()
        {

        }

        public Component(Guid id, string code, string name, List<ComponentItem> componentItems)
        {
            Id = id;
            Check.NotNull(code, nameof(code));
            Check.NotNull(name, nameof(name));
            Name = name;
            Code = code;
            ComponentItems = componentItems;
        }
        public static Component FillProperties(Component source, Component destination, IEnumerable<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                var sourceValue = property.GetValue(source);
                property.SetValue(destination, sourceValue);
            }

            return destination;
        }
        public static Component FillPropertiesForInsert(Component source, Component destination)
        {
            var properties = typeof(Component).GetProperties()
                .Where(p => p.CanRead && p.CanWrite && p.Name != "Id" && p.Name != "ConcurrencyStamp");
            return FillProperties(source, destination, properties);
        }
        public static Component FillPropertiesForUpdate(Component source, Component destination)
        {
            var properties = typeof(Component).GetProperties()
                .Where(p => p.CanRead && p.CanWrite && p.Name != "Id");
            return FillProperties(source, destination, properties);
        }
    }
}