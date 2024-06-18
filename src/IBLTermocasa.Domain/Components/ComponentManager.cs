using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace IBLTermocasa.Components
{
    public class ComponentManager : DomainService
    {
        protected IComponentRepository _componentRepository;

        public ComponentManager(IComponentRepository componentRepository)
        {
            _componentRepository = componentRepository;
        }

        public virtual async Task<Component> CreateAsync(Component entityInput)
        {
            Check.NotNull(entityInput, nameof(entityInput));
            Check.NotNullOrWhiteSpace(entityInput.Name, nameof(entityInput.Name));

            var component = new Component(
                id: GuidGenerator.Create(),
                code: entityInput.Code,
                name: entityInput.Name,
                componentItems: entityInput.ComponentItems
             );
            return await _componentRepository.InsertAsync(component);
        }

        public virtual async Task<Component> UpdateAsync(Guid id, Component entityInput)
        {
            Check.NotNull(entityInput, nameof(entityInput));
            Check.NotNullOrWhiteSpace(entityInput.Code, nameof(entityInput.Code));
            Check.NotNullOrWhiteSpace(entityInput.Name, nameof(entityInput.Name));

            var component = await _componentRepository.GetAsync(id);
            component = Component.FillPropertiesForUpdate(entityInput, component);
            component.Name = entityInput.Name;
            component.ComponentItems = entityInput.ComponentItems;
            component.SetConcurrencyStampIfNotNull(entityInput.ConcurrencyStamp);
            return await _componentRepository.UpdateAsync(component);
        }

    }
}