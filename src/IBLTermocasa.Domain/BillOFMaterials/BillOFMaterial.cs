using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using IBLTermocasa.Common;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace IBLTermocasa.BillOFMaterials
{
    public class BillOFMaterial : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        [NotNull] public virtual RequestForQuotationProperty RequestForQuotationProperty { get; set; } = new();

        public virtual List<BOMItem>? ListItems { get; set; } = new();

        protected BillOFMaterial()
        {

        }

        public BillOFMaterial(Guid id, string name, RequestForQuotationProperty requestForQuotationProperty, List<BOMItem> listItems = null)
        {

            Id = id;
            Check.NotNull(name, nameof(name));
            Name = name;
            RequestForQuotationProperty = requestForQuotationProperty;
            ListItems = listItems;
        }

        //generete static methot to fill all properties of the RequestForQuotation except the Id using reflection with 2 variants source and destination
        public static BillOFMaterial FillProperties(BillOFMaterial source, BillOFMaterial destination,
            IEnumerable<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                var sourceValue = property.GetValue(source);
                property.SetValue(destination, sourceValue);
            }

            return destination;
        }

        public static BillOFMaterial FillPropertiesForInsert(BillOFMaterial source,
            BillOFMaterial destination)
        {
            var properties = typeof(BillOFMaterial).GetProperties()
                .Where(p => p.CanRead && p.CanWrite && p.Name != "Id" && p.Name != "ConcurrencyStamp");
            return FillProperties(source, destination, properties);
        }

        public static BillOFMaterial FillPropertiesForUpdate(BillOFMaterial source,
            BillOFMaterial destination)
        {
            var properties = typeof(BillOFMaterial).GetProperties()
                .Where(p => p.CanRead && p.CanWrite && p.Name != "Id");
            return FillProperties(source, destination, properties);
        }
        
    }
}