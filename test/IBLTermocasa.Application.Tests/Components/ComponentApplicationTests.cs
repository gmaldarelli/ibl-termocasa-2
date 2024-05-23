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
            result.Items.Any(x => x.Id == Guid.Parse("a277f0ce-c5ed-4957-9b49-9cdc39c6bb90")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("2519e506-b867-4e8b-b1c8-8c7a3085baef")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _componentsAppService.GetAsync(Guid.Parse("a277f0ce-c5ed-4957-9b49-9cdc39c6bb90"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("a277f0ce-c5ed-4957-9b49-9cdc39c6bb90"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new ComponentCreateDto
            {
                Name = "58fb1abe2cf140afbc4f5513aefa1b615ab9d5ebdbbe4800b1228e7aa8d6"
            };

            // Act
            var serviceResult = await _componentsAppService.CreateAsync(input);

            // Assert
            var result = await _componentRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe("58fb1abe2cf140afbc4f5513aefa1b615ab9d5ebdbbe4800b1228e7aa8d6");
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new ComponentUpdateDto()
            {
                Name = "e2b22ab7398f4b89a5069bc76bd25517cd30e646c4d147b5916decb367a21a70cc72"
            };

            // Act
            var serviceResult = await _componentsAppService.UpdateAsync(Guid.Parse("a277f0ce-c5ed-4957-9b49-9cdc39c6bb90"), input);

            // Assert
            var result = await _componentRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Name.ShouldBe("e2b22ab7398f4b89a5069bc76bd25517cd30e646c4d147b5916decb367a21a70cc72");
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _componentsAppService.DeleteAsync(Guid.Parse("a277f0ce-c5ed-4957-9b49-9cdc39c6bb90"));

            // Assert
            var result = await _componentRepository.FindAsync(c => c.Id == Guid.Parse("a277f0ce-c5ed-4957-9b49-9cdc39c6bb90"));

            result.ShouldBeNull();
        }
    }
}