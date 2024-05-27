using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace IBLTermocasa.Components
{
    public abstract class ComponentsAppServiceTests<TStartupModule> : IBLTermocasaApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IComponentsAppService _componentsAppService;
        private readonly IRepository<Component, Guid> _componentRepository;

        public ComponentsAppServiceTests()
        {
            _componentsAppService = GetRequiredService<IComponentsAppService>();
            _componentRepository = GetRequiredService<IRepository<Component, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _componentsAppService.GetListAsync(new GetComponentsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("93c4cb63-038b-48ba-8d3f-9a16fb6fa8b2")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("679c6836-e338-4e7e-b8f5-7c0cc6f103d0")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _componentsAppService.GetAsync(Guid.Parse("93c4cb63-038b-48ba-8d3f-9a16fb6fa8b2"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("93c4cb63-038b-48ba-8d3f-9a16fb6fa8b2"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new ComponentCreateDto
            {
                Name = "9245fad9caf34d02833810de5608259cabebf6ad84f24e53a30688ff12aec7549c71a956882547dbac01eb12eb6e294"
            };

            // Act
            var serviceResult = await _componentsAppService.CreateAsync(input);

            // Assert
            var result = await _componentRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe("9245fad9caf34d02833810de5608259cabebf6ad84f24e53a30688ff12aec7549c71a956882547dbac01eb12eb6e294");
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new ComponentUpdateDto()
            {
                Name = "b61e4ac29f44477393ed6f365d94f2653f5914427541445bbf614b373"
            };

            // Act
            var serviceResult = await _componentsAppService.UpdateAsync(Guid.Parse("93c4cb63-038b-48ba-8d3f-9a16fb6fa8b2"), input);

            // Assert
            var result = await _componentRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe("b61e4ac29f44477393ed6f365d94f2653f5914427541445bbf614b373");
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _componentsAppService.DeleteAsync(Guid.Parse("93c4cb63-038b-48ba-8d3f-9a16fb6fa8b2"));

            // Assert
            var result = await _componentRepository.FindAsync(c => c.Id == Guid.Parse("93c4cb63-038b-48ba-8d3f-9a16fb6fa8b2"));

            result.ShouldBeNull();
        }
    }
}