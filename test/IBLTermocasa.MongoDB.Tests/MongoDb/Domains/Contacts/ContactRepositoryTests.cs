using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using IBLTermocasa.Contacts;
using IBLTermocasa.MongoDB;
using Xunit;

namespace IBLTermocasa.MongoDB.Domains.Contacts
{
    [Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
    public class ContactRepositoryTests : IBLTermocasaMongoDbTestBase
    {
        private readonly IContactRepository _contactRepository;

        public ContactRepositoryTests()
        {
            _contactRepository = GetRequiredService<IContactRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _contactRepository.GetListAsync(
                    title: "99d536f9aac6419f90f10729efd0e748ef88d35fce1947db947bdaed201ea5d83608230",
                    name: "c11e0464d96b4dacabbec4543b7c67ff63399c2832a54fc8a84f75b160591",
                    surname: "b044772ce0f14c06920248b51d8d9de0e61985754d854794bb4c1a53482b1150744d616469044722a5b4aa8",
                    confidentialName: "847e0700d8d74699bcad981f09697395a2855ebd2c634d7fbf26c25d561f0837beb739295554479b88e5288859e766",
                    jobRole: "4d9cb06a2b9248558a47f51e64f65eb1fb6f0467f96142",
                    mailInfo: "39c63e8c2ec3413c90507d6fd862e4c0f48",
                    phoneInfo: "2656f5630a7d4c",
                    addressInfo: "d7f4d8fac62442a7bd565849ded93af9c466d65a68c841",
                    tag: "05fdea52fca943e0b9b86820b1"
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("12fa922b-1f43-4071-b46f-804e04828c70"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _contactRepository.GetCountAsync(
                    title: "8bff608e15104467aaaf53791bccddeab31b8faab3d94acd84753ea18",
                    name: "a5f073a",
                    surname: "465232405ac340769f72949caa836cbcc394f01d70984311a096c77f8d248a",
                    confidentialName: "cd93c04aa773440ab07054c968039ee7cb8258d0011a4ee6af9c6c0",
                    jobRole: "3a331aae5a794edaa6e26445a33abff363b0c438399d4c549e527bca42c39849eb6c001c",
                    mailInfo: "5db2345e7052454cac71cc82dda119ec19a3f47a954e41f4b99fe5544bf1efe93a91c72",
                    phoneInfo: "a91c46885c1a4dc195b51acc0cc1d3c31",
                    addressInfo: "76fdc37b09064b5ebba337893413aff2f523a44b542d43069eb1b5f780e111a859c43d72aeaf4b2",
                    tag: "4e674347fd714d9a9e306517e34fe3754465"
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}