using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace IBLTermocasa.ConsumptionEstimations
{
    public class ConsumptionEstimation : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }
        public virtual Guid IdProduct { get; set; }
        
        public virtual List<ConsumptionProduct> ConsumptionProduct { get; set; }
        
        public virtual List<ConsumptionWork> ConsumptionWork { get; set; }

        protected ConsumptionEstimation()
        {

        }

        public ConsumptionEstimation(Guid id, Guid idProduct, List<ConsumptionProduct> consumptionProduct = null, List<ConsumptionWork> consumptionWork = null)
        {
            Id = id;
            IdProduct = idProduct;
            ConsumptionProduct = consumptionProduct;
            ConsumptionWork = consumptionWork;
        }
        
        //generate static method to fill all properties of the ConsumptionEstimation except the Id using reflection with 2 variants source and destination
        public static ConsumptionEstimation FillProperties(ConsumptionEstimation source, ConsumptionEstimation destination,
            IEnumerable<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                var sourceValue = property.GetValue(source);
                property.SetValue(destination, sourceValue);
            }

            return destination;
        }

        public static ConsumptionEstimation FillPropertiesForInsert(ConsumptionEstimation source,
            ConsumptionEstimation destination)
        {
            var properties = typeof(ConsumptionEstimation).GetProperties()
                .Where(p => p.CanRead && p.CanWrite && p.Name != "Id" && p.Name != "ConcurrencyStamp");
            return FillProperties(source, destination, properties);
        }

        public static ConsumptionEstimation FillPropertiesForUpdate(ConsumptionEstimation source,
            ConsumptionEstimation destination)
        {
            var properties = typeof(ConsumptionEstimation).GetProperties()
                .Where(p => p.CanRead && p.CanWrite && p.Name != "Id");
            return FillProperties(source, destination, properties);
        }

    }
}