using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace IBLTermocasa.Interactions
{
    public abstract class InteractionsAppServiceTests<TStartupModule> : IBLTermocasaApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IInteractionsAppService _interactionsAppService;
        private readonly IRepository<Interaction, Guid> _interactionRepository;

        public InteractionsAppServiceTests()
        {
            _interactionsAppService = GetRequiredService<IInteractionsAppService>();
            _interactionRepository = GetRequiredService<IRepository<Interaction, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _interactionsAppService.GetListAsync(new GetInteractionsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Interaction.Id == Guid.Parse("32a533d4-a18e-4a68-8976-837ae62cf83e")).ShouldBe(true);
            result.Items.Any(x => x.Interaction.Id == Guid.Parse("f2cc0ade-f799-4456-971e-ce7630b6a366")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _interactionsAppService.GetAsync(Guid.Parse("32a533d4-a18e-4a68-8976-837ae62cf83e"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("32a533d4-a18e-4a68-8976-837ae62cf83e"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new InteractionCreateDto
            {
                InteractionType = default,
                InteractionDate = new DateTime(2011, 8, 13),
                Content = "5a174e0412f9484abcd9c9fce8220e7e8ba70503daf14ceab908ced46d8b1ca3d598e56bc1fc4345b13a688227384367818392af93cb48d1a60fd674d73693ddc7377d27a0ea47e8bcf8f522ea1a46d511f8535ddd9a4178885b938444e90e9f3f7c18299b7d4f4a8ef016e19e682d564a76092c00f94ba1bed857e60291ab4f2f7c93abe11645288b7268e23f6b962fc951d724cf3a4264bc7ee1406f94e245f985c5929adc40208e3888c6f903f4c042574b771fdd4e4c916543b6caf85e665bbecf15e8444fbeb08330961b363c44185efcebfc714f76b769b1e61848ab10ace26a7959604981a27685a438f4e6bb44229e8a7dee4322be43",
                ReferenceObject = "990464faf1c641e9b2456b2b6825230b",
                WriterNotes = "7f19525e11a546",
                WriterUserId = Guid.Parse("f2cc0ade-f799-4456-971e-ce7630b6a366"),

            };

            // Act
            var serviceResult = await _interactionsAppService.CreateAsync(input);

            // Assert
            var result = await _interactionRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.InteractionType.ShouldBe(default);
            result.InteractionDate.ShouldBe(new DateTime(2011, 8, 13));
            result.Content.ShouldBe("5a174e0412f9484abcd9c9fce8220e7e8ba70503daf14ceab908ced46d8b1ca3d598e56bc1fc4345b13a688227384367818392af93cb48d1a60fd674d73693ddc7377d27a0ea47e8bcf8f522ea1a46d511f8535ddd9a4178885b938444e90e9f3f7c18299b7d4f4a8ef016e19e682d564a76092c00f94ba1bed857e60291ab4f2f7c93abe11645288b7268e23f6b962fc951d724cf3a4264bc7ee1406f94e245f985c5929adc40208e3888c6f903f4c042574b771fdd4e4c916543b6caf85e665bbecf15e8444fbeb08330961b363c44185efcebfc714f76b769b1e61848ab10ace26a7959604981a27685a438f4e6bb44229e8a7dee4322be43");
            result.ReferenceObject.ShouldBe("990464faf1c641e9b2456b2b6825230b");
            result.WriterNotes.ShouldBe("7f19525e11a546");
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new InteractionUpdateDto()
            {
                InteractionType = default,
                InteractionDate = new DateTime(2007, 10, 11),
                Content = "910f90cc297d44dbbc1e348ee4fd6a114f2ba09b2f9f4f819dfde1903f56585042fca4ffe6364fc68b9952c5ffb30027ebc26711d88d4ff58d2246a47fcbd5b543c4a04ed80242cb9882b5c2a58163dd71953d08b6d34367bdc3367596491bbd707638425e0a4ff395501d41efaae6bbe00d36d2f00c41c9bf611566cc91208a3e4270b089494e96baa95ca9c735f01b3a88e73243584dd78546b21b2d6a64b25f5ed3a283c64138bdf4045b0792b97b9b756da8f73e41c2ba0b022be66669238a8873e76da1468db7263e01c83f7de984d978e392824db290ba9eaf93ed520ea867723446b44fbbb61a71bdb205ee46842e5ee645494e10bd86",
                ReferenceObject = "f86180b3b",
                WriterNotes = "072f82f2918c4dffa2277b158951ba7b507cef9e41724b33ae4a0c71d150a0e79a2957ac49a5443b8a8aa87f28d",
                WriterUserId = Guid.Parse("f2cc0ade-f799-4456-971e-ce7630b6a366"),

            };

            // Act
            var serviceResult = await _interactionsAppService.UpdateAsync(Guid.Parse("32a533d4-a18e-4a68-8976-837ae62cf83e"), input);

            // Assert
            var result = await _interactionRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.InteractionType.ShouldBe(default);
            result.InteractionDate.ShouldBe(new DateTime(2007, 10, 11));
            result.Content.ShouldBe("910f90cc297d44dbbc1e348ee4fd6a114f2ba09b2f9f4f819dfde1903f56585042fca4ffe6364fc68b9952c5ffb30027ebc26711d88d4ff58d2246a47fcbd5b543c4a04ed80242cb9882b5c2a58163dd71953d08b6d34367bdc3367596491bbd707638425e0a4ff395501d41efaae6bbe00d36d2f00c41c9bf611566cc91208a3e4270b089494e96baa95ca9c735f01b3a88e73243584dd78546b21b2d6a64b25f5ed3a283c64138bdf4045b0792b97b9b756da8f73e41c2ba0b022be66669238a8873e76da1468db7263e01c83f7de984d978e392824db290ba9eaf93ed520ea867723446b44fbbb61a71bdb205ee46842e5ee645494e10bd86");
            result.ReferenceObject.ShouldBe("f86180b3b");
            result.WriterNotes.ShouldBe("072f82f2918c4dffa2277b158951ba7b507cef9e41724b33ae4a0c71d150a0e79a2957ac49a5443b8a8aa87f28d");
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _interactionsAppService.DeleteAsync(Guid.Parse("32a533d4-a18e-4a68-8976-837ae62cf83e"));

            // Assert
            var result = await _interactionRepository.FindAsync(c => c.Id == Guid.Parse("32a533d4-a18e-4a68-8976-837ae62cf83e"));

            result.ShouldBeNull();
        }
    }
}