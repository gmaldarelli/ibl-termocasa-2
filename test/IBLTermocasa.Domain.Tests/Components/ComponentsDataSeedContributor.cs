using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using IBLTermocasa.Components;

namespace IBLTermocasa.Components
{
    public class ComponentsDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly IComponentRepository _componentRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public ComponentsDataSeedContributor(IComponentRepository componentRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _componentRepository = componentRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _componentRepository.InsertAsync(new Component
            (
                id: Guid.Parse("93c4cb63-038b-48ba-8d3f-9a16fb6fa8b2"),
                name: "5216cc1c456a489c830a",
                componentItems: new System.Collections.Generic.List<ComponentItem>
                {
                    new ComponentItem
                    (
                        id: Guid.Parse("f3b3b3b3-3b3b-3b3b-3b3b-3b3b3b3b3b3b"),
                        materialId: Guid.Parse("f3b3b3b3-3b3b-3b3b-3b3b-3b3b3b3b3b3b"),
                        isDefault: true
                    )
                }
            ));

            await _componentRepository.InsertAsync(new Component
            (
                id: Guid.Parse("679c6836-e338-4e7e-b8f5-7c0cc6f103d0"),
                name: "7e943dde2d0548d2b909b8061fca",
                componentItems: new System.Collections.Generic.List<ComponentItem>
                {
                    new ComponentItem
                    (
                        id: Guid.Parse("f3b3b3b3-3b3b-3b3b-3b3b-3b3b3b3b3b3b"),
                        materialId: Guid.Parse("f3b3b3b3-3b3b-3b3b-3b3b-3b3b3b3b3b3b"),
                        isDefault: true
                    )
                }
            ));

            await _unitOfWorkManager!.Current!.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}