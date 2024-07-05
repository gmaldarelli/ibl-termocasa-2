using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IBLTermocasa.ConsumptionEstimations;
using IBLTermocasa.MongoDB;
using Xunit;

namespace IBLTermocasa.MongoDB.Domains.ConsumptionEstimations
{
    [Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
    public class ConsumptionEstimationRepositoryTests : IBLTermocasaMongoDbTestBase
    {
        private readonly IConsumptionEstimationRepository _consumptionEstimationRepository;

        public ConsumptionEstimationRepositoryTests()
        {
            _consumptionEstimationRepository = GetRequiredService<IConsumptionEstimationRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _consumptionEstimationRepository.GetListAsync(
                    productId: Guid.Parse("ed6b38803c3a418fa1da8622f52e98800e5f67434c694f86a71fd668fd28739506e51")
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("868f7fde-b43b-41b3-b100-009312267a62"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _consumptionEstimationRepository.GetCountAsync(
                    productId: Guid.Parse("ed6b38803c3a418fa1da8622f52e98800e5f67434c694f86a71fd668fd28739506e51")
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}