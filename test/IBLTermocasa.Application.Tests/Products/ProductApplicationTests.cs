using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace IBLTermocasa.Products
{
    public abstract class ProductsAppServiceTests<TStartupModule> : IBLTermocasaApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IProductsAppService _productsAppService;
        private readonly IRepository<Product, Guid> _productRepository;

        public ProductsAppServiceTests()
        {
            _productsAppService = GetRequiredService<IProductsAppService>();
            _productRepository = GetRequiredService<IRepository<Product, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _productsAppService.GetListAsync(new GetProductsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Product.Id == Guid.Parse("24dd8306-6ec6-45de-b299-d964d3ae9047")).ShouldBe(true);
            result.Items.Any(x => x.Product.Id == Guid.Parse("dc25b2b3-d2f4-4d83-87c3-41a58d8f3806")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _productsAppService.GetAsync(Guid.Parse("24dd8306-6ec6-45de-b299-d964d3ae9047"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("24dd8306-6ec6-45de-b299-d964d3ae9047"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new ProductCreateDto
            {
                Code = "89e3640194a44c218113f960bc982b33eeebe1143e44459",
                Name = "ccc938f7f98341329c4ef7acd7c13edf081a98506e4540f698cee98",
                Description = "032bf42bc77b43928d76bda9bd55da10f81a6",
                IsAssembled = true,
                IsInternal = true
            };

            // Act
            var serviceResult = await _productsAppService.CreateAsync(input);

            // Assert
            var result = await _productRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Code.ShouldBe("89e3640194a44c218113f960bc982b33eeebe1143e44459");
            result.Name.ShouldBe("ccc938f7f98341329c4ef7acd7c13edf081a98506e4540f698cee98");
            result.Description.ShouldBe("032bf42bc77b43928d76bda9bd55da10f81a6");
            result.IsAssembled.ShouldBe(true);
            result.IsInternal.ShouldBe(true);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new ProductUpdateDto()
            {
                Code = "95f15df70b2648389aa",
                Name = "edc150b5f3ce443394ead4e3f459f3230319011ef0cf47da",
                Description = "afb6b6cd0ea6472c98516695c7",
                IsAssembled = true,
                IsInternal = true
            };

            // Act
            var serviceResult = await _productsAppService.UpdateAsync(Guid.Parse("24dd8306-6ec6-45de-b299-d964d3ae9047"), input);

            // Assert
            var result = await _productRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Code.ShouldBe("95f15df70b2648389aa");
            result.Name.ShouldBe("edc150b5f3ce443394ead4e3f459f3230319011ef0cf47da");
            result.Description.ShouldBe("afb6b6cd0ea6472c98516695c7");
            result.IsAssembled.ShouldBe(true);
            result.IsInternal.ShouldBe(true);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _productsAppService.DeleteAsync(Guid.Parse("24dd8306-6ec6-45de-b299-d964d3ae9047"));

            // Assert
            var result = await _productRepository.FindAsync(c => c.Id == Guid.Parse("24dd8306-6ec6-45de-b299-d964d3ae9047"));

            result.ShouldBeNull();
        }
    }
}