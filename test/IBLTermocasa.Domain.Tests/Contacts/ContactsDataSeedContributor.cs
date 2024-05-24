using System;
using System.Threading.Tasks;
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
                id: Guid.Parse("12fa922b-1f43-4071-b46f-804e04828c70"),
                title: "99d536f9aac6419f90f10729efd0e748ef88d35fce1947db947bdaed201ea5d83608230",
                name: "c11e0464d96b4dacabbec4543b7c67ff63399c2832a54fc8a84f75b160591",
                surname: "b044772ce0f14c06920248b51d8d9de0e61985754d854794bb4c1a53482b1150744d616469044722a5b4aa8",
                confidentialName: "847e0700d8d74699bcad981f09697395a2855ebd2c634d7fbf26c25d561f0837beb739295554479b88e5288859e766",
                jobRole: "4d9cb06a2b9248558a47f51e64f65eb1fb6f0467f96142",
                birthDate: new DateTime(2005, 3, 7),
                mailInfo: "39c63e8c2ec3413c90507d6fd862e4c0f48",
                phoneInfo: "2656f5630a7d4c",
                socialInfo: "29c0c0b1fcb2417dbe50d0f85762464436f97e75224c4e1ba75789ae274147f16163bb006fe646b8923cbf6634455e",
                addressInfo: "d7f4d8fac62442a7bd565849ded93af9c466d65a68c841",
                tag: "05fdea52fca943e0b9b86820b1",
                notes: "a7fc332f15b34fae8f0a18bc929fddf3807"
            ));

            await _contactRepository.InsertAsync(new Contact
            (
                id: Guid.Parse("6447913c-a85f-4df5-9c00-7c97e6bf76ed"),
                title: "8bff608e15104467aaaf53791bccddeab31b8faab3d94acd84753ea18",
                name: "a5f073a",
                surname: "465232405ac340769f72949caa836cbcc394f01d70984311a096c77f8d248a",
                confidentialName: "cd93c04aa773440ab07054c968039ee7cb8258d0011a4ee6af9c6c0",
                jobRole: "3a331aae5a794edaa6e26445a33abff363b0c438399d4c549e527bca42c39849eb6c001c",
                birthDate: new DateTime(2009, 7, 10),
                mailInfo: "5db2345e7052454cac71cc82dda119ec19a3f47a954e41f4b99fe5544bf1efe93a91c72",
                phoneInfo: "a91c46885c1a4dc195b51acc0cc1d3c31",
                socialInfo: "5edabdba75b3446898c5367ff7d8046c0e5253d0bf1a4cd2bf0444df7c1fbffc",
                addressInfo: "76fdc37b09064b5ebba337893413aff2f523a44b542d43069eb1b5f780e111a859c43d72aeaf4b2",
                tag: "4e674347fd714d9a9e306517e34fe3754465",
                notes: "d4623953cdc24dc3bb2be6576b17c81c3d4561e1884045d4a84d8d898cceccd43f64cfbcfc874"
            ));

            await _unitOfWorkManager!.Current!.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}