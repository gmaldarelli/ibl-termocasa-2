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
                    code: "c51228db35954bcbb036558cfe34ee871a27e6adab9940bfad29e7bc9f9ca2fdf9fbddde78f746a0",
                    name: "f687499a4f344537b9852cbfb81d806cf42c2406d21443aa867ba5e8",
                    description: "0f648430cf6f466d9ff18a67fda002e06a22abd9d9e7408a922aadaddb07fae9fcaf7916b8f74aaf801c4b1ff594ab",
                    isAssembled: true,
                    isInternal: true
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("4438de60-6a9a-45e3-8aa3-7692c33e752c"));
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
                    code: "53b6bf2d62964a9bbaebe7a61fd8a9e5074a648ec0a64a4cbbfc14262ecd9641dfa072a",
                    name: "67b09f03c87e46e8b9106089a41e4a4ce4062ee641b34af5b4",
                    description: "64338505adae42d0876b44aa265b8232e",
                    isAssembled: true,
                    isInternal: true
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}