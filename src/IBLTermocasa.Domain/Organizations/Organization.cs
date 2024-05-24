using IBLTermocasa.Types;
using IBLTermocasa.Industries;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace IBLTermocasa.Organizations
{
    public class Organization : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull]
        public virtual string Code { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        public virtual OrganizationType OrganizationType { get; set; }

        [CanBeNull]
        public virtual string? MailInfo { get; set; }

        [CanBeNull]
        public virtual string? PhoneInfo { get; set; }

        [CanBeNull]
        public virtual string? SocialInfo { get; set; }

        [CanBeNull]
        public virtual string? BillingAddress { get; set; }

        [CanBeNull]
        public virtual string? ShippingAddress { get; set; }

        [CanBeNull]
        public virtual string? Tags { get; set; }

        [CanBeNull]
        public virtual string? Notes { get; set; }

        [CanBeNull]
        public virtual string? ImageId { get; set; }
        public Guid IndustryId { get; set; }

        protected Organization()
        {

        }

        public Organization(Guid id, Guid industryId, string code, string name, OrganizationType organizationType, string? mailInfo = null, string? phoneInfo = null, string? socialInfo = null, string? billingAddress = null, string? shippingAddress = null, string? tags = null, string? notes = null, string? imageId = null)
        {

            Id = id;
            Check.NotNull(code, nameof(code));
            Check.NotNull(name, nameof(name));
            Code = code;
            Name = name;
            OrganizationType = organizationType;
            MailInfo = mailInfo;
            PhoneInfo = phoneInfo;
            SocialInfo = socialInfo;
            BillingAddress = billingAddress;
            ShippingAddress = shippingAddress;
            Tags = tags;
            Notes = notes;
            ImageId = imageId;
            IndustryId = industryId;
        }

    }
}