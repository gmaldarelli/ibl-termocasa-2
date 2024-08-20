using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace IBLTermocasa.Catalogs
{
    public abstract class CatalogsAppServiceTests<TStartupModule> : IBLTermocasaApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly ICatalogsAppService _catalogsAppService;
        private readonly IRepository<Catalog, Guid> _catalogRepository;

        public CatalogsAppServiceTests()
        {
            _catalogsAppService = GetRequiredService<ICatalogsAppService>();
            _catalogRepository = GetRequiredService<IRepository<Catalog, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _catalogsAppService.GetListAsync(new GetCatalogsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("ae5a5088-4847-445c-b018-0145ccc4a842")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("edd79e7d-3dc7-4b25-9ccd-1a97cb1a6c38")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _catalogsAppService.GetAsync(Guid.Parse("ae5a5088-4847-445c-b018-0145ccc4a842"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("ae5a5088-4847-445c-b018-0145ccc4a842"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new CatalogCreateDto
            {
                Name = "317c923112bf460f91567d1584937db1808de10c8a5b42a1b1",
                From = new DateTime(2006, 3, 14),
                To = new DateTime(2006, 8, 25),
                Description = "f4e26fa830244375a051f597579f041a0eb22978a5284684b92771abb58cc253e814a7cf41ca47149bc4ce172ffda98d76"
            };

            // Act
            var serviceResult = await _catalogsAppService.CreateAsync(input);

            // Assert
            var result = await _catalogRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe("317c923112bf460f91567d1584937db1808de10c8a5b42a1b1");
            result.From.ShouldBe(new DateTime(2006, 3, 14));
            result.To.ShouldBe(new DateTime(2006, 8, 25));
            result.Description.ShouldBe("f4e26fa830244375a051f597579f041a0eb22978a5284684b92771abb58cc253e814a7cf41ca47149bc4ce172ffda98d76");
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new CatalogUpdateDto()
            {
                Name = "8b9cf4e3f0794fb7a6227a95",
                From = new DateTime(2000, 6, 24),
                To = new DateTime(2016, 6, 19),
                Description = "51cb8d36fa934d2"
            };

            // Act
            var serviceResult = await _catalogsAppService.UpdateAsync(Guid.Parse("ae5a5088-4847-445c-b018-0145ccc4a842"), input);

            // Assert
            var result = await _catalogRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe("8b9cf4e3f0794fb7a6227a95");
            result.From.ShouldBe(new DateTime(2000, 6, 24));
            result.To.ShouldBe(new DateTime(2016, 6, 19));
            result.Description.ShouldBe("51cb8d36fa934d2");
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _catalogsAppService.DeleteAsync(Guid.Parse("ae5a5088-4847-445c-b018-0145ccc4a842"));

            // Assert
            var result = await _catalogRepository.FindAsync(c => c.Id == Guid.Parse("ae5a5088-4847-445c-b018-0145ccc4a842"));

            result.ShouldBeNull();
        }
    }
}