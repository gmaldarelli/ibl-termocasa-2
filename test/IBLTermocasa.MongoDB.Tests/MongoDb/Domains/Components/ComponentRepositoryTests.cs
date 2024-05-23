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
                    name: "b8b5b7d130954994afc0877be341ab33ed1e8a6306704f"
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("a277f0ce-c5ed-4957-9b49-9cdc39c6bb90"));
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
                    name: "6a6a917e366d4221b16"
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}