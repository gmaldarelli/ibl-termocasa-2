using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace IBLTermocasa.RequestForQuotations
{
    public abstract class RequestForQuotationsAppServiceTests<TStartupModule> : IBLTermocasaApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IRequestForQuotationsAppService _requestForQuotationsAppService;
        private readonly IRepository<RequestForQuotation, Guid> _requestForQuotationRepository;

        public RequestForQuotationsAppServiceTests()
        {
            _requestForQuotationsAppService = GetRequiredService<IRequestForQuotationsAppService>();
            _requestForQuotationRepository = GetRequiredService<IRepository<RequestForQuotation, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _requestForQuotationsAppService.GetListAsync(new GetRequestForQuotationsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.RequestForQuotation.Id == Guid.Parse("de88a145-0c74-4b77-9f92-df32c6a6bc4e")).ShouldBe(true);
            result.Items.Any(x => x.RequestForQuotation.Id == Guid.Parse("b07b21f0-04fb-4039-9454-390c10206801")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _requestForQuotationsAppService.GetAsync(Guid.Parse("de88a145-0c74-4b77-9f92-df32c6a6bc4e"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("de88a145-0c74-4b77-9f92-df32c6a6bc4e"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new RequestForQuotationCreateDto
            {
                QuoteNumber = "199d21ad1b2c4b80a9b9fd4e51e52753e86975c7356b45698d4f5ff",
                WorkSite = "1abc19",
                City = "1ad91bd8bdea4ba0b2ad6937e170438243ca6def2a2a48e4bb",
                OrganizationProperty = "4d5f75d427d34ca1a7f2842",
                ContactProperty = "c633d7aabb1b4506804b33df",
                PhoneInfo = "5a4f863ca20f4014a3131329243047830b612f389af947319f97e4dd98549b34a8da",
                MailInfo = "3394f211d1304e5dabaa724e92cc2d8e82d3383346824d3a924ff8c53617ae053c7e7",
                Discount = 1332453570,
                Description = "b01dba2682c647148ef417a9baa1377edc91b285a2ac40c38e6379e",
                Status = default
            };

            // Act
            var serviceResult = await _requestForQuotationsAppService.CreateAsync(input);

            // Assert
            var result = await _requestForQuotationRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.QuoteNumber.ShouldBe("199d21ad1b2c4b80a9b9fd4e51e52753e86975c7356b45698d4f5ff");
            result.WorkSite.ShouldBe("1abc19");
            result.City.ShouldBe("1ad91bd8bdea4ba0b2ad6937e170438243ca6def2a2a48e4bb");
            result.OrganizationProperty.ShouldBe("4d5f75d427d34ca1a7f2842");
            result.ContactProperty.ShouldBe("c633d7aabb1b4506804b33df");
            result.PhoneInfo.ShouldBe("5a4f863ca20f4014a3131329243047830b612f389af947319f97e4dd98549b34a8da");
            result.MailInfo.ShouldBe("3394f211d1304e5dabaa724e92cc2d8e82d3383346824d3a924ff8c53617ae053c7e7");
            result.Discount.ShouldBe(1332453570);
            result.Description.ShouldBe("b01dba2682c647148ef417a9baa1377edc91b285a2ac40c38e6379e");
            result.Status.ShouldBe(default);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new RequestForQuotationUpdateDto()
            {
                QuoteNumber = "1cff3f",
                WorkSite = "404139d795c045beb4",
                City = "d286b23a0d4e481f97b3a4e7a71eee53309ce7c94ab2427",
                OrganizationProperty = "256d0a049c3d4762b2ef75e05dce6f4717227e99449f4bde9e56734cf629784891900b2233a8434191e08933ff5052",
                ContactProperty = "b848b51c6d2c4962a530cfa4be452340a672d2dc924b4927912d3918231debc",
                PhoneInfo = "611b139c0e8341f8ac3f1df66b71acaf9f9d7a43b3b74a8095b497ad5c41207686c46bc1b2df4",
                MailInfo = "ca82d991ff32467bb0600e6a4a0f1857b5490a7512d4467a8f58074fbb09297f7ca",
                Discount = 210622969,
                Description = "998189eb95b944c89e555739789860465092d799ee7d42eda6ef577f2ffeacdfd",
                Status = default
            };

            // Act
            var serviceResult = await _requestForQuotationsAppService.UpdateAsync(Guid.Parse("de88a145-0c74-4b77-9f92-df32c6a6bc4e"), input);

            // Assert
            var result = await _requestForQuotationRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.QuoteNumber.ShouldBe("1cff3f");
            result.WorkSite.ShouldBe("404139d795c045beb4");
            result.City.ShouldBe("d286b23a0d4e481f97b3a4e7a71eee53309ce7c94ab2427");
            result.OrganizationProperty.ShouldBe("256d0a049c3d4762b2ef75e05dce6f4717227e99449f4bde9e56734cf629784891900b2233a8434191e08933ff5052");
            result.ContactProperty.ShouldBe("b848b51c6d2c4962a530cfa4be452340a672d2dc924b4927912d3918231debc");
            result.PhoneInfo.ShouldBe("611b139c0e8341f8ac3f1df66b71acaf9f9d7a43b3b74a8095b497ad5c41207686c46bc1b2df4");
            result.MailInfo.ShouldBe("ca82d991ff32467bb0600e6a4a0f1857b5490a7512d4467a8f58074fbb09297f7ca");
            result.Discount.ShouldBe(210622969);
            result.Description.ShouldBe("998189eb95b944c89e555739789860465092d799ee7d42eda6ef577f2ffeacdfd");
            result.Status.ShouldBe(default);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _requestForQuotationsAppService.DeleteAsync(Guid.Parse("de88a145-0c74-4b77-9f92-df32c6a6bc4e"));

            // Assert
            var result = await _requestForQuotationRepository.FindAsync(c => c.Id == Guid.Parse("de88a145-0c74-4b77-9f92-df32c6a6bc4e"));

            result.ShouldBeNull();
        }
    }
}