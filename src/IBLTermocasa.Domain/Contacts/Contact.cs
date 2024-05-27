using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IBLTermocasa.Common;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace IBLTermocasa.Contacts
{
    public class Contact : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [CanBeNull] public virtual string? Title { get; set; }

        [NotNull] public virtual string Name { get; set; }

        [NotNull] public virtual string Surname { get; set; }


        [CanBeNull] public virtual string? ConfidentialName { get; set; }

        [CanBeNull] public virtual string? JobRole { get; set; }
        public virtual DateTime? BirthDate { get; set; }
        public Address AddressInfo { get; set; } = new Address();
        public SocialInfo SocialInfo { get; set; } = new SocialInfo();
        public virtual PhoneInfo PhoneInfo { get; set; } = new PhoneInfo();
        public virtual MailInfo MailInfo { get; set; } = new MailInfo();
        public List<string> Tags { get; set; }
        public Guid? ImageId { get; set; }
        public string? Notes { get; set; }


        protected Contact()
        {
        }


        // public Constructor with all the properties extracted TenantId
        public Contact(Guid id, string name, string surname, string? title, string? confidentialName,
            string? jobRole, DateTime? birthDate, Address addressInfo, SocialInfo socialInfo, PhoneInfo phoneInfo,
            MailInfo mailInfo, List<string> tags, Guid? imageId, string? notes)
        {
            Id = id;
            Check.NotNull(name, nameof(name));
            Check.NotNull(surname, nameof(surname));
            Name = name;
            Surname = surname;
            Title = title;
            ConfidentialName = confidentialName;
            JobRole = jobRole;
            BirthDate = birthDate;
            AddressInfo = addressInfo;
            SocialInfo = socialInfo;
            PhoneInfo = phoneInfo;
            MailInfo = mailInfo;
            Tags = tags;
            ImageId = imageId;
            Notes = notes;
        }
        public static Contact FillProperties(Contact source, Contact destination, IEnumerable<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                var sourceValue = property.GetValue(source);
                property.SetValue(destination, sourceValue);
            }

            return destination;
        }
        public static Contact FillPropertiesForInsert(Contact source, Contact destination)
        {
            var properties = typeof(Contact).GetProperties()
                .Where(p => p.CanRead && p.CanWrite && p.Name != "Id" && p.Name != "ConcurrencyStamp");
            return FillProperties(source, destination, properties);
        }
        public static Contact FillPropertiesForUpdate(Contact source, Contact destination)
        {
            var properties = typeof(Contact).GetProperties()
                .Where(p => p.CanRead && p.CanWrite && p.Name != "Id");
            return FillProperties(source, destination, properties);
        }
    }
}