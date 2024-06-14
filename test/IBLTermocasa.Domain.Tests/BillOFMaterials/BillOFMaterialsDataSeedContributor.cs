using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using IBLTermocasa.BillOfMaterials;
using IBLTermocasa.Common;

namespace IBLTermocasa.BillOfMaterials
{
    public class BillOFMaterialsDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly IBillOFMaterialRepository _billOFMaterialRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public BillOFMaterialsDataSeedContributor(IBillOFMaterialRepository billOFMaterialRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _billOFMaterialRepository = billOFMaterialRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _billOFMaterialRepository.InsertAsync(new BillOFMaterial
            (
                id: Guid.Parse("69297841-8d3b-44de-a9dd-87fdbdf73964"),
                name: "678705e217e042a0bb87467f7e84609bdeab0599408d4243ba43b2ce40c191053e21e974109c40cdb9d8bad64c5c3145a1e",
                requestForQuotationProperty: new RequestForQuotationProperty(),
                listItems: new List<BOMItem>()
            ));

            await _billOFMaterialRepository.InsertAsync(new BillOFMaterial
            (
                id: Guid.Parse("4b23ffce-c8af-456d-96ad-3db8439a9117"),
                name: "8eda306c76434c1abd18f4ab5f534e5c34f83ab4710a4e8c848502e9649685db74fd6b650ef64098974cde7cdbde759",
                requestForQuotationProperty: new RequestForQuotationProperty(),
                listItems: new List<BOMItem>()
            ));

            await _unitOfWorkManager!.Current!.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}