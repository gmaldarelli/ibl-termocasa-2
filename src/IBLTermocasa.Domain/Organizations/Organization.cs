using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IBLTermocasa.Common;
using IBLTermocasa.Contacts;
using IBLTermocasa.Types;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace IBLTermocasa.Organizations
{
    public class Organization : FullAuditedAggregateRoot<Guid>, IMultiTenant
    {
        public virtual Guid? TenantId { get; set; }

        [NotNull] public virtual string Code { get; set; }

        [NotNull] public virtual string Name { get; set; }

        public virtual OrganizationType OrganizationType { get; set; }

        public Guid IndustryId { get; set; }
        public List<ContactProperty> ListContacts { get; set; } = new();

        [CanBeNull] public virtual string? Notes { get; set; }

        [CanBeNull] public virtual Guid? ImageId { get; set; }

        public Address? ShippingAddress { get; set; } = new Address();
        public Address? BillingAddress { get; set; } = new Address();
        public SocialInfo SocialInfo { get; set; } = new SocialInfo();
        public virtual PhoneInfo PhoneInfo { get; set; } = new PhoneInfo();
        public virtual MailInfo MailInfo { get; set; } = new MailInfo();
        public List<string> Tags { get; set; }
        
        public virtual SourceType SourceType { get; set; }

        public virtual DateTime? FirstSync { get; set; }

        public virtual DateTime? LastSync { get; set; }

        public Organization()
        {
        }

        public Organization(Guid id)
        {
            Id = id;
        }

        public Organization(Guid id, string name, Guid industryId, List<ContactProperty> listContacts, OrganizationType organizationType,
            Address shippingAddress, Address billingAddress, SocialInfo socialInfo, PhoneInfo phoneInfo,
            MailInfo mailInfo, List<string> tags, Guid? imageId, string notes)
        {
            Id = id;
            Check.NotNull(name, nameof(name));
            Name = name;
            IndustryId = industryId;
            ListContacts = listContacts;
            OrganizationType = organizationType;
            ShippingAddress = shippingAddress;
            BillingAddress = billingAddress;
            SocialInfo = socialInfo;
            PhoneInfo = phoneInfo;
            MailInfo = mailInfo;
            Tags = tags;
            ImageId = imageId;
            Notes = notes;
        }

        public Organization(Guid id, string name, Guid industryId, List<ContactProperty> listContacts, OrganizationType organizationType,
            Address shippingAddress, Address billingAddress, SocialInfo socialInfo, PhoneInfo phoneInfo,
            MailInfo mailInfo, List<string> tags, Guid? imageId, string notes,
            SourceType sourceType, DateTime? firstSync, DateTime? lastSync)
        {
            Id = id;
            Check.NotNull(name, nameof(name));
            Name = name;
            IndustryId = industryId;
            ListContacts = listContacts;
            OrganizationType = organizationType;
            ShippingAddress = shippingAddress;
            BillingAddress = billingAddress;
            SocialInfo = socialInfo;
            PhoneInfo = phoneInfo;
            MailInfo = mailInfo;
            Tags = tags;
            ImageId = imageId;
            Notes = notes;
            SourceType = sourceType;
            FirstSync = firstSync;
            LastSync = lastSync;
        }
        
        
        //generate static method to fill all properties of the Organization except the Id using reflection with 2 variants source and destination
        public static Organization FillProperties(Organization source, Organization destination,
            IEnumerable<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                if (property.Name == "Contacts")
                {
                    // Handle the Contacts property separately
                    var sourceContacts = (List<Contact>)property.GetValue(source) ?? new List<Contact>();
                    var destinationContacts = (List<Contact>)property.GetValue(destination) ?? new List<Contact>();

                    foreach (var contact in sourceContacts)
                    {
                        var existingContact = destinationContacts.FirstOrDefault(c => c.Id == contact.Id);
                        if (existingContact != null)
                        {
                            // Update existing contact
                            Contact.FillProperties(contact, existingContact, typeof(Contact).GetProperties());
                        }
                        else
                        {
                            // Add new contact
                            destinationContacts.Add(contact);
                        }
                    }
                    property.SetValue(destination, destinationContacts);
                }
                else
                {
                    var sourceValue = property.GetValue(source);
                    property.SetValue(destination, sourceValue);
                }
            }

            return destination;
        }

        public static Organization FillPropertiesForInsert(Organization source, Organization destination)
        {
            var properties = typeof(Organization).GetProperties()
                .Where(p => p.CanRead && p.CanWrite && p.Name != "Id" && p.Name != "ConcurrencyStamp");
            return FillProperties(source, destination, properties);
        }

        public static Organization FillPropertiesForUpdate(Organization source, Organization destination)
        {
            var properties = typeof(Organization).GetProperties()
                .Where(p => p.CanRead && p.CanWrite && p.Name != "Id");
            return FillProperties(source, destination, properties);
        }
    }
}