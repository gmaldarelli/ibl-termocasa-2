using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace IBLTermocasa.Contacts
{
    public abstract class ContactBase : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [CanBeNull]
        public virtual string? Title { get; set; }

        [NotNull]
        public virtual string Name { get; set; }

        [NotNull]
        public virtual string Surname { get; set; }

        [CanBeNull]
        public virtual string? ConfidentialName { get; set; }

        [CanBeNull]
        public virtual string? JobRole { get; set; }

        public virtual DateTime BirthDate { get; set; }

        [CanBeNull]
        public virtual string? MailInfo { get; set; }

        [CanBeNull]
        public virtual string? PhoneInfo { get; set; }

        [CanBeNull]
        public virtual string? SocialInfo { get; set; }

        [CanBeNull]
        public virtual string? AddressInfo { get; set; }

        [CanBeNull]
        public virtual string? Tag { get; set; }

        [CanBeNull]
        public virtual string? Notes { get; set; }

        [CanBeNull]
        public virtual string? ImageId { get; set; }

        protected ContactBase()
        {

        }

        public ContactBase(Guid id, string name, string surname, DateTime birthDate, string? title = null, string? confidentialName = null, string? jobRole = null, string? mailInfo = null, string? phoneInfo = null, string? socialInfo = null, string? addressInfo = null, string? tag = null, string? notes = null)
        {

            Id = id;
            Check.NotNull(name, nameof(name));
            Check.NotNull(surname, nameof(surname));
            Name = name;
            Surname = surname;
            BirthDate = birthDate;
            Title = title;
            ConfidentialName = confidentialName;
            JobRole = jobRole;
            MailInfo = mailInfo;
            PhoneInfo = phoneInfo;
            SocialInfo = socialInfo;
            AddressInfo = addressInfo;
            Tag = tag;
            Notes = notes;
        }

    }
}