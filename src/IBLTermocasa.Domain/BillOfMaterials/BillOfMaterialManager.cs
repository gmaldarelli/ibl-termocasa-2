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
    public class BillOfMaterialManager : DomainService
    {
        protected IBillOfMaterialRepository _billOfMaterialRepository;

        public BillOfMaterialManager(IBillOfMaterialRepository billOfMaterialRepository)
        {
            _billOfMaterialRepository = billOfMaterialRepository;
        }
        
        public virtual async Task<BillOfMaterial> CreateAsync(BillOfMaterial billOfMaterial)
        {
            Check.NotNull(billOfMaterial, nameof(billOfMaterial));
            return await _billOfMaterialRepository.InsertAsync(billOfMaterial);
        }

        public virtual async Task<BillOfMaterial> UpdateAsync(Guid id, BillOfMaterial billOfMaterial)
        {
            Check.NotNull(billOfMaterial, nameof(billOfMaterial));
            var existingBillOfMaterial = await _billOfMaterialRepository.GetAsync(id);
            BillOfMaterial.FillPropertiesForUpdate(billOfMaterial, existingBillOfMaterial);
            return await _billOfMaterialRepository.UpdateAsync(existingBillOfMaterial);
        }
    }
}