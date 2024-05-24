using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace IBLTermocasa.Organizations
{
    public abstract class OrganizationsAppServiceTests<TStartupModule> : IBLTermocasaApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IOrganizationsAppService _organizationsAppService;
        private readonly IRepository<Organization, Guid> _organizationRepository;

        public OrganizationsAppServiceTests()
        {
            _organizationsAppService = GetRequiredService<IOrganizationsAppService>();
            _organizationRepository = GetRequiredService<IRepository<Organization, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _organizationsAppService.GetListAsync(new GetOrganizationsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Organization.Id == Guid.Parse("a88b3013-fe24-4b76-a523-b5c921319218")).ShouldBe(true);
            result.Items.Any(x => x.Organization.Id == Guid.Parse("6ca5b11b-6b30-42fe-827c-4c2377bb05cf")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _organizationsAppService.GetAsync(Guid.Parse("a88b3013-fe24-4b76-a523-b5c921319218"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("a88b3013-fe24-4b76-a523-b5c921319218"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new OrganizationCreateDto
            {
                Code = "915c881c0de843fdad6151e5233c9aca5e7c038544904c7e925aa5f7427ad8cfd57fe81843b",
                Name = "5bab3e3aee5a4b3d89ff6b220bb76cedb7936504199c4b1bb9",
                OrganizationType = default,
                MailInfo = "29b9f4f129a848b58dbcabf3",
                PhoneInfo = "45e1b5228a554824",
                SocialInfo = "7435409aed5c42a39472f934712103b42c8d1e93bd0747d480288c3b0b24",
                BillingAddress = "ad07b5c8836b4a38bcedde",
                ShippingAddress = "ac95149b21a349ce8ee1d8dc239db79827b2c97ba82943b8a59cbe015e434cffc9e3895b561346099a23f1aa2dff3f3a",
                Tags = "866d72174f604da1871fde1",
                Notes = "0087cc5cf51947a79cd71322c643aedadcfb01c6e9c8407589cc339df91fdf5f61c3e7300c2646a186640425e2676d",
                ImageId = "b2fde9fd6c0",
                IndustryId = Guid.Parse("0ec6c141-e1b9-454f-bb53-357a250bba54")
            };

            // Act
            var serviceResult = await _organizationsAppService.CreateAsync(input);

            // Assert
            var result = await _organizationRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Code.ShouldBe("915c881c0de843fdad6151e5233c9aca5e7c038544904c7e925aa5f7427ad8cfd57fe81843b");
            result.Name.ShouldBe("5bab3e3aee5a4b3d89ff6b220bb76cedb7936504199c4b1bb9");
            result.OrganizationType.ShouldBe(default);
            result.MailInfo.ShouldBe("29b9f4f129a848b58dbcabf3");
            result.PhoneInfo.ShouldBe("45e1b5228a554824");
            result.SocialInfo.ShouldBe("7435409aed5c42a39472f934712103b42c8d1e93bd0747d480288c3b0b24");
            result.BillingAddress.ShouldBe("ad07b5c8836b4a38bcedde");
            result.ShippingAddress.ShouldBe("ac95149b21a349ce8ee1d8dc239db79827b2c97ba82943b8a59cbe015e434cffc9e3895b561346099a23f1aa2dff3f3a");
            result.Tags.ShouldBe("866d72174f604da1871fde1");
            result.Notes.ShouldBe("0087cc5cf51947a79cd71322c643aedadcfb01c6e9c8407589cc339df91fdf5f61c3e7300c2646a186640425e2676d");
            result.ImageId.ShouldBe("b2fde9fd6c0");
        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new OrganizationUpdateDto()
            {
                Code = "a459c61c51b64ced9b020f0646c6ad1329df2a45da8148d4ba00b7d003cf659d74fa5244819541",
                Name = "d3af990dd07e497fac2",
                OrganizationType = default,
                MailInfo = "e6f26b7441974888b3e3939396fbae37998fbcd740fe46c2a74282a8fc52669c1230133696a949ad92d0",
                PhoneInfo = "25a6656a53ad4ee8b3498683c7e8813f",
                SocialInfo = "9b02632e70564fe78158b7a1c352ee70139e30d32b804b57a5c3c0f4b9dbca9895504444fdad463284b5789fb6095",
                BillingAddress = "a626ace82b0e4f26b1c69bc51bc9b348574c38511a3d462ab9ff4",
                ShippingAddress = "ab22703786304f5e90ce4a7a2ecf6080affa6ad4e20549a",
                Tags = "ff70cf762c704f4884ef49de7c1b04ba1813340fedec49418ce0ce1e671317615b8633ec06c74b",
                Notes = "2f77e383c3124e41b6feffe6f01bf43ec823338114964089a01a54f0b20f0ae07f2560ce0e824c2898",
                ImageId = "8ec5fc2ad8724168a8f59a30c",
                IndustryId = Guid.Parse("0ec6c141-e1b9-454f-bb53-357a250bba54")
            };

            // Act
            var serviceResult = await _organizationsAppService.UpdateAsync(Guid.Parse("a88b3013-fe24-4b76-a523-b5c921319218"), input);

            // Assert
            var result = await _organizationRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Code.ShouldBe("a459c61c51b64ced9b020f0646c6ad1329df2a45da8148d4ba00b7d003cf659d74fa5244819541");
            result.Name.ShouldBe("d3af990dd07e497fac2");
            result.OrganizationType.ShouldBe(default);
            result.MailInfo.ShouldBe("e6f26b7441974888b3e3939396fbae37998fbcd740fe46c2a74282a8fc52669c1230133696a949ad92d0");
            result.PhoneInfo.ShouldBe("25a6656a53ad4ee8b3498683c7e8813f");
            result.SocialInfo.ShouldBe("9b02632e70564fe78158b7a1c352ee70139e30d32b804b57a5c3c0f4b9dbca9895504444fdad463284b5789fb6095");
            result.BillingAddress.ShouldBe("a626ace82b0e4f26b1c69bc51bc9b348574c38511a3d462ab9ff4");
            result.ShippingAddress.ShouldBe("ab22703786304f5e90ce4a7a2ecf6080affa6ad4e20549a");
            result.Tags.ShouldBe("ff70cf762c704f4884ef49de7c1b04ba1813340fedec49418ce0ce1e671317615b8633ec06c74b");
            result.Notes.ShouldBe("2f77e383c3124e41b6feffe6f01bf43ec823338114964089a01a54f0b20f0ae07f2560ce0e824c2898");
            result.ImageId.ShouldBe("8ec5fc2ad8724168a8f59a30c");
        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _organizationsAppService.DeleteAsync(Guid.Parse("a88b3013-fe24-4b76-a523-b5c921319218"));

            // Assert
            var result = await _organizationRepository.FindAsync(c => c.Id == Guid.Parse("a88b3013-fe24-4b76-a523-b5c921319218"));

            result.ShouldBeNull();
        }
    }
}