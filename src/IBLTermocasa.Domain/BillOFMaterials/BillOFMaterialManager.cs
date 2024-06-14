using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace IBLTermocasa.BillOfMaterials
{
    public class BillOFMaterialManager : DomainService
    {
        protected IBillOFMaterialRepository _billOFMaterialRepository;

        public BillOFMaterialManager(IBillOFMaterialRepository billOFMaterialRepository)
        {
            _billOFMaterialRepository = billOFMaterialRepository;
        }
        
        public virtual async Task<BillOFMaterial> CreateAsync(BillOFMaterial billOfMaterial)
        {
            Check.NotNull(billOfMaterial, nameof(billOfMaterial));
            return await _billOFMaterialRepository.InsertAsync(billOfMaterial);
        }

        public virtual async Task<BillOFMaterial> UpdateAsync(Guid id, BillOFMaterial billOfMaterial)
        {
            Check.NotNull(billOfMaterial, nameof(billOfMaterial));
            var existingBillOfMaterial = await _billOFMaterialRepository.GetAsync(id);
            BillOFMaterial.FillPropertiesForUpdate(billOfMaterial, existingBillOfMaterial);
            return await _billOFMaterialRepository.UpdateAsync(existingBillOfMaterial);
        }
    }
}