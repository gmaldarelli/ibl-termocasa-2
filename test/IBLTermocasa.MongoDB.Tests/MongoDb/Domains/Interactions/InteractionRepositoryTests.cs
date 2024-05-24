using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using IBLTermocasa.Interactions;
using IBLTermocasa.MongoDB;
using Xunit;

namespace IBLTermocasa.MongoDB.Domains.Interactions
{
    [Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
    public class InteractionRepositoryTests : IBLTermocasaMongoDbTestBase
    {
        private readonly IInteractionRepository _interactionRepository;

        public InteractionRepositoryTests()
        {
            _interactionRepository = GetRequiredService<IInteractionRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _interactionRepository.GetListAsync(
                    interactionType: default,
                    content: "ec0ab8f9f28347be8731cc1ff67023be71bf410c1dc245a490ad4291804e804bc950f5e0d7514dfc92a2b8b140fe9870ae9a9657a86e4a16a5557344a97cea1101f2a2ef26e649a0b028facf795bbed6ec51bee767ae4b69982a46ead4321eb184cbd4f4c1a74707af1df887fe2d2dc7415ec839d9f64950b442d45b42d2af1aeed2ca44fb444e70a2ff8f708eeb9d7c3d304bf7455c4ca38e41e56636a77beaa39fe50d5825489cb2b2cf8d6b6fe5428c2fb4b08e3c41afa9b6e18a37d63063b55b4c057e5444d9adcd526621c7da558c9a7928d1c34c4d975082c3478af17483354979177244688bd48d3860b3f7114ff0e1dfff2c48a8bbc9",
                    referenceObject: "7ce64adcf00541d68612ce93774c7a7742a24b2681f04a3f94dcfe00d12cf1e1ab90dd",
                    writerNotes: "528119b559bb494"
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("32a533d4-a18e-4a68-8976-837ae62cf83e"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _interactionRepository.GetCountAsync(
                    interactionType: default,
                    content: "269fe8baab6d400faed789670b581596d340d2f7822f4f7780943ac30cbc932f5e20f7a3a67744a2bc293a3b6105afed7c5d6da8dbb14cb78cebde7b633a6edfd312ee352e0e4461ad1df171a6723ee55a6ec707fde344d7a1d1949a4a3cbadfe2115f21cfee485d9dfffec783f7b950253e2d725c8d49689f857a6fcf89f23a20185ea88c44435b8b20f33137a6d7a8ee5d3ea6045a4eaba6ac6ef5d5fadce2fa07a76504a44b3281e19d6a0f45ea4bd0a3d1473f6a4d6693058ceb7e3daeedd418ce4e02d040da82e5ee866d3bb2863c96b5195d5944718c767823d881d37bf44e2dd6fb634bf9ba40e126f78c33bbaa80adff57724c9e8845",
                    referenceObject: "c81305854fac40cfbef09a719518b7b02",
                    writerNotes: "f2873f18293949f0bedb6329f32c47c21ac2e2a5e3f040a18f0e0fcc89c"
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}