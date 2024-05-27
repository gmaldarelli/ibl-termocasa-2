using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using IBLTermocasa.Catalogs;
using IBLTermocasa.MongoDB;
using Xunit;

namespace IBLTermocasa.MongoDB.Domains.Catalogs
{
    [Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
    public class CatalogRepositoryTests : IBLTermocasaMongoDbTestBase
    {
        private readonly ICatalogRepository _catalogRepository;

        public CatalogRepositoryTests()
        {
            _catalogRepository = GetRequiredService<ICatalogRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _catalogRepository.GetListAsync(
                    name: "f9dc259290a243719498be978cf87f351aed289af0fd429b967607",
                    description: "d730f25de1af4afab481dd"
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("ae5a5088-4847-445c-b018-0145ccc4a842"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _catalogRepository.GetCountAsync(
                    name: "468ee521986645b7b78a03fff9b6b4902",
                    description: "6747042f63a446359552618cffa9a70b6289eb46cc0543c08d497"
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}