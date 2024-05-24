using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace IBLTermocasa.QuestionTemplates
{
    public abstract class QuestionTemplatesAppServiceTests<TStartupModule> : IBLTermocasaApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IQuestionTemplatesAppService _questionTemplatesAppService;
        private readonly IRepository<QuestionTemplate, Guid> _questionTemplateRepository;

        public QuestionTemplatesAppServiceTests()
        {
            _questionTemplatesAppService = GetRequiredService<IQuestionTemplatesAppService>();
            _questionTemplateRepository = GetRequiredService<IRepository<QuestionTemplate, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _questionTemplatesAppService.GetListAsync(new GetQuestionTemplatesInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("109334a7-5eec-4d88-8e32-a3a4277c7ece")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("2bac9d7f-dec5-4b0e-b2b1-7712e713cc57")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _questionTemplatesAppService.GetAsync(Guid.Parse("109334a7-5eec-4d88-8e32-a3a4277c7ece"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("109334a7-5eec-4d88-8e32-a3a4277c7ece"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new QuestionTemplateCreateDto
            {
                Code = "63989ba7002e4df78fac6f22e24c11aa951b00f539874ab8b54bd5fd65369fbb00fdfd687c4a430798beb",
                QuestionText = "745ae22ac095470b9dd7a2d743e",
                AnswerType = default,
                ChoiceValue = "2987c58f14cf42a296e98e7372e478801c1ed961d11"
            };

            // Act
            var serviceResult = await _questionTemplatesAppService.CreateAsync(input);

            // Assert
            var result = await _questionTemplateRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Code.ShouldBe("63989ba7002e4df78fac6f22e24c11aa951b00f539874ab8b54bd5fd65369fbb00fdfd687c4a430798beb");
            result.QuestionText.ShouldBe("745ae22ac095470b9dd7a2d743e");
            result.AnswerType.ShouldBe(default);
            result.ChoiceValue.ShouldBe("2987c58f14cf42a296e98e7372e478801c1ed961d11");
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new QuestionTemplateUpdateDto()
            {
                Code = "7ac9aa957d0947daa11db1d06bbee156015f1e8d65cb4d4ab8275c7eebe18b69b900b714f6384c9a8628",
                QuestionText = "c2db1cce18134ed9b740be2468838828ab03e2924ec84d30bf3d368afca6b72e3f09a61b94ed4820861751d15b14",
                AnswerType = default,
                ChoiceValue = "ffa5b02ad5fa4ca49e735b5456e63d91e"
            };

            // Act
            var serviceResult = await _questionTemplatesAppService.UpdateAsync(Guid.Parse("109334a7-5eec-4d88-8e32-a3a4277c7ece"), input);

            // Assert
            var result = await _questionTemplateRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Code.ShouldBe("7ac9aa957d0947daa11db1d06bbee156015f1e8d65cb4d4ab8275c7eebe18b69b900b714f6384c9a8628");
            result.QuestionText.ShouldBe("c2db1cce18134ed9b740be2468838828ab03e2924ec84d30bf3d368afca6b72e3f09a61b94ed4820861751d15b14");
            result.AnswerType.ShouldBe(default);
            result.ChoiceValue.ShouldBe("ffa5b02ad5fa4ca49e735b5456e63d91e");
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _questionTemplatesAppService.DeleteAsync(Guid.Parse("109334a7-5eec-4d88-8e32-a3a4277c7ece"));

            // Assert
            var result = await _questionTemplateRepository.FindAsync(c => c.Id == Guid.Parse("109334a7-5eec-4d88-8e32-a3a4277c7ece"));

            result.ShouldBeNull();
        }
    }
}