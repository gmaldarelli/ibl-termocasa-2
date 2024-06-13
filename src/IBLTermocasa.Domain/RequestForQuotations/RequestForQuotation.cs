using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IBLTermocasa.Common;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace IBLTermocasa.RequestForQuotations
{
    public class RequestForQuotation : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        public virtual string QuoteNumber { get; set; }

        public virtual string? WorkSite { get; set; }

        public virtual string? City { get; set; }
        public virtual AgentProperty? AgentProperty { get; set; }

        public virtual OrganizationProperty? OrganizationProperty { get; set; }

        public virtual ContactProperty? ContactProperty { get; set; }

        public virtual PhoneInfo? PhoneInfo { get; set; }

        public virtual MailInfo? MailInfo { get; set; }

        public virtual decimal Discount { get; set; }

        public virtual string? Description { get; set; }

        public virtual Status Status { get; set; }
        public DateTime? DateDocument { get; set; }
        public List<RequestForQuotationItem>? RequestForQuotationItems { get; set; }

        protected RequestForQuotation()
        {
        }

        public RequestForQuotation(Guid id, string quoteNumber, string? workSite, string? city, AgentProperty? agentProperty,
            OrganizationProperty? organizationProperty, ContactProperty? contactProperty, PhoneInfo? phoneInfo,
            MailInfo? mailInfo, decimal discount, string? description, Status status, DateTime? dateDocument,
            List<RequestForQuotationItem>? requestForQuotationItems)
        {
            Id = id;
            QuoteNumber = Check.NotNullOrWhiteSpace(quoteNumber, nameof(quoteNumber));
            WorkSite = workSite;
            City = city;
            AgentProperty = agentProperty;
            OrganizationProperty = organizationProperty;
            ContactProperty = contactProperty;
            PhoneInfo = phoneInfo;
            MailInfo = mailInfo;
            Discount = discount;
            Description = description;
            Status = status;
            DateDocument = dateDocument;
            RequestForQuotationItems = requestForQuotationItems;
        }

        //generete static methot to fill all properties of the RequestForQuotation except the Id using reflection with 2 variants source and destination
        public static RequestForQuotation FillProperties(RequestForQuotation source, RequestForQuotation destination,
            IEnumerable<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                var sourceValue = property.GetValue(source);
                property.SetValue(destination, sourceValue);
            }

            return destination;
        }

        public static RequestForQuotation FillPropertiesForInsert(RequestForQuotation source,
            RequestForQuotation destination)
        {
            var properties = typeof(RequestForQuotation).GetProperties()
                .Where(p => p.CanRead && p.CanWrite && p.Name != "Id" && p.Name != "ConcurrencyStamp");
            return FillProperties(source, destination, properties);
        }

        public static RequestForQuotation FillPropertiesForUpdate(RequestForQuotation source,
            RequestForQuotation destination)
        {
            var properties = typeof(RequestForQuotation).GetProperties()
                .Where(p => p.CanRead && p.CanWrite && p.Name != "Id");
            return FillProperties(source, destination, properties);
        }
    }
}