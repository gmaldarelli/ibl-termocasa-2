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

namespace IBLTermocasa.ProfessionalProfiles
{
    public class ProfessionalProfile : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        public virtual string Code { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        public virtual double StandardPrice { get; set; }

        protected ProfessionalProfile()
        {

        }

        public ProfessionalProfile(Guid id, string code, string name, double standardPrice)
        {
            Id = id;
            Code = code;
            Check.NotNull(name, nameof(name));
            Name = name;
            StandardPrice = standardPrice;
        }
        
        //generate static method to fill all properties of the ProfessionalProfile except the Id using reflection with 2 variants source and destination
        public static ProfessionalProfile FillProperties(ProfessionalProfile source, ProfessionalProfile destination,
            IEnumerable<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                var sourceValue = property.GetValue(source);
                property.SetValue(destination, sourceValue);
            }

            return destination;
        }

        public static ProfessionalProfile FillPropertiesForInsert(ProfessionalProfile source,
            ProfessionalProfile destination)
        {
            var properties = typeof(ProfessionalProfile).GetProperties()
                .Where(p => p.CanRead && p.CanWrite && p.Name != "Id" && p.Name != "ConcurrencyStamp");
            return FillProperties(source, destination, properties);
        }

        public static ProfessionalProfile FillPropertiesForUpdate(ProfessionalProfile source,
            ProfessionalProfile destination)
        {
            var properties = typeof(ProfessionalProfile).GetProperties()
                .Where(p => p.CanRead && p.CanWrite && p.Name != "Id");
            return FillProperties(source, destination, properties);
        }

    }
}