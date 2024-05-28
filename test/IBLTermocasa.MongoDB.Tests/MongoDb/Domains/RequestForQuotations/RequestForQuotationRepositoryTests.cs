using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;
using IBLTermocasa.Common;
using IBLTermocasa.RequestForQuotations;
using IBLTermocasa.MongoDB;
using Xunit;

namespace IBLTermocasa.MongoDB.Domains.RequestForQuotations
{
    [Collection(IBLTermocasaTestConsts.CollectionDefinitionName)]
    public class RequestForQuotationRepositoryTests : IBLTermocasaMongoDbTestBase
    {
        private readonly IRequestForQuotationRepository _requestForQuotationRepository;

        public RequestForQuotationRepositoryTests()
        {
            _requestForQuotationRepository = GetRequiredService<IRequestForQuotationRepository>();
        }

        [Fact]
        public async Task GetListAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _requestForQuotationRepository.GetListAsync(
                    quoteNumber: "6b08063686094e4f85272938a43c9067541d0a56587043f7a60bb94b357615878adbe6c2f",
                    workSite: "91754491a73344d5a15ed1a7dcae3e52e4148c0f807d49bb95c9d2fbe8a5",
                    city: "0dad859077014c528e0c5e17d422bcaaa452f78f77874ea483d887d03e5e1879c8326ff40fca4de3ae73f0a39218d32c",
                    organizationProperty:  new OrganizationProperty(),
                    contactProperty:  new ContactProperty(),
                    phoneInfo: new PhoneInfo(),
                    mailInfo: new MailInfo(),
                    description: "3dd3810d82f0412197af9e3e295b446656d2847c0a89405",
                    status: default
                );

                // Assert
                result.Count.ShouldBe(1);
                result.FirstOrDefault().ShouldNotBe(null);
                result.First().Id.ShouldBe(Guid.Parse("de88a145-0c74-4b77-9f92-df32c6a6bc4e"));
            });
        }

        [Fact]
        public async Task GetCountAsync()
        {
            // Arrange
            await WithUnitOfWorkAsync(async () =>
            {
                // Act
                var result = await _requestForQuotationRepository.GetCountAsync(
                    quoteNumber: "4b29a9eb71844285a13a58228038266f1870be34e39f45b0aa05b2188b630919d6d54ce5afa34ab19a68cfca9e027e013",
                    workSite: "bce0c34b2d5a4935a055f4d0d775583bf57120f6030344219bfefc24a4",
                    city: "91f19e2afc604bb1a5576b91578ae2d5e4b7dcddc5a64e1c836cd5b4fa6732a9f9089dfe3b5242ee8be0",
                    organizationProperty:  new OrganizationProperty(),
                    contactProperty:  new ContactProperty(),
                    phoneInfo: new PhoneInfo(),
                    mailInfo: new MailInfo(),
                    description: "f6fcb38c9a3d4391838ed66db1ccb97b25e",
                    status: default
                );

                // Assert
                result.ShouldBe(1);
            });
        }
    }
}