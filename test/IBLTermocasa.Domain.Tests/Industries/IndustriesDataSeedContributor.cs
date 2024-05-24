using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using IBLTermocasa.Industries;

namespace IBLTermocasa.Industries
{
    public class IndustriesDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly IIndustryRepository _industryRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public IndustriesDataSeedContributor(IIndustryRepository industryRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _industryRepository = industryRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _industryRepository.InsertAsync(new Industry
            (
                id: Guid.Parse("0ec6c141-e1b9-454f-bb53-357a250bba54"),
                code: "ec9a28bcda614ef3a7acd926dbbcc6eb59af572f326549558f4fa16fee3d3402117c3b6d18e54ca4871466aeb31645a3a9e9475c894e401da57f0cf5b59f261e4a154ce5dff443059d15420f0d9ff400fec956df2a784cd9a32def945ffc408d9955cbe3dc7b4b7ca525cb488ff23bcea3ae806887cf4663ba3ea9e4a311b24",
                description: "275263b4be564a2386eefc04466c6cee3ff639ef05ca4687a2547904d6eb42d1b72c3217a9ed454b85b2cd1ec6ae10f08a134b909eb7428e90a4196d1dbd735715f9c784b3a84917a3a527cc5b732ec2eada691b275d424aa61d3ba022c10d5a4f3616459f7f444c8e54fa63ba25bee2b3447408736340deb558a74e3d0f747fc848dff90be9457a971aa5957c307c0dfb681a7ec6434ff28e41c7ebde6f0294eb9c0bd8b526472dbafcee301bde38659661560f3d1b487386dd16e2b2b24d3593c61242a47842a0b3b66d4172be438094c914b8cda04fb680de193da81f167b267de39b1dc4436cb15ef8f5096748c19c763e51f54a435e9baf"
            ));

            await _industryRepository.InsertAsync(new Industry
            (
                id: Guid.Parse("87d3616a-ab6f-40ed-8b20-4f1af33acf98"),
                code: "a9a3e2843b304d679e9264b7df4de79e11262aa6a80a434ca10ec563b23bac716f2c8ab30b0a464ab4e3fcea972d9e7bce8c59819be744449652e7886c5e47701edcc6050b80416d89670416356416f828995fd34fa245c08d04d1004607ba0b17b8fcb2ddb7443fba8b77c13e75ddf3aeca4efb33704b238763e2fadf269a3",
                description: "6a61a36070d94629935eed3bc203260a0d8859de5c5c4930af30448ef0db0a315e90f0e964d94594add1ade294a6ec59cdcfab8ebf4e43e9b9d92be47d5214ec337fcd6751484caaa0e58645233dd54bde892be31c4541ffba3187e3776d98874cb6a450d9604831b4e5b4874069d9b035a184c417ac43699452a332103007d44c3c6f156e4f40288979e278c9d2a2e5463c24c28852406ebb81eb4dfc177cadaf574736318c4cd2b2049f59062f6785a4886b6651864a4b8a37e61ed43ebcdff9f0f75367ec437f831cf00c078b4c5c9c89ce48a5b04bf68e443459120d27aac951b670591b498aa76dd052d94adf0aac49474c32a44040990a"
            ));

            await _unitOfWorkManager!.Current!.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}