using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using IBLTermocasa.Materials;
using IBLTermocasa.MongoDB;
using Xunit;

namespace IBLTermocasa.MongoDB.Domains.Materials
{
    [Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
    public class MaterialRepositoryTests : IBLTermocasaMongoDbTestBase
    {
        private readonly IMaterialRepository _materialRepository;

        public MaterialRepositoryTests()
        {
            _materialRepository = GetRequiredService<IMaterialRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _materialRepository.GetListAsync(
                    code: "14395ed627fa4c48a2ab9c7d5f88a21f36d4c28",
                    name: "acea513193b9400fbc1ebec968e57"
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("113ff063-4359-4c2b-9270-fc257e4c6258"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _materialRepository.GetCountAsync(
                    code: "78efb9fd003b4825a2c2057ec45cb2c79fe23f08c1",
                    name: "86c447606e1f48fba8629229a9b78d2507c8deabad584e2fb9f12e1bde2"
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}