using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using IBLTermocasa.Quotations;
using IBLTermocasa.MongoDB;
using Xunit;

namespace IBLTermocasa.MongoDB.Domains.Quotations
{
    [Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
    public class QuotationRepositoryTests : IBLTermocasaMongoDbTestBase
    {
        private readonly IQuotationRepository _quotationRepository;

        public QuotationRepositoryTests()
        {
            _quotationRepository = GetRequiredService<IQuotationRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _quotationRepository.GetListAsync(
                    code: "83dbc9bb41704e1e9c6514c599e24938971ad25f82a34e28",
                    name: "708d2b1e0a9c4c69b0ea1ac76bb284856619176f52b0468781cae46b7f27a0dd9bf15f6e74544b2e9",
                    status: default,
                    depositRequired: true
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("cfc6d5db-1312-4d3b-9309-da13a446028c"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _quotationRepository.GetCountAsync(
                    code: "34494dd14c864b1e9b6a02ee296b00952cb6a7a974",
                    name: "54e3ef38c2874560825f89041bbc35727b7fc542e8d44e14a2b73888db39f806aa8b31780b524b37ab9",
                    status: default,
                    depositRequired: true
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}