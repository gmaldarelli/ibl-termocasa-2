using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using IBLTermocasa.Materials;
using IBLTermocasa.MongoDB;
using IBLTermocasa.Types;
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
                    code: "07ea10fbd7a1445abafe9be472dc51",
                    name: "6fdedd93112848b98f76e7b552a2a533d0d9e80311f242d8810498b6260090c37adb4a2909354b"
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("20cbea78-9a26-48c0-a0ec-bf6ebd0c8e87"));
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
                    code: "3a9a6e7d66394eeaacdddd94f40a855d83a4294bf9914ac595d6bf9224cf740d3feb49e522a54f49",
                    name: "2230813eb238465ba8288175a792ef6797a5d862034646a78711ec061efb4526b2a8ddc826114cb69bec3b139b9c8fd00a6"
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}