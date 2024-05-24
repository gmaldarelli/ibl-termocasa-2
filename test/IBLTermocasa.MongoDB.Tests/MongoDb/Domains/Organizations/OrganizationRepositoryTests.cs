using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using IBLTermocasa.Organizations;
using IBLTermocasa.MongoDB;
using Xunit;

namespace IBLTermocasa.MongoDB.Domains.Organizations
{
    [Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
    public class OrganizationRepositoryTests : IBLTermocasaMongoDbTestBase
    {
        private readonly IOrganizationRepository _organizationRepository;

        public OrganizationRepositoryTests()
        {
            _organizationRepository = GetRequiredService<IOrganizationRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _organizationRepository.GetListAsync(
                    code: "378d6069a07f424eb18a81caef33b9ef857ecd91bd7d4674ad590720d6de9470986c",
                    name: "28307bbac6ec4ba7bb172d89183361a96cdd19a9792c40f0960fbdb6726a9235f150419d5209",
                    organizationType: default,
                    mailInfo: "3f8df7ddc51744e580727d5f2edae831d56c5c8fd56d40c18a551508c416904d3c3a05e78ab24b8897f755820",
                    phoneInfo: "17a1a1728bff4273b0bd564ca422a0e40c73b56d0ed14359968d6eb716",
                    socialInfo: "c1f8de9f46d94e0ea51170ed9ab6c5e31d",
                    billingAddress: "a6a04f9e1ea24d",
                    shippingAddress: "b01b64c712eb4e9bab984ee1972c8bd89f00d73f5459412390681776ec287d30e8814a4f3d3c4d",
                    tags: "692a9d871966431ea4cd6bd30815928055c00e6b024e45c6922a6d9cc0885b5ca01532800cc44326b87026b4",
                    notes: "3909d85d38904bd8ac1f1c35f2dad902c0636c82d4fa49e9b",
                    imageId: "4a369936946b42b5a2a98719114bd381c69fa3c9bf6b4f31893c126fa3d26c3662dd62a1b1424f"
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("a88b3013-fe24-4b76-a523-b5c921319218"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _organizationRepository.GetCountAsync(
                    code: "18fe21ea28e240f7b7f0461c1acc126c0651e8fefb184746a8edf81aae926d9a9f160cde5e6f",
                    name: "27e1a9b7cb1142b89248e883fce6044",
                    organizationType: default,
                    mailInfo: "65cddfb8c1124525897c48a5c6de96f7138145f1e2f04993b0b230e6",
                    phoneInfo: "2f8c59ae9a014897ab06841a14e0fe0d52918e8",
                    socialInfo: "c937c4d6cdd74e8d86da600843bbe27a84571fde96654d9f808288e40ed137c4dd07595dc99e4def9c981d",
                    billingAddress: "7ff6ab8da9bf467592a9ca53ce7261fc0f281e6575b842b981c35a5ede4c13447d8daa1a24974",
                    shippingAddress: "7872e5124c67",
                    tags: "85efa623638c47288e83bccfb4b5e72f62957b24fd2d4a2e8a171134673b60e2db3306112",
                    notes: "5140f6cd334644f2bb77254545d9670f65dbb5b10f9f427292faf4d6682b3108025c1e014b25",
                    imageId: "21eff76f220e4a81b7d25f5b669dcb7ff8298648746b46aba1249a9c21f410e7b5"
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}