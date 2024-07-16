using IBLTermocasa.Types;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using IBLTermocasa.Quotations;
using IBLTermocasa.RequestForQuotations;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace IBLTermocasa.Quotations
{
    public class Quotation : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }
        
        public virtual Guid IdRFQ { get; set; }

        public virtual Guid IdBOM { get; set; }

        public virtual string Code { get; set; }

        public virtual string Name { get; set; }

        public virtual DateTime CreationDate { get; set; }
        public virtual DateTime? SentDate { get; set; }

        public virtual DateTime? QuotationValidDate { get; set; }

        public virtual DateTime? ConfirmedDate { get; set; }

        public virtual QuotationStatus Status { get; set; }

        public virtual bool DepositRequired { get; set; }

        public virtual double? DepositRequiredValue { get; set; }

        [CanBeNull]
        public virtual List<QuotationItem>? QuotationItems { get; set; }

        protected Quotation()
        {

        }
        
        public Quotation(Guid id, Guid idRFQ, Guid idBOM, string code, string name, DateTime creationDate,  DateTime? sentDate, DateTime? quotationValidDate, DateTime? confirmedDate, QuotationStatus status, bool depositRequired, double? depositRequiredValue, List<QuotationItem>? quotationItems) : base(id)
        {
            Id = id;
            Check.NotNull(idRFQ, nameof(idRFQ));
            Check.NotNull(idBOM, nameof(idBOM));
            Check.NotNull(code, nameof(code));
            Check.NotNull(name, nameof(name));
            IdRFQ = idRFQ;
            IdBOM = idBOM;
            Code = code;
            Name = name;
            CreationDate = creationDate;
            SentDate = sentDate;
            QuotationValidDate = quotationValidDate;
            ConfirmedDate = confirmedDate;
            Status = status;
            DepositRequired = depositRequired;
            DepositRequiredValue = depositRequiredValue;
            QuotationItems = quotationItems;
        }
        
        //generate static method to fill all properties of the Quotation except the Id using reflection with 2 variants source and destination
        public static Quotation FillProperties(Quotation source, Quotation destination,
            IEnumerable<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                var sourceValue = property.GetValue(source);
                property.SetValue(destination, sourceValue);
            }

            return destination;
        }

        public static Quotation FillPropertiesForInsert(Quotation source,
            Quotation destination)
        {
            var properties = typeof(Quotation).GetProperties()
                .Where(p => p.CanRead && p.CanWrite && p.Name != "Id" && p.Name != "ConcurrencyStamp");
            return FillProperties(source, destination, properties);
        }

        public static Quotation FillPropertiesForUpdate(Quotation source,
            Quotation destination)
        {
            var properties = typeof(Quotation).GetProperties()
                .Where(p => p.CanRead && p.CanWrite && p.Name != "Id");
            return FillProperties(source, destination, properties);
        }

    }
}