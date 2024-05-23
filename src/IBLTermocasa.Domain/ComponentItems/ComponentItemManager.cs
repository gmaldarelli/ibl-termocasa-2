using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace IBLTermocasa.ComponentItems
{
    public abstract class ComponentItemManagerBase : DomainService
    {
        protected IComponentItemRepository _componentItemRepository;

        public ComponentItemManagerBase(IComponentItemRepository componentItemRepository)
        {
            _componentItemRepository = componentItemRepository;
        }

        public virtual async Task<ComponentItem> CreateAsync(
        Guid componentId, Guid materialId, bool isDefault)
        {
            Check.NotNull(materialId, nameof(materialId));

            var componentItem = new ComponentItem(
             GuidGenerator.Create(),
             componentId, materialId, isDefault
             );

            return await _componentItemRepository.InsertAsync(componentItem);
        }

        public virtual async Task<ComponentItem> UpdateAsync(
            Guid id,
            Guid componentId, Guid materialId, bool isDefault
        )
        {
            Check.NotNull(materialId, nameof(materialId));

            var componentItem = await _componentItemRepository.GetAsync(id);

            componentItem.ComponentId = componentId;
            componentItem.MaterialId = materialId;
            componentItem.IsDefault = isDefault;

            return await _componentItemRepository.UpdateAsync(componentItem);
        }

    }
}