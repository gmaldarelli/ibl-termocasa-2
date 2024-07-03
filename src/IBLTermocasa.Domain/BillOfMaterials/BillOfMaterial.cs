using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using IBLTermocasa.Common;
using IBLTermocasa.Types;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace IBLTermocasa.BillOfMaterials
{
    public class BillOfMaterial : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull] public virtual string BomNumber { get; set; }
        [NotNull] public virtual RequestForQuotationProperty RequestForQuotationProperty { get; set; } = new();
        public virtual string Notes { get; set; }

        [NotNull] public virtual BomStatus Status { get; set; } = BomStatus.CREATED;
        public virtual List<BomItem> ListItems { get; set; } = new();

        protected BillOfMaterial()
        {

        }

        public BillOfMaterial(Guid id, string bomNumber, RequestForQuotationProperty requestForQuotationProperty, List<BomItem> listItems = null, string? notes = null, BomStatus status = BomStatus.CREATED)
        {

            Id = id;
            Check.NotNull(bomNumber, nameof(bomNumber));
            BomNumber = bomNumber;
            RequestForQuotationProperty = requestForQuotationProperty;
            ListItems = listItems;
            Notes = notes;
            Status = status;
        }  

        //generate static method to fill all properties of the RequestForQuotation except the Id using reflection with 2 variants source and destination
        public static BillOfMaterial FillProperties(BillOfMaterial source, BillOfMaterial destination,
            IEnumerable<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                var sourceValue = property.GetValue(source);
                property.SetValue(destination, sourceValue);
            }

            return destination;
        }

        public static BillOfMaterial FillPropertiesForInsert(BillOfMaterial source,
            BillOfMaterial destination)
        {
            var properties = typeof(BillOfMaterial).GetProperties()
                .Where(p => p.CanRead && p.CanWrite && p.Name != "Id" && p.Name != "ConcurrencyStamp");
            return FillProperties(source, destination, properties);
        }

        public static BillOfMaterial FillPropertiesForUpdate(BillOfMaterial source,
            BillOfMaterial destination)
        {
            var properties = typeof(BillOfMaterial).GetProperties()
                .Where(p => p.CanRead && p.CanWrite && p.Name != "Id");
            return FillProperties(source, destination, properties);
        }
        
    }
}