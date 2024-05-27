using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using IBLTermocasa.Components;
using IBLTermocasa.MongoDB;
using Xunit;

namespace IBLTermocasa.MongoDB.Domains.Components
{
    [Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
    public class ComponentRepositoryTests : IBLTermocasaMongoDbTestBase
    {
        private readonly IComponentRepository _componentRepository;

        public ComponentRepositoryTests()
        {
            _componentRepository = GetRequiredService<IComponentRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _componentRepository.GetListAsync(
                    name: "5216cc1c456a489c830a"
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("93c4cb63-038b-48ba-8d3f-9a16fb6fa8b2"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _componentRepository.GetCountAsync(
                    name: "7e943dde2d0548d2b909b8061fca"
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}