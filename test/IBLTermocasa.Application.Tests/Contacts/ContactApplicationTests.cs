using System;
using System.Linq;
using Shouldly;
using System.Threading.Tasks;
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
            result.Items.Any(x => x.Id == Guid.Parse("12fa922b-1f43-4071-b46f-804e04828c70")).ShouldBe(true);
            result.Items.Any(x => x.Id == Guid.Parse("6447913c-a85f-4df5-9c00-7c97e6bf76ed")).ShouldBe(true);
        }

        [Fact]
        public async Task GetAsync()
        {
            // Act
            var result = await _contactsAppService.GetAsync(Guid.Parse("12fa922b-1f43-4071-b46f-804e04828c70"));

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(Guid.Parse("12fa922b-1f43-4071-b46f-804e04828c70"));
        }

        [Fact]
        public async Task CreateAsync()
        {
            // Arrange
            var input = new ContactCreateDto
            {
                Title = "821357d99",
                Name = "25a0a1b49ba74141b3e3aa50be2ac6be084f3fb0bb1e43e5be73a504e9b28d3e0ac20833d2a94625b6320f28b56",
                Surname = "c93d8e66d571403dab2c0918b0f85aa5d07a57067cb84fd79ebaece697",
                ConfidentialName = "ce5383359c1f454ab2e0e7ead966a8c",
                JobRole = "a8eb1387fe964217bc9749fdc6844d770d77246e",
                BirthDate = new DateTime(2016, 11, 2),
                MailInfo = "8e48c02390884096bddab3ad7eb5c145b3ffe4792b3d4434a73435c46a44e2601f6c1e7cdcfa48179668419ca853fcc6",
                PhoneInfo = "3c22a7c055f6441a812f2db9d096e98ffd14df27fe404b9dbbcc6ac325804bf62755ec7f90374d4f85b519e872",
                SocialInfo = "5ce6e75386384611964cfc9b980d5",
                AddressInfo = "dd9d26557e294bf28ea5b8e312ec013bd6bd7869058b475",
                Tag = "467a0506ae714235b5a92fb22d2be62ed2926b9de35c409a8ad1f14",
                Notes = "c55479fb57354aec9004b8f64f0c",

            };

            // Act
            var serviceResult = await _contactsAppService.CreateAsync(input);

            // Assert
            var result = await _contactRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Title.ShouldBe("821357d99");
            result.Name.ShouldBe("25a0a1b49ba74141b3e3aa50be2ac6be084f3fb0bb1e43e5be73a504e9b28d3e0ac20833d2a94625b6320f28b56");
            result.Surname.ShouldBe("c93d8e66d571403dab2c0918b0f85aa5d07a57067cb84fd79ebaece697");
            result.ConfidentialName.ShouldBe("ce5383359c1f454ab2e0e7ead966a8c");
            result.JobRole.ShouldBe("a8eb1387fe964217bc9749fdc6844d770d77246e");
            result.BirthDate.ShouldBe(new DateTime(2016, 11, 2));
            result.MailInfo.ShouldBe("8e48c02390884096bddab3ad7eb5c145b3ffe4792b3d4434a73435c46a44e2601f6c1e7cdcfa48179668419ca853fcc6");
            result.PhoneInfo.ShouldBe("3c22a7c055f6441a812f2db9d096e98ffd14df27fe404b9dbbcc6ac325804bf62755ec7f90374d4f85b519e872");
            result.SocialInfo.ShouldBe("5ce6e75386384611964cfc9b980d5");
            result.AddressInfo.ShouldBe("dd9d26557e294bf28ea5b8e312ec013bd6bd7869058b475");
            result.Tag.ShouldBe("467a0506ae714235b5a92fb22d2be62ed2926b9de35c409a8ad1f14");
            result.Notes.ShouldBe("c55479fb57354aec9004b8f64f0c");

        }

        [Fact]
        public async Task UpdateAsync()
        {
            // Arrange
            var input = new ContactUpdateDto()
            {
                Title = "36c7d3b803cb4e12bad5c025505ef571bb8bf690868d4e",
                Name = "ddbcc992a9c1427fa6ec6ac58e1a307ae06c184a90264c578cbb5cc50a39a79d232fe6fc46554a3",
                Surname = "e8c4d1ebd59f438e8a3b68dab8afc5a1537bc0e697244743a6dfb3ef3eab8d",
                ConfidentialName = "c2151012c6",
                JobRole = "b7b557eb217f4237b0a4db9a38551e41a7fd96ce7b744470818a3b9144",
                BirthDate = new DateTime(2006, 4, 7),
                MailInfo = "e60ec8bb167c490bbdc02de0",
                PhoneInfo = "62d01d726bdb46c895bc101a05c9af4dd6581546005c4337a3ffd62cf120c6351c58a41eef",
                SocialInfo = "44e93297eac14ff4819de3609244f1daa4e900a706fb4abbbbde95e7cbbd6900871eec086bc64f3dbd24e7d74",
                AddressInfo = "e65c2f56cbc746b3b2cdedf9",
                Tag = "c6eb53b10fa",
                Notes = "d485bdbfa9eb4d8583635017d90f310f05ae57d9a3c44ad89b298b68743ec97b8e65c1564c02",

            };

            // Act
            var serviceResult = await _contactsAppService.UpdateAsync(Guid.Parse("12fa922b-1f43-4071-b46f-804e04828c70"), input);

            // Assert
            var result = await _contactRepository.FindAsync(c => c.Id == serviceResult.Id);

            result.ShouldNotBe(null);
            result.Title.ShouldBe("36c7d3b803cb4e12bad5c025505ef571bb8bf690868d4e");
            result.Name.ShouldBe("ddbcc992a9c1427fa6ec6ac58e1a307ae06c184a90264c578cbb5cc50a39a79d232fe6fc46554a3");
            result.Surname.ShouldBe("e8c4d1ebd59f438e8a3b68dab8afc5a1537bc0e697244743a6dfb3ef3eab8d");
            result.ConfidentialName.ShouldBe("c2151012c6");
            result.JobRole.ShouldBe("b7b557eb217f4237b0a4db9a38551e41a7fd96ce7b744470818a3b9144");
            result.BirthDate.ShouldBe(new DateTime(2006, 4, 7));
            result.MailInfo.ShouldBe("e60ec8bb167c490bbdc02de0");
            result.PhoneInfo.ShouldBe("62d01d726bdb46c895bc101a05c9af4dd6581546005c4337a3ffd62cf120c6351c58a41eef");
            result.SocialInfo.ShouldBe("44e93297eac14ff4819de3609244f1daa4e900a706fb4abbbbde95e7cbbd6900871eec086bc64f3dbd24e7d74");
            result.AddressInfo.ShouldBe("e65c2f56cbc746b3b2cdedf9");
            result.Tag.ShouldBe("c6eb53b10fa");
            result.Notes.ShouldBe("d485bdbfa9eb4d8583635017d90f310f05ae57d9a3c44ad89b298b68743ec97b8e65c1564c02");

        }

        [Fact]
        public async Task DeleteAsync()
        {
            // Act
            await _contactsAppService.DeleteAsync(Guid.Parse("12fa922b-1f43-4071-b46f-804e04828c70"));

            // Assert
            var result = await _contactRepository.FindAsync(c => c.Id == Guid.Parse("12fa922b-1f43-4071-b46f-804e04828c70"));

            result.ShouldBeNull();
        }
    }
}