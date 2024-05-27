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
                    title: "0d5d3725faa6437199a7d4bfe06e34fcf67489f93fa64732903df0ed2e2136f7e9c403b5d6ec489ab390109",
                    name: "0ec009b9fc2d4162",
                    surname: "466327e3c1374e04a598e751a2cde5ee7ebfdf647",
                    confidentialName: "15fc122b14504c88beee692a004084eced394041394849b588defef191cdee69dcd35e2",
                    jobRole: "11ef673d532c41e386a1ed5ad3030408741a341c4a6b4f9a8eb2a5b",
                    mailInfo: "505780442fc14722844788d85ceb34ef20a384104ca2427dbb872c48be54d974ca667dfefd1b4c5790a",
                    phoneInfo: "e1c5382999984c26b907986a6521e132c99d9c10c8ff44b3858d4c2",
                    addressInfo: "04e99bfed70748c1bb3f39ab412fa939285e5f5730bf49e6b4d7faad04d341bc2057f1b073db4385a37c0",
                    tag: "2f7f147bf2f041e88e391890460dde828f3ea592423"
                    );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("fd0a8d18-5a5f-4b09-8cff-37b020c1ac25"));
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
                    filterText: "d3f7604eda4d469783440b6cb9a455a48c9aef6035c",
                    title: "32c10e8f1f59",
                    name: "d3f7604eda4d469783440b6cb9a455a48c9aef6035c",
                    surname: "36cbdcd391f342d589c5c4628cde9a5069d03fe50f5140d583",
                    confidentialName: "11eecfec066748e9bdf802e36ed14e29d61e523567fd40afa5d5f4a07658591ec825188fb4a745be",
                    jobRole: "0a2054cf6dbf48f2b750a8814c0f818cf8115eaea5ef408983ec1b0",
                    mailInfo: "fdc21631c6f04b228638dc2ec8673edfb06d14b214334879",
                    phoneInfo: "1d1f10c",
                    addressInfo: "9020feab2cfb4fba833eb0401ea139bc2dba2f4373e34eb586e534a1b945abe4a3a2241933a14d32b42983d",
                    tag: "7c5c7b0d25ef45d59439024c11230c9097a5f8326e"
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}