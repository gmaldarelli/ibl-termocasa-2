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
            result.Items.Any(x => x.Id == Guid.Parse("20cbea78-9a26-48c0-a0ec-bf6ebd0c8e87")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("6084f99f-8171-40db-b346-1a77b255a697")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _materialsAppService.GetAsync(Guid.Parse("20cbea78-9a26-48c0-a0ec-bf6ebd0c8e87"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("20cbea78-9a26-48c0-a0ec-bf6ebd0c8e87"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new MaterialCreateDto
            {
                Code = "1f3698eb960e4fa4844d27136807ef743466ae82ada14942b07b3f17b5f",
                Name = "981667f10987489bafcf4fb80eb0db144f1c19cf4",
                MeasureUnit = default,
                Quantity = 153014448,
                Lifo = 924138079,
                StandardPrice = 1867170580,
                AveragePrice = 1217707986,
                LastPrice = 878194315,
                AveragePriceSecond = 1115200902,

            };

            // Act
            var serviceResult = await _materialsAppService.CreateAsync(input);

            // Assert
            var result = await _materialRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Code.ShouldBe("1f3698eb960e4fa4844d27136807ef743466ae82ada14942b07b3f17b5f");
            result.Name.ShouldBe("981667f10987489bafcf4fb80eb0db144f1c19cf4");
            result.MeasureUnit.ShouldBe(default);
            result.Quantity.ShouldBe(153014448);
            result.Lifo.ShouldBe(924138079);
            result.StandardPrice.ShouldBe(1867170580);
            result.AveragePrice.ShouldBe(1217707986);
            result.LastPrice.ShouldBe(878194315);
            result.AveragePriceSecond.ShouldBe(1115200902);

        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new MaterialUpdateDto()
            {
                Code = "0b37f51972",
                Name = "d1682138916b4",
                MeasureUnit = default,
                Quantity = 584542307,
                Lifo = 1872311076,
                StandardPrice = 9074098,
                AveragePrice = 839891819,
                LastPrice = 1939515330,
                AveragePriceSecond = 2030872625,

            };

            // Act
            var serviceResult = await _materialsAppService.UpdateAsync(Guid.Parse("20cbea78-9a26-48c0-a0ec-bf6ebd0c8e87"), input);

            // Assert
            var result = await _materialRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Code.ShouldBe("0b37f51972");
            result.Name.ShouldBe("d1682138916b4");
            result.MeasureUnit.ShouldBe(default);
            result.Quantity.ShouldBe(584542307);
            result.Lifo.ShouldBe(1872311076);
            result.StandardPrice.ShouldBe(9074098);
            result.AveragePrice.ShouldBe(839891819);
            result.LastPrice.ShouldBe(1939515330);
            result.AveragePriceSecond.ShouldBe(2030872625);

        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _materialsAppService.DeleteAsync(Guid.Parse("20cbea78-9a26-48c0-a0ec-bf6ebd0c8e87"));

            // Assert
            var result = await _materialRepository.FindAsync(c => c.Id == Guid.Parse("20cbea78-9a26-48c0-a0ec-bf6ebd0c8e87"));

            result.ShouldBeNull();
        }
    }
}