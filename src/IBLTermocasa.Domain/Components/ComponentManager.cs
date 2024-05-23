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
    public abstract class ComponentManagerBase : DomainService
    {
        protected IComponentRepository _componentRepository;

        public ComponentManagerBase(IComponentRepository componentRepository)
        {
            _componentRepository = componentRepository;
        }

        public virtual async Task<Component> CreateAsync(
        string name)
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var component = new Component(
             GuidGenerator.Create(),
             name
             );

            return await _componentRepository.InsertAsync(component);
        }

        public virtual async Task<Component> UpdateAsync(
            Guid id,
            string name, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(name, nameof(name));

            var component = await _componentRepository.GetAsync(id);

            component.Name = name;

            component.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _componentRepository.UpdateAsync(component);
        }

    }
}