using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IBLTermocasa.Common;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using IBLTermocasa.Contacts;

namespace IBLTermocasa.Contacts
{
    public class ContactsDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly IContactRepository _contactRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ContactsDataSeedContributor(IContactRepository contactRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _contactRepository = contactRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _contactRepository.InsertAsync(new Contact
            (
                id: Guid.Parse("fd0a8d18-5a5f-4b09-8cff-37b020c1ac25"),
                title: "0d5d3725faa6437199a7d4bfe06e34fcf67489f93fa64732903df0ed2e2136f7e9c403b5d6ec489ab390109",
                name: "0ec009b9fc2d4162",
                surname: "466327e3c1374e04a598e751a2cde5ee7ebfdf647",
                confidentialName: "15fc122b14504c88beee692a004084eced394041394849b588defef191cdee69dcd35e2",
                jobRole: "11ef673d532c41e386a1ed5ad3030408741a341c4a6b4f9a8eb2a5b",
                birthDate: new DateTime(2021, 8, 13),
                mailInfo:  new MailInfo()
                {
                    MailItems = new List<MailItem>()
                    {
                        new MailItem(){Email = "maill@sample.com", Type = MailType.OTHER},
                    }
                },
                phoneInfo: new PhoneInfo()
                {
                    PhoneItems = new List<PhoneItem>()
                    {
                        new PhoneItem(){Number = "123456789", Type = PhoneType.OTHER},
                    }
                },
                socialInfo: new SocialInfo()
                {
                    SocialItems = new List<SocialItem>()
                    {
                        new SocialItem(){Url = "social", SocialType = SocialType.OTHER},
                    }
                },
                addressInfo: new Address()
                {
                    City = "City",
                    Country = "Country",
                    PostalCode = "PostalCode",
                    Street = "Street",
                    State = "State",
                    AdditionalInfo = "AdditionalInfo"
                },
                tags: new List<string> { "f4c", "f4c", "f4c" },
                imageId: Guid.Parse("f4c4c26a1b8944a7a9f19d741af7e8ce1c1f84114a096415093a2e1fb8320ef258580dbfba8ec48988f4403"),
                notes: "8c4c26a1b8944a7a9f19d741af7e8ce1c1f84114a096415093a2e1fb8320ef258580dbfba8ec48988f4403"
            ));

            await _contactRepository.InsertAsync(new Contact
            (
                id: Guid.Parse("93b144f3-4e0b-4658-b107-ed134e5b04a3"),
                title: "32c10e8f1f59",
                name: "d3f7604eda4d469783440b6cb9a455a48c9aef6035c",
                surname: "36cbdcd391f342d589c5c4628cde9a5069d03fe50f5140d583",
                confidentialName: "11eecfec066748e9bdf802e36ed14e29d61e523567fd40afa5d5f4a07658591ec825188fb4a745be",
                jobRole: "0a2054cf6dbf48f2b750a8814c0f818cf8115eaea5ef408983ec1b0",
                birthDate: new DateTime(2009, 2, 22),
                mailInfo:  new MailInfo()
                {
                    MailItems = new List<MailItem>()
                    {
                        new MailItem(){Email = "maill@sample.com", Type = MailType.OTHER},
                    }
                },
                phoneInfo: new PhoneInfo()
                {
                    PhoneItems = new List<PhoneItem>()
                    {
                        new PhoneItem(){Number = "123456789", Type = PhoneType.OTHER},
                    }
                },
                socialInfo: new SocialInfo()
                {
                    SocialItems = new List<SocialItem>()
                    {
                        new SocialItem(){Url = "social", SocialType = SocialType.OTHER},
                    }
                },
                addressInfo: new Address()
                {
                    City = "City",
                    Country = "Country",
                    PostalCode = "PostalCode",
                    Street = "Street",
                    State = "State",
                    AdditionalInfo = "AdditionalInfo"
                },
                tags: new List<string>() {"f4c", "f4c", "f4c"},
                imageId: Guid.Parse("f4c4c26a1b8944a7a9f19d741af7e8ce1c1f84114a096415093a2e1fb8320ef258580dbfba8ec48988f4403"),
                notes: "aacd33806d1747b08728e213181abbac6bf6f39fd9"
            ));

            await _unitOfWorkManager!.Current!.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}