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
                id: Guid.Parse("a277f0ce-c5ed-4957-9b49-9cdc39c6bb90"),
                name: "b8b5b7d130954994afc0877be341ab33ed1e8a6306704f"
            ));

            await _componentRepository.InsertAsync(new Component
            (
                id: Guid.Parse("2519e506-b867-4e8b-b1c8-8c7a3085baef"),
                name: "6a6a917e366d4221b16"
            ));

            await _unitOfWorkManager!.Current!.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}