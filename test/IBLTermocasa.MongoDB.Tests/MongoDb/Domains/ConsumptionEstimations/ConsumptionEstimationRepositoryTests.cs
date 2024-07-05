using Shouldly;
using System;
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
                    consumptionProduct: "df77227afb4249b9b6af520810481f",
                    consumptionWork: "5088cd37f1a241b08108f9d9fb5097ceccca5cb0280d4f39a2fb177f208c72587be8c553d48149cab1f38e"
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
                    consumptionProduct: "da0e467a83cc4bd3aa0b75",
                    consumptionWork: "1c68d608a39c4b48ac67c9665d3dbce9b293ad9740a8"
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}