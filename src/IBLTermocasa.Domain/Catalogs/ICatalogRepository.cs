using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace IBLTermocasa.Catalogs
{
    public interface ICatalogRepository : IRepository<Catalog, Guid>
    {
        Task<CatalogWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<Catalog>> GetListAsync(
                    string? filterText = null,
                    string? name = null,
                    DateTime? fromMin = null,
                    DateTime? fromMax = null,
                    DateTime? toMin = null,
                    DateTime? toMax = null,
                    string? description = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? name = null,
            DateTime? fromMin = null,
            DateTime? fromMax = null,
            DateTime? toMin = null,
            DateTime? toMax = null,
            string? description = null,
            CancellationToken cancellationToken = default);
        
        Task<List<CatalogWithNavigationProperties>> GetListCatalogWithProducts(
            string? filterText = null,
            string? name = null,
            DateTime? fromMin = null,
            DateTime? fromMax = null,
            DateTime? toMin = null,
            DateTime? toMax = null,
            string? description = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );
    }
}