using IBLTermocasa.RequestForQuotations;
using Volo.Abp.Identity;
using IBLTermocasa.Contacts;
using IBLTermocasa.Organizations;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace IBLTermocasa.RequestForQuotations
{
    public class RequestForQuotation : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string QuoteNumber { get; set; }

        [CanBeNull]
        public virtual string? WorkSite { get; set; }

        [CanBeNull]
        public virtual string? City { get; set; }

        [CanBeNull]
        public virtual string? OrganizationProperty { get; set; }

        [CanBeNull]
        public virtual string? ContactProperty { get; set; }

        [CanBeNull]
        public virtual string? PhoneInfo { get; set; }

        [CanBeNull]
        public virtual string? MailInfo { get; set; }

        public virtual decimal Discount { get; set; }

        [CanBeNull]
        public virtual string? Description { get; set; }

        public virtual Status Status { get; set; }
        public Guid? AgentId { get; set; }
        public Guid? ContactId { get; set; }
        public Guid? OrganizationId { get; set; }

        protected RequestForQuotation()
        {

        }

        public RequestForQuotation(Guid id, Guid? agentId, Guid? contactId, Guid? organizationId, string quoteNumber, decimal discount, Status status, string? workSite = null, string? city = null, string? organizationProperty = null, string? contactProperty = null, string? phoneInfo = null, string? mailInfo = null, string? description = null)
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
        }

    }
}