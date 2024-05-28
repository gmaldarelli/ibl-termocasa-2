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
                id: Guid.Parse("20cbea78-9a26-48c0-a0ec-bf6ebd0c8e87"),
                code: "07ea10fbd7a1445abafe9be472dc51",
                name: "6fdedd93112848b98f76e7b552a2a533d0d9e80311f242d8810498b6260090c37adb4a2909354b",
                measureUnit: default,
                quantity: 65033568,
                lifo: 1361326440,
                standardPrice: 1213020104,
                averagePrice: 1013794078,
                lastPrice: 1082523199,
                averagePriceSecond: 615218975
            ));

            await _materialRepository.InsertAsync(new Material
            (
                id: Guid.Parse("6084f99f-8171-40db-b346-1a77b255a697"),
                code: "3a9a6e7d66394eeaacdddd94f40a855d83a4294bf9914ac595d6bf9224cf740d3feb49e522a54f49",
                name: "2230813eb238465ba8288175a792ef6797a5d862034646a78711ec061efb4526b2a8ddc826114cb69bec3b139b9c8fd00a6",
                measureUnit: default,
                quantity: 1611656380,
                lifo: 286191257,
                standardPrice: 1325944952,
                averagePrice: 1995146924,
                lastPrice: 1990308698,
                averagePriceSecond: 190599140
            ));

            await _unitOfWorkManager!.Current!.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}