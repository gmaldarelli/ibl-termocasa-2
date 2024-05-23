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
                    code: "860a3bc448c048b",
                    name: "16b08a8b23e94fbb920af5cbc4eea81e4098c6db820b400a85e6becb3"
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("7a33ea5e-e9f7-473e-85b2-78738b1f8e7c"));
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
                    code: "3c399c1ab1a8411ebb1bc3ca08f6dfb21e5c7ce1079b4d538e456db51766b56e69d51632eebf4400a",
                    name: "2e4744027935412ea1ac54e6e32447a6a121809719dc40dea1");

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}