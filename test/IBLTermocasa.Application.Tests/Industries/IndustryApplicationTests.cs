using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace IBLTermocasa.Industries
{
    public abstract class IndustriesAppServiceTests<TStartupModule> : IBLTermocasaApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IIndustriesAppService _industriesAppService;
        private readonly IRepository<Industry, Guid> _industryRepository;

        public IndustriesAppServiceTests()
        {
            _industriesAppService = GetRequiredService<IIndustriesAppService>();
            _industryRepository = GetRequiredService<IRepository<Industry, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _industriesAppService.GetListAsync(new GetIndustriesInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("0ec6c141-e1b9-454f-bb53-357a250bba54")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("87d3616a-ab6f-40ed-8b20-4f1af33acf98")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _industriesAppService.GetAsync(Guid.Parse("0ec6c141-e1b9-454f-bb53-357a250bba54"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("0ec6c141-e1b9-454f-bb53-357a250bba54"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new IndustryCreateDto
            {
                Code = "8f649410d1394c1883e9431ab5d2bbe29c96042c96964620b8f824a679495853b8f1a4c38bf844d39f5625ac3f7ed5f9b9b09bcff29549178c0860ca559d8c5b3fccca360aa14b06b262192b2571e37f6ddcb67ced3342aea5b371f5e96fbac2717b4d3f6bc14e1a8d468516ebe75e3aa1a99489019b4a46a2c5936734d24c4",
                Description = "f53e960646e54d849fc50b51579a42fdbd041181ee8b4980ab2bb01a9387ee104c9972cb4dc44f9fbde1ba98197f82ab2967a9f086a546c5bf21d69f1e13d7f165fd154846174fa19c953ecc6f565bd82b4f4324befb4c46a6c9eceef76ba250aada1bcb2c6d4cf7bbc2d0defc5d866f608abe70f08147d69561479c3e165025fd2da1debcd441feaca9e915e2e81f63ba93ee20ee364d01b1157308d954ff912871a22e34d54cd6a8322ea439730af646b6661bd91c46c5b3a7772b89e4998ce48a00a2caf2418498582b068b1addb6132c79458d0845e183e145bae59c4e15d715958aa19b4d39bd86227be6ad14e8f76645bc568a4266a93c"
            };

            // Act
            var serviceResult = await _industriesAppService.CreateAsync(input);

            // Assert
            var result = await _industryRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Code.ShouldBe("8f649410d1394c1883e9431ab5d2bbe29c96042c96964620b8f824a679495853b8f1a4c38bf844d39f5625ac3f7ed5f9b9b09bcff29549178c0860ca559d8c5b3fccca360aa14b06b262192b2571e37f6ddcb67ced3342aea5b371f5e96fbac2717b4d3f6bc14e1a8d468516ebe75e3aa1a99489019b4a46a2c5936734d24c4");
            result.Description.ShouldBe("f53e960646e54d849fc50b51579a42fdbd041181ee8b4980ab2bb01a9387ee104c9972cb4dc44f9fbde1ba98197f82ab2967a9f086a546c5bf21d69f1e13d7f165fd154846174fa19c953ecc6f565bd82b4f4324befb4c46a6c9eceef76ba250aada1bcb2c6d4cf7bbc2d0defc5d866f608abe70f08147d69561479c3e165025fd2da1debcd441feaca9e915e2e81f63ba93ee20ee364d01b1157308d954ff912871a22e34d54cd6a8322ea439730af646b6661bd91c46c5b3a7772b89e4998ce48a00a2caf2418498582b068b1addb6132c79458d0845e183e145bae59c4e15d715958aa19b4d39bd86227be6ad14e8f76645bc568a4266a93c");
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new IndustryUpdateDto()
            {
                Code = "94c419e77cad4df784c25b7cb516f305b33a79301aec4768bc4c03b56b3453eadda0f9368bf94c65978249b48b2cccbb852067a7e2d34ff98302b0602fd0cbf59df05cfd695b496aa84ae622d596a13dc07c12297e6a4393915aa0ee9f73a3422ad1ddf19a1b4967b701a7f9704cad9c17511142cd314950b10465a41826300",
                Description = "00c9859c02d04d86957d1e1a9197feb06b936276f3de4316bba4a0c1c8f9e9f61764a7fc18e04c1488b120db1ca7bf8cf5b920ca02724f91868e7ecd4829c4a360ea6a905dc84771b6711561e19fda59dbeb3c6f4d6d415f94424546909196e489af168f60ea468bb1ce0fe8ef73a91f41dfce6f279f4e9ea1264ddd352cc924aae87235ed1e4f2b9c7bd0ba3a672437e4404cee64fc45edb52b16c15dbc5417676242ff4a0649ce9d52b60311e47d37b32c09b885494f638a7379d42b07ba2b0f35d7ccb2fb49d1891378905e3839077008d1528ee2461983723723022125b73bfeb69a06984c87862610a33137dc38bf79961d368645ec8fdf"
            };

            // Act
            var serviceResult = await _industriesAppService.UpdateAsync(Guid.Parse("0ec6c141-e1b9-454f-bb53-357a250bba54"), input);

            // Assert
            var result = await _industryRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Code.ShouldBe("94c419e77cad4df784c25b7cb516f305b33a79301aec4768bc4c03b56b3453eadda0f9368bf94c65978249b48b2cccbb852067a7e2d34ff98302b0602fd0cbf59df05cfd695b496aa84ae622d596a13dc07c12297e6a4393915aa0ee9f73a3422ad1ddf19a1b4967b701a7f9704cad9c17511142cd314950b10465a41826300");
            result.Description.ShouldBe("00c9859c02d04d86957d1e1a9197feb06b936276f3de4316bba4a0c1c8f9e9f61764a7fc18e04c1488b120db1ca7bf8cf5b920ca02724f91868e7ecd4829c4a360ea6a905dc84771b6711561e19fda59dbeb3c6f4d6d415f94424546909196e489af168f60ea468bb1ce0fe8ef73a91f41dfce6f279f4e9ea1264ddd352cc924aae87235ed1e4f2b9c7bd0ba3a672437e4404cee64fc45edb52b16c15dbc5417676242ff4a0649ce9d52b60311e47d37b32c09b885494f638a7379d42b07ba2b0f35d7ccb2fb49d1891378905e3839077008d1528ee2461983723723022125b73bfeb69a06984c87862610a33137dc38bf79961d368645ec8fdf");
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _industriesAppService.DeleteAsync(Guid.Parse("0ec6c141-e1b9-454f-bb53-357a250bba54"));

            // Assert
            var result = await _industryRepository.FindAsync(c => c.Id == Guid.Parse("0ec6c141-e1b9-454f-bb53-357a250bba54"));

            result.ShouldBeNull();
        }
    }
}