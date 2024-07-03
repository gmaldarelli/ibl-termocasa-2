using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace IBLTermocasa.ProfessionalProfiles
{
    public abstract class ProfessionalProfilesAppServiceTests<TStartupModule> : IBLTermocasaApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IProfessionalProfilesAppService _professionalProfilesAppService;
        private readonly IRepository<ProfessionalProfile, Guid> _professionalProfileRepository;

        public ProfessionalProfilesAppServiceTests()
        {
            _professionalProfilesAppService = GetRequiredService<IProfessionalProfilesAppService>();
            _professionalProfileRepository = GetRequiredService<IRepository<ProfessionalProfile, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _professionalProfilesAppService.GetListAsync(new GetProfessionalProfilesInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("b8f44d45-44c6-4e2d-9c75-623c8764bfa4")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("b8b8926a-e8d6-4163-ab88-0865e1b588d7")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _professionalProfilesAppService.GetAsync(Guid.Parse("b8f44d45-44c6-4e2d-9c75-623c8764bfa4"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("b8f44d45-44c6-4e2d-9c75-623c8764bfa4"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new ProfessionalProfileCreateDto
            {
                Name = "e6bf46b9778440acb187926bf92c7d7c0cd21d31f9de40f9ba16f44d78e45421204898ec6f8344faa0512f1f81d7b2b4c1",
                StandardPrice = 2098226076
            };

            // Act
            var serviceResult = await _professionalProfilesAppService.CreateAsync(input);

            // Assert
            var result = await _professionalProfileRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe("e6bf46b9778440acb187926bf92c7d7c0cd21d31f9de40f9ba16f44d78e45421204898ec6f8344faa0512f1f81d7b2b4c1");
            result.StandardPrice.ShouldBe(2098226076);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new ProfessionalProfileUpdateDto()
            {
                Name = "4e765e54835d47d1811a4895b4dd1346810fb1d61f0e490ebf0778fedab0b72f60539e1317d3429393c6d",
                StandardPrice = 1136976070
            };

            // Act
            var serviceResult = await _professionalProfilesAppService.UpdateAsync(Guid.Parse("b8f44d45-44c6-4e2d-9c75-623c8764bfa4"), input);

            // Assert
            var result = await _professionalProfileRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe("4e765e54835d47d1811a4895b4dd1346810fb1d61f0e490ebf0778fedab0b72f60539e1317d3429393c6d");
            result.StandardPrice.ShouldBe(1136976070);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _professionalProfilesAppService.DeleteAsync(Guid.Parse("b8f44d45-44c6-4e2d-9c75-623c8764bfa4"));

            // Assert
            var result = await _professionalProfileRepository.FindAsync(c => c.Id == Guid.Parse("b8f44d45-44c6-4e2d-9c75-623c8764bfa4"));

            result.ShouldBeNull();
        }
    }
}