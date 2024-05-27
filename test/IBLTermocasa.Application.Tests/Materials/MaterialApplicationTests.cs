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
            result.Items.Any(x => x.Id == Guid.Parse("113ff063-4359-4c2b-9270-fc257e4c6258")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("92cd78ea-760e-4c73-a1da-810a1acf28d6")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _materialsAppService.GetAsync(Guid.Parse("113ff063-4359-4c2b-9270-fc257e4c6258"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("113ff063-4359-4c2b-9270-fc257e4c6258"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new MaterialCreateDto
            {
                Code = "56d9302ca4374bfbbe797888833059",
                Name = "20feb4ac13654cbaba6aae7e8551",
                MeasureUnit = default,
                Quantity = 1913082733,
                Lifo = 2066389345,
                StandardPrice = 208518872,
                AveragePrice = 1912707961,
                LastPrice = 1384571396,
                AveragePriceSecond = 1469382445
            };

            // Act
            var serviceResult = await _materialsAppService.CreateAsync(input);

            // Assert
            var result = await _materialRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Code.ShouldBe("56d9302ca4374bfbbe797888833059");
            result.Name.ShouldBe("20feb4ac13654cbaba6aae7e8551");
            result.MeasureUnit.ShouldBe(default);
            result.Quantity.ShouldBe(1913082733);
            result.Lifo.ShouldBe(2066389345);
            result.StandardPrice.ShouldBe(208518872);
            result.AveragePrice.ShouldBe(1912707961);
            result.LastPrice.ShouldBe(1384571396);
            result.AveragePriceSecond.ShouldBe(1469382445);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new MaterialUpdateDto()
            {
                Code = "aa45b9e99eb642999e2accf8e845486fa38d850097d44820b8b",
                Name = "709250bc69b3488fb60965eb64515f367709190e967e4045983471800cd09ed1443b3937b6f",
                MeasureUnit = default,
                Quantity = 1457560087,
                Lifo = 1697767489,
                StandardPrice = 1858443576,
                AveragePrice = 760654440,
                LastPrice = 1216301084,
                AveragePriceSecond = 78741786
            };

            // Act
            var serviceResult = await _materialsAppService.UpdateAsync(Guid.Parse("113ff063-4359-4c2b-9270-fc257e4c6258"), input);

            // Assert
            var result = await _materialRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Code.ShouldBe("aa45b9e99eb642999e2accf8e845486fa38d850097d44820b8b");
            result.Name.ShouldBe("709250bc69b3488fb60965eb64515f367709190e967e4045983471800cd09ed1443b3937b6f");
            result.MeasureUnit.ShouldBe(default);
            result.Quantity.ShouldBe(1457560087);
            result.Lifo.ShouldBe(1697767489);
            result.StandardPrice.ShouldBe(1858443576);
            result.AveragePrice.ShouldBe(760654440);
            result.LastPrice.ShouldBe(1216301084);
            result.AveragePriceSecond.ShouldBe(78741786);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _materialsAppService.DeleteAsync(Guid.Parse("113ff063-4359-4c2b-9270-fc257e4c6258"));

            // Assert
            var result = await _materialRepository.FindAsync(c => c.Id == Guid.Parse("113ff063-4359-4c2b-9270-fc257e4c6258"));

            result.ShouldBeNull();
        }
    }
}