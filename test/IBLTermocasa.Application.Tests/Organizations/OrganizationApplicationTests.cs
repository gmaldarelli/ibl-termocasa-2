using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace IBLTermocasa.Organizations
{
    public abstract class OrganizationsAppServiceTests<TStartupModule> : IBLTermocasaApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IOrganizationsAppService _organizationsAppService;
        private readonly IRepository<Organization, Guid> _organizationRepository;

        public OrganizationsAppServiceTests()
        {
            _organizationsAppService = GetRequiredService<IOrganizationsAppService>();
            _organizationRepository = GetRequiredService<IRepository<Organization, Guid>>();
        }

               [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _organizationsAppService.GetListAsync(new GetOrganizationsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _organizationsAppService.GetAsync(Guid.Parse("9d656c2d-37ca-4d1b-a5bc-ca4b4cc5268a"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("9d656c2d-37ca-4d1b-a5bc-ca4b4cc5268a"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new OrganizationCreateDto
            {
                Name = "635cd8a648e54483b1b85b67af1a0c06195ca5f90be7450387aeaa446c0f339bc22622837b6b40c1b8291d86e3f9bb9a"
            };

            // Act
            var serviceResult = await _organizationsAppService.CreateAsync(input);

            // Assert
            var result = await _organizationRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe("635cd8a648e54483b1b85b67af1a0c06195ca5f90be7450387aeaa446c0f339bc22622837b6b40c1b8291d86e3f9bb9a");
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new OrganizationUpdateDto()
            {
                Name = "62523c3a7b334c1fb0cc318028534a3d9783bfb1c6e84bdea38d4f"
            };

            // Act
            var serviceResult = await _organizationsAppService.UpdateAsync(Guid.Parse("9d656c2d-37ca-4d1b-a5bc-ca4b4cc5268a"), input);

            // Assert
            var result = await _organizationRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe("62523c3a7b334c1fb0cc318028534a3d9783bfb1c6e84bdea38d4f");
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _organizationsAppService.DeleteAsync(Guid.Parse("9d656c2d-37ca-4d1b-a5bc-ca4b4cc5268a"));

            // Assert
            var result = await _organizationRepository.FindAsync(c => c.Id == Guid.Parse("9d656c2d-37ca-4d1b-a5bc-ca4b4cc5268a"));

            result.ShouldBeNull();
        }
    }
}