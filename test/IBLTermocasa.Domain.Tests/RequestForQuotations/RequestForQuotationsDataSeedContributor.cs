using IBLTermocasa.Organizations;
using IBLTermocasa.Contacts;
using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using IBLTermocasa.RequestForQuotations;

namespace IBLTermocasa.RequestForQuotations
{
    public class RequestForQuotationsDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly IRequestForQuotationRepository _requestForQuotationRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ContactsDataSeedContributor _contactsDataSeedContributor;

        private readonly OrganizationsDataSeedContributor _organizationsDataSeedContributor;

        public RequestForQuotationsDataSeedContributor(IRequestForQuotationRepository requestForQuotationRepository, IUnitOfWorkManager unitOfWorkManager, ContactsDataSeedContributor contactsDataSeedContributor, OrganizationsDataSeedContributor organizationsDataSeedContributor)
        {
            _requestForQuotationRepository = requestForQuotationRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _contactsDataSeedContributor = contactsDataSeedContributor; _organizationsDataSeedContributor = organizationsDataSeedContributor;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _contactsDataSeedContributor.SeedAsync(context);
            await _organizationsDataSeedContributor.SeedAsync(context);

            await _requestForQuotationRepository.InsertAsync(new RequestForQuotation
            (
                id: Guid.Parse("de88a145-0c74-4b77-9f92-df32c6a6bc4e"),
                quoteNumber: "6b08063686094e4f85272938a43c9067541d0a56587043f7a60bb94b357615878adbe6c2f",
                workSite: "91754491a73344d5a15ed1a7dcae3e52e4148c0f807d49bb95c9d2fbe8a5",
                city: "0dad859077014c528e0c5e17d422bcaaa452f78f77874ea483d887d03e5e1879c8326ff40fca4de3ae73f0a39218d32c",
                organizationProperty: "755eaf9f173d4b",
                contactProperty: "f17a24eab35a4ddc961f42163fe093cc0ce57ac14b6e45cc925a8809fd1bb59337273891ab",
                phoneInfo: "c18edca40a7d424da79ab155d988b6074d0563b8260244",
                mailInfo: "ef100bc029f94cdf92f0",
                discount: 376578900,
                description: "3dd3810d82f0412197af9e3e295b446656d2847c0a89405",
                status: default,
                agentId: null,
                contactId: null,
                organizationId: null
            ));

            await _requestForQuotationRepository.InsertAsync(new RequestForQuotation
            (
                id: Guid.Parse("b07b21f0-04fb-4039-9454-390c10206801"),
                quoteNumber: "4b29a9eb71844285a13a58228038266f1870be34e39f45b0aa05b2188b630919d6d54ce5afa34ab19a68cfca9e027e013",
                workSite: "bce0c34b2d5a4935a055f4d0d775583bf57120f6030344219bfefc24a4",
                city: "91f19e2afc604bb1a5576b91578ae2d5e4b7dcddc5a64e1c836cd5b4fa6732a9f9089dfe3b5242ee8be0",
                organizationProperty: "bcae71333fd54daea62dbebe41d12fc9a1e82a17",
                contactProperty: "3e2202a08c22414d88ca3dd2f0163cb48c9179269b4242c68a0b724ee793250f322b564a2f7847bda780cf8e51c",
                phoneInfo: "ce7a1b4a35064285b405a18cd9ac818002c1763e0e4c4e5da7",
                mailInfo: "58f0252fbac74fbd812d3ebdbe4ec4e51cde313ba36b4941bf84bac4ab2890362576b633c684410f9bbba73c",
                discount: 2000374377,
                description: "f6fcb38c9a3d4391838ed66db1ccb97b25e",
                status: default,
                agentId: null,
                contactId: null,
                organizationId: null
            ));

            await _unitOfWorkManager!.Current!.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}