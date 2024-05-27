using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using IBLTermocasa.Materials;

namespace IBLTermocasa.Materials
{
    public class MaterialsDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly IMaterialRepository _materialRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public MaterialsDataSeedContributor(IMaterialRepository materialRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _materialRepository = materialRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _materialRepository.InsertAsync(new Material
            (
                id: Guid.Parse("113ff063-4359-4c2b-9270-fc257e4c6258"),
                code: "14395ed627fa4c48a2ab9c7d5f88a21f36d4c28",
                name: "acea513193b9400fbc1ebec968e57",
                measureUnit: default,
                quantity: 1106131168,
                lifo: 1659121399,
                standardPrice: 695549082,
                averagePrice: 656622496,
                lastPrice: 1740405910,
                averagePriceSecond: 476452139
            ));

            await _materialRepository.InsertAsync(new Material
            (
                id: Guid.Parse("92cd78ea-760e-4c73-a1da-810a1acf28d6"),
                code: "78efb9fd003b4825a2c2057ec45cb2c79fe23f08c1",
                name: "86c447606e1f48fba8629229a9b78d2507c8deabad584e2fb9f12e1bde2",
                measureUnit: default,
                quantity: 1355299485,
                lifo: 942665376,
                standardPrice: 910522389,
                averagePrice: 141963053,
                lastPrice: 438255412,
                averagePriceSecond: 1467442144
            ));

            await _unitOfWorkManager!.Current!.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}