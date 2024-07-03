using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using IBLTermocasa.ProfessionalProfiles;
using IBLTermocasa.MongoDB;
using Xunit;

namespace IBLTermocasa.MongoDB.Domains.ProfessionalProfiles
{
    [Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
    public class ProfessionalProfileRepositoryTests : IBLTermocasaMongoDbTestBase
    {
        private readonly IProfessionalProfileRepository _professionalProfileRepository;

        public ProfessionalProfileRepositoryTests()
        {
            _professionalProfileRepository = GetRequiredService<IProfessionalProfileRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _professionalProfileRepository.GetListAsync(
                    name: "dec40a5d478746f8bdc180237ec848fd5bbd1d5fe54245e0"
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("b8f44d45-44c6-4e2d-9c75-623c8764bfa4"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _professionalProfileRepository.GetCountAsync(
                    name: "87735376e0c44ff8b23d064089c041b4087320f0938a4a5187c7481d78"
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}