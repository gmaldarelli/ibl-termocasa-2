using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using IBLTermocasa.RequestForQuotations;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace IBLTermocasa.Quotations
{
    public abstract class QuotationsAppServiceTests<TStartupModule> : IBLTermocasaApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IQuotationsAppService _quotationsAppService;
        private readonly IRepository<Quotation, Guid> _quotationRepository;

        public QuotationsAppServiceTests()
        {
            _quotationsAppService = GetRequiredService<IQuotationsAppService>();
            _quotationRepository = GetRequiredService<IRepository<Quotation, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _quotationsAppService.GetListAsync(new GetQuotationsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("cfc6d5db-1312-4d3b-9309-da13a446028c")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("c91c4ceb-10c8-459b-af22-be9e5b4ab2e9")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _quotationsAppService.GetAsync(Guid.Parse("cfc6d5db-1312-4d3b-9309-da13a446028c"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("cfc6d5db-1312-4d3b-9309-da13a446028c"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new QuotationCreateDto
            {
                IdRFQ = Guid.Parse("011be196d06a466a8edf1b58c10778fcf6efffa3cd79447393"),
                IdBOM = Guid.Parse("3098bd3655474b1c90cd1e3c61c188d3efce1b2ea9f24c6591d9edd69dd"),
                Code = "93e0e198835c4320a3c3b9f435cf6fa664db912",
                Name = "8b969ccacffc4c0f9c354ae2efcca462d1",
                SentDate = new DateTime(2020, 2, 2),
                QuotationValidDate = new DateTime(2014, 5, 3),
                ConfirmedDate = new DateTime(2020, 8, 11),
                Status = default,
                DepositRequired = true,
                DepositRequiredValue = 557413839,
                QuotationItems = new List<QuotationItem>()
            };

            // Act
            var serviceResult = await _quotationsAppService.CreateAsync(input);

            // Assert
            var result = await _quotationRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.IdRFQ.ShouldBe(Guid.Parse("011be196d06a466a8edf1b58c10778fcf6efffa3cd79447393"));
            result.IdBOM.ShouldBe(Guid.Parse("3098bd3655474b1c90cd1e3c61c188d3efce1b2ea9f24c6591d9edd69dd"));
            result.Code.ShouldBe("93e0e198835c4320a3c3b9f435cf6fa664db912");
            result.Name.ShouldBe("8b969ccacffc4c0f9c354ae2efcca462d1");
            result.SentDate.ShouldBe(new DateTime(2020, 2, 2));
            result.QuotationValidDate.ShouldBe(new DateTime(2014, 5, 3));
            result.ConfirmedDate.ShouldBe(new DateTime(2020, 8, 11));
            result.Status.ShouldBe(default);
            result.DepositRequired.ShouldBe(true);
            result.DepositRequiredValue.ShouldBe(557413839);
            result.QuotationItems.ShouldBe(new List<QuotationItem>());
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new QuotationUpdateDto()
            {
                IdRFQ = Guid.Parse("0259cc6d5a544727a83d11b9664575c09413eda2478c4ad7b14b8664e1a5f91971b7dc8ff"),
                IdBOM = Guid.Parse("1ca0e9d0abb540f2ba2cee670fcf528aec4a143"),
                Name = "e0f4148f4c8c4b8c870cd8b6b44b78e4853fc20e826e48fa9a290ccd3",
                SentDate = new DateTime(2002, 8, 24),
                QuotationValidDate = new DateTime(2009, 9, 18),
                ConfirmedDate = new DateTime(2014, 11, 15),
                Status = default,
                DepositRequired = true,
                DepositRequiredValue = 380868577,
                QuotationItems = new List<QuotationItemDto>()
            };

            // Act
            var serviceResult = await _quotationsAppService.UpdateAsync(Guid.Parse("cfc6d5db-1312-4d3b-9309-da13a446028c"), input);

            // Assert
            var result = await _quotationRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.IdRFQ.ShouldBe(Guid.Parse("0259cc6d5a544727a83d11b9664575c09413eda2478c4ad7b14b8664e1a5f91971b7dc8ff"));
            result.IdBOM.ShouldBe(Guid.Parse("1ca0e9d0abb540f2ba2cee670fcf528aec4a143"));
            result.Name.ShouldBe("e0f4148f4c8c4b8c870cd8b6b44b78e4853fc20e826e48fa9a290ccd3");
            result.SentDate.ShouldBe(new DateTime(2002, 8, 24));
            result.QuotationValidDate.ShouldBe(new DateTime(2009, 9, 18));
            result.ConfirmedDate.ShouldBe(new DateTime(2014, 11, 15));
            result.Status.ShouldBe(default);
            result.DepositRequired.ShouldBe(true);
            result.DepositRequiredValue.ShouldBe(380868577);
            result.QuotationItems.ShouldBe(new List<QuotationItem>());
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _quotationsAppService.DeleteAsync(Guid.Parse("cfc6d5db-1312-4d3b-9309-da13a446028c"));

            // Assert
            var result = await _quotationRepository.FindAsync(c => c.Id == Guid.Parse("cfc6d5db-1312-4d3b-9309-da13a446028c"));

            result.ShouldBeNull();
        }
    }
}