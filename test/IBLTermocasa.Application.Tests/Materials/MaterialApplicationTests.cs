using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace IBLTermocasa.Materials
{
    public abstract class MaterialsAppServiceTests<TStartupModule> : IBLTermocasaApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IMaterialsAppService _materialsAppService;
        private readonly IRepository<Material, Guid> _materialRepository;

        public MaterialsAppServiceTests()
        {
            _materialsAppService = GetRequiredService<IMaterialsAppService>();
            _materialRepository = GetRequiredService<IRepository<Material, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _materialsAppService.GetListAsync(new GetMaterialsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("7a33ea5e-e9f7-473e-85b2-78738b1f8e7c")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("91b4cf12-71b6-4e07-934c-4c3f72cbf91e")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _materialsAppService.GetAsync(Guid.Parse("7a33ea5e-e9f7-473e-85b2-78738b1f8e7c"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("7a33ea5e-e9f7-473e-85b2-78738b1f8e7c"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new MaterialCreateDto
            {
                Code = "12545b98d9a542c38b797344ebe31418dc7ccd278d3849b7914bee6263652243e8000ea464944f2eba73f7355c7",
                Name = "e1e18a7097464e6285880b19b4b0785aa9f45ad570d943ee86f3eecfdfbecd6650c019b8a6024b6d8abfa7454d2de0",
                MeasureUnit = default,
                Quantity = 116406908,
                Lifo = 1534413796,
                StandardPrice = 1192151445,
                AveragePrice = 1321208382,
                LastPrice = 387059227,
                AveragePriceSecond = 1526351810
            };

            // Act
            var serviceResult = await _materialsAppService.CreateAsync(input);

            // Assert
            var result = await _materialRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Code.ShouldBe("12545b98d9a542c38b797344ebe31418dc7ccd278d3849b7914bee6263652243e8000ea464944f2eba73f7355c7");
            result.Name.ShouldBe("e1e18a7097464e6285880b19b4b0785aa9f45ad570d943ee86f3eecfdfbecd6650c019b8a6024b6d8abfa7454d2de0");
            result.MeasureUnit.ShouldBe(default);
            result.Quantity.ShouldBe(116406908);
            result.Lifo.ShouldBe(1534413796);
            result.StandardPrice.ShouldBe(1192151445);
            result.AveragePrice.ShouldBe(1321208382);
            result.LastPrice.ShouldBe(387059227);
            result.AveragePriceSecond.ShouldBe(1526351810);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new MaterialUpdateDto()
            {
                Code = "d884dc640bbe48139c7f6010f82b7cb6b1bedff3e80946e98f06a6b302c0fc898180714d922a404f87b680a18",
                Name = "e2123546f40f4d3d8256b0957a16eed88b189842d",
                MeasureUnit = default,
                Quantity = 1228622338,
                Lifo = 1191447531,
                StandardPrice = 1632640114,
                AveragePrice = 1342830231,
                LastPrice = 841408058,
                AveragePriceSecond = 908368917
            };

            // Act
            var serviceResult = await _materialsAppService.UpdateAsync(Guid.Parse("7a33ea5e-e9f7-473e-85b2-78738b1f8e7c"), input);

            // Assert
            var result = await _materialRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Code.ShouldBe("d884dc640bbe48139c7f6010f82b7cb6b1bedff3e80946e98f06a6b302c0fc898180714d922a404f87b680a18");
            result.Name.ShouldBe("e2123546f40f4d3d8256b0957a16eed88b189842d");
            result.MeasureUnit.ShouldBe(default);
            result.Quantity.ShouldBe(1228622338);
            result.Lifo.ShouldBe(1191447531);
            result.StandardPrice.ShouldBe(1632640114);
            result.AveragePrice.ShouldBe(1342830231);
            result.LastPrice.ShouldBe(841408058);
            result.AveragePriceSecond.ShouldBe(908368917);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _materialsAppService.DeleteAsync(Guid.Parse("7a33ea5e-e9f7-473e-85b2-78738b1f8e7c"));

            // Assert
            var result = await _materialRepository.FindAsync(c => c.Id == Guid.Parse("7a33ea5e-e9f7-473e-85b2-78738b1f8e7c"));

            result.ShouldBeNull();
        }
    }
}