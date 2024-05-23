using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using IBLTermocasa.Products;
using IBLTermocasa.MongoDB;
using Xunit;

namespace IBLTermocasa.MongoDB.Domains.Products
{
    [Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
    public class ProductRepositoryTests : IBLTermocasaMongoDbTestBase
    {
        private readonly IProductRepository _productRepository;

        public ProductRepositoryTests()
        {
            _productRepository = GetRequiredService<IProductRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _productRepository.GetListAsync(
                    code: "f89035412abd49ca901c6",
                    name: "d149a7618e564b6cb0d2b86e2c43057e481f450333ed49f08121f6b04fb7ad5cf1f471d087",
                    description: "eb9f92542c83476b99",
                    isAssembled: true,
                    isInternal: true
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("24dd8306-6ec6-45de-b299-d964d3ae9047"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _productRepository.GetCountAsync(
                    code: "3be38c6dc52c4c229",
                    name: "a07486fb37284c8896b272257ea15e1088424",
                    description: "b5f09bc8a30c4e2caa7c820a8959885e3332b",
                    isAssembled: true,
                    isInternal: true
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}