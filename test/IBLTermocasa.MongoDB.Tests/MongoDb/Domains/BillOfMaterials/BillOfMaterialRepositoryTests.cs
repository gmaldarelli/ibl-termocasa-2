using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using IBLTermocasa.BillOfMaterials;
using IBLTermocasa.Common;
using IBLTermocasa.MongoDB;
using IBLTermocasa.Types;
using Xunit;

namespace IBLTermocasa.MongoDB.Domains.BillOfMaterials
{
    [Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
    public class BillOfMaterialRepositoryTests : IBLTermocasaMongoDbTestBase
    {
        private readonly IBillOfMaterialRepository _billOfMaterialRepository;

        public BillOfMaterialRepositoryTests()
        {
            _billOfMaterialRepository = GetRequiredService<IBillOfMaterialRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _billOfMaterialRepository.GetListAsync(
                    bomNumber: "678705e217e042a0bb87467f7e84609bdeab0599408d4243ba43b2ce40c191053e21e974109c40cdb9d8bad64c5c3145a1e",
                    requestForQuotationProperty: new RequestForQuotationProperty(),
                    status: BomStatusType.CREATED
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("69297841-8d3b-44de-a9dd-87fdbdf73964"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _billOfMaterialRepository.GetCountAsync(
                    bomNumber: "678705e217e042a0bb87467f7e84609bdeab0599408d4243ba43b2ce40c191053e21e974109c40cdb9d8bad64c5c3145a1e",
                    requestForQuotationProperty: new RequestForQuotationProperty(),
                    status: BomStatusType.CREATED
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}