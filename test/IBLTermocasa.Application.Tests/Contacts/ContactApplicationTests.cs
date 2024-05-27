using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
using IBLTermocasa.Common;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace IBLTermocasa.Contacts
{
    public abstract class ContactsAppServiceTests<TStartupModule> : IBLTermocasaApplicationTestBase<TStartupModule>
        where TStartupModule : IAbpModule
    {
        private readonly IContactsAppService _contactsAppService;
        private readonly IRepository<Contact, Guid> _contactRepository;

        public ContactsAppServiceTests()
        {
            _contactsAppService = GetRequiredService<IContactsAppService>();
            _contactRepository = GetRequiredService<IRepository<Contact, Guid>>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Act
            var result = await _contactsAppService.GetListAsync(new GetContactsInput());

            // Assert
            result.TotalCount.ShouldBe(2);
            result.Items.Count.ShouldBe(2);
            result.Items.Any(x => x.Id == Guid.Parse("fd0a8d18-5a5f-4b09-8cff-37b020c1ac25")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("93b144f3-4e0b-4658-b107-ed134e5b04a3")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _contactsAppService.GetAsync(Guid.Parse("fd0a8d18-5a5f-4b09-8cff-37b020c1ac25"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("fd0a8d18-5a5f-4b09-8cff-37b020c1ac25"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new ContactCreateDto
            {
                Title = "0a8241ccb35c4d29856024f9f12403dc6e5c3be436414695b9cffa6792d04a983bac",
                Name = "85f681621e7646168c9e8f1b1495cb84151e52b65181432693a194bb8090087bf05b67c95b5842",
                Surname = "585f125be65945b",
                ConfidentialName = "b1860aa9c27f464890d18ae757265813c3ab9",
                JobRole = "91221c1fdb0b4bdabc8c74840c61bfb25c92a05e01b842f5bc343d99662100",
                BirthDate = new DateTime(2023, 10, 18),
                MailInfo = new MailInfoDto()
                {
                    MailItems = new List<MailItemDto>()
                    {
                        new MailItemDto() { Email = "maill@sample.com", Type = MailType.OTHER },
                    }
                },
                PhoneInfo = new PhoneInfoDto()
                {
                    PhoneItems = new List<PhoneItemDto>()
                    {
                        new PhoneItemDto() { Number = "123456789", Type = PhoneType.OTHER },
                    }
                },
                SocialInfo = new SocialInfoDto()
                {
                    SocialItems = new List<SocialItemDto>()
                    {
                        new SocialItemDto() { Url = "social", SocialType = SocialType.OTHER },
                    }
                },
                AddressInfo = new AddressDto()
                {
                    City = "City",
                    Country = "Country",
                    PostalCode = "PostalCode",
                    Street = "Street",
                    State = "State",
                    AdditionalInfo = "AdditionalInfo"
                },
                Tags = new List<string>() { "f4c", "f4d", "f4e" },
                Notes = "6c9f7eb0e6224b2794b9459450bf7c7",

            };

            // Act
            var serviceResult = await _contactsAppService.CreateAsync(input);

            // Assert
            var result = await _contactRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Title.ShouldBe("0a8241ccb35c4d29856024f9f12403dc6e5c3be436414695b9cffa6792d04a983bac");
            result.Name.ShouldBe("85f681621e7646168c9e8f1b1495cb84151e52b65181432693a194bb8090087bf05b67c95b5842");
            result.Surname.ShouldBe("585f125be65945b");
            result.ConfidentialName.ShouldBe("b1860aa9c27f464890d18ae757265813c3ab9");
            result.JobRole.ShouldBe("91221c1fdb0b4bdabc8c74840c61bfb25c92a05e01b842f5bc343d99662100");
            result.BirthDate.ShouldBe(new DateTime(2023, 10, 18));
            result.MailInfo.MailItems[0].Email.ShouldBe("maill@sample.com");
            result.PhoneInfo.PhoneItems[0].Number.ShouldBe("123456789");
            result.SocialInfo.SocialItems[0].Url.ShouldBe("social");
            result.AddressInfo.City.ShouldBe("City");
            result.AddressInfo.Country.ShouldBe("Country");
            result.AddressInfo.PostalCode.ShouldBe("PostalCode");
            result.AddressInfo.Street.ShouldBe("Street");
            result.AddressInfo.State.ShouldBe("State");
            result.AddressInfo.AdditionalInfo.ShouldBe("AdditionalInfo");
            result.Tags[0].ShouldBe("f4c");
            result.Tags[1].ShouldBe("f4d");
            result.Tags[2].ShouldBe("f4e");
            result.Notes.ShouldBe("6c9f7eb0e6224b2794b9459450bf7c7");

        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new ContactUpdateDto()
            {
                Title =
                    "34fb5597cbe44d838158873f6f2c5cfd7da8f4164b4b4d01966a21e6a6186c5cb52bc76ae65647e78ebf2937878e54",
                Name = "20c8629193374d11b39c826abd62aefe3faf44f2381",
                Surname = "8e186227cb514d2499e684d628c970d5e04a5751b5fb450aa065478d2217130c7",
                ConfidentialName = "5d7c460029994117a1d20da961c3f2814b4eedf93fe34cdb",
                JobRole = "d6e8de7eaaec495c90c87ca7ade266c82662a6c5c6e7481893dca9f9eeebc5",
                BirthDate = new DateTime(2009, 2, 23),
                MailInfo = new MailInfoDto()
                {
                    MailItems = new List<MailItemDto>()
                    {
                        new MailItemDto() { Email = "maill@sample.com", Type = MailType.OTHER },
                    }
                },
                PhoneInfo = new PhoneInfoDto()
                {
                    PhoneItems = new List<PhoneItemDto>()
                    {
                        new PhoneItemDto() { Number = "123456789", Type = PhoneType.OTHER },
                    }
                },
                SocialInfo = new SocialInfoDto()
                {
                    SocialItems = new List<SocialItemDto>()
                    {
                        new SocialItemDto() { Url = "social", SocialType = SocialType.OTHER },
                    }
                },
                AddressInfo = new AddressDto()
                {
                    City = "City",
                    Country = "Country",
                    PostalCode = "PostalCode",
                    Street = "Street",
                    State = "State",
                    AdditionalInfo = "AdditionalInfo"
                },
                Tags = new List<string>() { "f4c", "f4d", "f4e" },
                Notes = "d9bf8b226817411d9afe8994b717a18c839557b2a41f46ac8498a33317ec2c501878",
            };

            // Act
            var serviceResult = await _contactsAppService.UpdateAsync(Guid.Parse("fd0a8d18-5a5f-4b09-8cff-37b020c1ac25"), input);

            // Assert
            var result = await _contactRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Title.ShouldBe("34fb5597cbe44d838158873f6f2c5cfd7da8f4164b4b4d01966a21e6a6186c5cb52bc76ae65647e78ebf2937878e54");
            result.Name.ShouldBe("20c8629193374d11b39c826abd62aefe3faf44f2381");
            result.Surname.ShouldBe("8e186227cb514d2499e684d628c970d5e04a5751b5fb450aa065478d2217130c7");
            result.ConfidentialName.ShouldBe("5d7c460029994117a1d20da961c3f2814b4eedf93fe34cdb");
            result.JobRole.ShouldBe("d6e8de7eaaec495c90c87ca7ade266c82662a6c5c6e7481893dca9f9eeebc5");
            result.BirthDate.ShouldBe(new DateTime(2009, 2, 23));
            result.MailInfo.MailItems[0].Email.ShouldBe("maill@sample.com");
            result.PhoneInfo.PhoneItems[0].Number.ShouldBe("123456789");
            result.SocialInfo.SocialItems[0].Url.ShouldBe("social");
            result.AddressInfo.City.ShouldBe("City");
            result.AddressInfo.Country.ShouldBe("Country");
            result.AddressInfo.PostalCode.ShouldBe("PostalCode");
            result.AddressInfo.Street.ShouldBe("Street");
            result.AddressInfo.State.ShouldBe("State");
            result.AddressInfo.AdditionalInfo.ShouldBe("AdditionalInfo");
            result.Tags[0].ShouldBe("f4c");
            result.Tags[1].ShouldBe("f4d");
            result.Tags[2].ShouldBe("f4e");
            result.Notes.ShouldBe("d9bf8b226817411d9afe8994b717a18c839557b2a41f46ac8498a33317ec2c501878");

        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _contactsAppService.DeleteAsync(Guid.Parse("fd0a8d18-5a5f-4b09-8cff-37b020c1ac25"));

            // Assert
            var result = await _contactRepository.FindAsync(c => c.Id == Guid.Parse("fd0a8d18-5a5f-4b09-8cff-37b020c1ac25"));

            result.ShouldBeNull();
        }
    }
}