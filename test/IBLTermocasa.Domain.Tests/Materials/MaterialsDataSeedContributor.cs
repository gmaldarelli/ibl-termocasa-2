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
                id: Guid.Parse("7a33ea5e-e9f7-473e-85b2-78738b1f8e7c"),
                code: "860a3bc448c048b",
                name: "16b08a8b23e94fbb920af5cbc4eea81e4098c6db820b400a85e6becb3",
                measureUnit: default,
                quantity: 1509127397,
                lifo: 1997146868,
                standardPrice: 21630560,
                averagePrice: 582906729,
                lastPrice: 124909221,
                averagePriceSecond: 490993074
            ));

            await _materialRepository.InsertAsync(new Material
            (
                id: Guid.Parse("91b4cf12-71b6-4e07-934c-4c3f72cbf91e"),
                code: "3c399c1ab1a8411ebb1bc3ca08f6dfb21e5c7ce1079b4d538e456db51766b56e69d51632eebf4400a",
                name: "2e4744027935412ea1ac54e6e32447a6a121809719dc40dea1",
                measureUnit: default,
                quantity: 1678642071,
                lifo: 20426592,
                standardPrice: 106804149,
                averagePrice: 690952447,
                lastPrice: 1529066953,
                averagePriceSecond: 2138273495
            ));

            await _unitOfWorkManager!.Current!.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}