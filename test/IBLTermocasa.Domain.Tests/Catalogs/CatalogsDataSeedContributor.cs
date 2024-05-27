using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;
using IBLTermocasa.Catalogs;

namespace IBLTermocasa.Catalogs
{
    public class CatalogsDataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private bool IsSeeded = false;
        private readonly ICatalogRepository _catalogRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public CatalogsDataSeedContributor(ICatalogRepository catalogRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _catalogRepository = catalogRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (IsSeeded)
            {
                return;
            }

            await _catalogRepository.InsertAsync(new Catalog
            (
                id: Guid.Parse("ae5a5088-4847-445c-b018-0145ccc4a842"),
                name: "f9dc259290a243719498be978cf87f351aed289af0fd429b967607",
                from: new DateTime(2004, 6, 23),
                to: new DateTime(2013, 3, 20),
                description: "d730f25de1af4afab481dd"
            ));

            await _catalogRepository.InsertAsync(new Catalog
            (
                id: Guid.Parse("edd79e7d-3dc7-4b25-9ccd-1a97cb1a6c38"),
                name: "468ee521986645b7b78a03fff9b6b4902",
                from: new DateTime(2006, 7, 23),
                to: new DateTime(2008, 7, 3),
                description: "6747042f63a446359552618cffa9a70b6289eb46cc0543c08d497"
            ));

            await _unitOfWorkManager!.Current!.SaveChangesAsync();

            IsSeeded = true;
        }
    }
}