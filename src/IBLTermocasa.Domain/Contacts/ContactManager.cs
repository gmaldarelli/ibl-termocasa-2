using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace IBLTermocasa.Contacts
{
    public class ContactManager : DomainService
    {
        protected IContactRepository _contactRepository;

        public ContactManager(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public virtual async Task<Contact> CreateAsync(Contact contact)
        {
            Check.NotNull(contact, nameof(contact));
            return await _contactRepository.InsertAsync(contact);
        }
        public virtual async Task<Contact> UpdateAsync(Guid id, Contact contact)
        {
            Check.NotNull(contact, nameof(contact));
            var existingContact = await _contactRepository.GetAsync(id);
            Contact.FillPropertiesForUpdate(contact, existingContact);
            return await _contactRepository.UpdateAsync(existingContact);
        }

    }
}