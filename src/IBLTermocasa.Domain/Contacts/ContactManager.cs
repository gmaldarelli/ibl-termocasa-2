using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace IBLTermocasa.Contacts
{
    public class ContactManager : DomainService
    {
        protected IContactRepository _contactRepository;

        public ContactManager(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public virtual async Task<Contact> CreateAsync(
        string name, string surname, DateTime birthDate, string? title = null, string? confidentialName = null, string? jobRole = null, string? mailInfo = null, string? phoneInfo = null, string? socialInfo = null, string? addressInfo = null, string? tag = null, string? notes = null)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNullOrWhiteSpace(surname, nameof(surname));
            Check.NotNull(birthDate, nameof(birthDate));

            var contact = new Contact(
             GuidGenerator.Create(),
             name, surname, birthDate, title, confidentialName, jobRole, mailInfo, phoneInfo, socialInfo, addressInfo, tag, notes
             );

            return await _contactRepository.InsertAsync(contact);
        }

        public virtual async Task<Contact> UpdateAsync(
            Guid id,
            string name, string surname, DateTime birthDate, string? title = null, string? confidentialName = null, string? jobRole = null, string? mailInfo = null, string? phoneInfo = null, string? socialInfo = null, string? addressInfo = null, string? tag = null, string? notes = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));
            Check.NotNullOrWhiteSpace(surname, nameof(surname));
            Check.NotNull(birthDate, nameof(birthDate));

            var contact = await _contactRepository.GetAsync(id);

            contact.Name = name;
            contact.Surname = surname;
            contact.BirthDate = birthDate;
            contact.Title = title;
            contact.ConfidentialName = confidentialName;
            contact.JobRole = jobRole;
            contact.MailInfo = mailInfo;
            contact.PhoneInfo = phoneInfo;
            contact.SocialInfo = socialInfo;
            contact.AddressInfo = addressInfo;
            contact.Tag = tag;
            contact.Notes = notes;

            contact.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _contactRepository.UpdateAsync(contact);
        }

    }
}