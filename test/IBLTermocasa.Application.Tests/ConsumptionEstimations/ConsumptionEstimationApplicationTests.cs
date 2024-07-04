using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace IBLTermocasa.ConsumptionEstimations
{
    public abstract class ConsumptionEstimationsAppServiceTests<TStartupModule> : IBLTermocasaApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IConsumptionEstimationsAppService _consumptionEstimationsAppService;
        private readonly IRepository<ConsumptionEstimation, Guid> _consumptionEstimationRepository;

        public ConsumptionEstimationsAppServiceTests()
        {
            _consumptionEstimationsAppService = GetRequiredService<IConsumptionEstimationsAppService>();
            _consumptionEstimationRepository = GetRequiredService<IRepository<ConsumptionEstimation, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _consumptionEstimationsAppService.GetListAsync(new GetConsumptionEstimationsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("868f7fde-b43b-41b3-b100-009312267a62")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("4d3f667a-a8ac-416e-8290-a4cac7c1863b")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _consumptionEstimationsAppService.GetAsync(Guid.Parse("868f7fde-b43b-41b3-b100-009312267a62"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("868f7fde-b43b-41b3-b100-009312267a62"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new ConsumptionEstimationCreateDto
            {
                ConsumptionProduct = "ed6b38803c3a418fa1da8622f52e98800e5f67434c694f86a71fd668fd28739506e51",
                ConsumptionWork = "795497c686f74cd7ba03d46d5f"
            };

            // Act
            var serviceResult = await _consumptionEstimationsAppService.CreateAsync(input);

            // Assert
            var result = await _consumptionEstimationRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.ConsumptionProduct.ShouldBe("ed6b38803c3a418fa1da8622f52e98800e5f67434c694f86a71fd668fd28739506e51");
            result.ConsumptionWork.ShouldBe("795497c686f74cd7ba03d46d5f");
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new ConsumptionEstimationUpdateDto()
            {
                ConsumptionProduct = "b9e58408d19144cfa95c7cb831",
                ConsumptionWork = "2b1e38becc0342f39539f9b70"
            };

            // Act
            var serviceResult = await _consumptionEstimationsAppService.UpdateAsync(Guid.Parse("868f7fde-b43b-41b3-b100-009312267a62"), input);

            // Assert
            var result = await _consumptionEstimationRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.ConsumptionProduct.ShouldBe("b9e58408d19144cfa95c7cb831");
            result.ConsumptionWork.ShouldBe("2b1e38becc0342f39539f9b70");
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _consumptionEstimationsAppService.DeleteAsync(Guid.Parse("868f7fde-b43b-41b3-b100-009312267a62"));

            // Assert
            var result = await _consumptionEstimationRepository.FindAsync(c => c.Id == Guid.Parse("868f7fde-b43b-41b3-b100-009312267a62"));

            result.ShouldBeNull();
        }
    }
}