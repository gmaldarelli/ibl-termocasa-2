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
            result.Items.Any(x => x.Product.Id == Guid.Parse("4438de60-6a9a-45e3-8aa3-7692c33e752c")).ShouldBe(true);
            result.Items.Any(x => x.Product.Id == Guid.Parse("58980b43-af35-4248-aa0e-fecdefd22bad")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _productsAppService.GetAsync(Guid.Parse("4438de60-6a9a-45e3-8aa3-7692c33e752c"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("4438de60-6a9a-45e3-8aa3-7692c33e752c"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new ProductCreateDto
            {
                Code = "3dfb27b6b5104905bfe7216d2975770232f07c49cc5948",
                Name = "c72da8fbe154474bab08681e1f19c47026304063e8b147ed960f299c32d2bee",
                Description = "d35f6b58b671458e8675e6da57b41d8ae42770d4014a480ba07d5d8d3ecd27f6f8a5c20",
                IsAssembled = true,
                IsInternal = true
            };

            // Act
            var serviceResult = await _productsAppService.CreateAsync(input);

            // Assert
            var result = await _productRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Code.ShouldBe("3dfb27b6b5104905bfe7216d2975770232f07c49cc5948");
            result.Name.ShouldBe("c72da8fbe154474bab08681e1f19c47026304063e8b147ed960f299c32d2bee");
            result.Description.ShouldBe("d35f6b58b671458e8675e6da57b41d8ae42770d4014a480ba07d5d8d3ecd27f6f8a5c20");
            result.IsAssembled.ShouldBe(true);
            result.IsInternal.ShouldBe(true);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new ProductUpdateDto()
            {
                Code = "cd4a0075c0a5",
                Name = "78ab4171f46c4d3b8de95dccf66bb9a889dd4b8d1aec471db831cdfced7",
                Description = "9d9b12590a294124af3ecdadf0b6fa168c207e2125fb4e759635633c49a36",
                IsAssembled = true,
                IsInternal = true
            };

            // Act
            var serviceResult = await _productsAppService.UpdateAsync(Guid.Parse("4438de60-6a9a-45e3-8aa3-7692c33e752c"), input);

            // Assert
            var result = await _productRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Code.ShouldBe("cd4a0075c0a5");
            result.Name.ShouldBe("78ab4171f46c4d3b8de95dccf66bb9a889dd4b8d1aec471db831cdfced7");
            result.Description.ShouldBe("9d9b12590a294124af3ecdadf0b6fa168c207e2125fb4e759635633c49a36");
            result.IsAssembled.ShouldBe(true);
            result.IsInternal.ShouldBe(true);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _productsAppService.DeleteAsync(Guid.Parse("4438de60-6a9a-45e3-8aa3-7692c33e752c"));

            // Assert
            var result = await _productRepository.FindAsync(c => c.Id == Guid.Parse("4438de60-6a9a-45e3-8aa3-7692c33e752c"));

            result.ShouldBeNull();
        }
    }
}