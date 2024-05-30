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

        [NotNull] public virtual string QuoteNumber { get; set; }

        [CanBeNull] public virtual string? WorkSite { get; set; }

        [CanBeNull] public virtual string? City { get; set; }

        [CanBeNull] public virtual OrganizationProperty? OrganizationProperty { get; set; }

        [CanBeNull] public virtual ContactProperty? ContactProperty { get; set; }

        [CanBeNull] public virtual PhoneInfo? PhoneInfo { get; set; }

        [CanBeNull] public virtual MailInfo? MailInfo { get; set; }

        public virtual decimal Discount { get; set; }

        [CanBeNull] public virtual string? Description { get; set; }

        public virtual Status Status { get; set; }
        public Guid? AgentId { get; set; }
        public Guid? ContactId { get; set; }
        public Guid? OrganizationId { get; set; }
        public DateTime? DateDocument { get; set; }
        public List<RequestForQuotationItem>? RequestForQuotationItems { get; set; }

        protected RequestForQuotation()
        {
        }

        public RequestForQuotation(Guid id, Guid? agentId, Guid? contactId, Guid? organizationId, string quoteNumber,
            decimal discount, Status status, string? workSite = null, string? city = null,
            OrganizationProperty? organizationProperty = null, ContactProperty? contactProperty = null,
            PhoneInfo? phoneInfo = null, MailInfo? mailInfo = null, string? description = null,
            DateTime? dateDocument = null, List<RequestForQuotationItem> requestForQuotationItems = null)
        {
            Id = id;
            Check.NotNull(quoteNumber, nameof(quoteNumber));
            QuoteNumber = quoteNumber;
            Discount = discount;
            Status = status;
            WorkSite = workSite;
            City = city;
            OrganizationProperty = organizationProperty;
            ContactProperty = contactProperty;
            PhoneInfo = phoneInfo;
            MailInfo = mailInfo;
            Description = description;
            AgentId = agentId;
            ContactId = contactId;
            OrganizationId = organizationId;
            DateDocument = dateDocument;
            RequestForQuotationItems = requestForQuotationItems;
        }

        //generete static methot to fill all properties of the Organization except the Id using reflection with 2 variants source and destination
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