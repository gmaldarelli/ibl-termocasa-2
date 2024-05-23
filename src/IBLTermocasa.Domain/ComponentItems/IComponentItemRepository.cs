using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace IBLTermocasa.ComponentItems
{
    public partial interface IComponentItemRepository : IRepository<ComponentItem, Guid>
    {
        Task<List<ComponentItem>> GetListByComponentIdAsync(
    Guid componentId,
    string? sorting = null,
    int maxResultCount = int.MaxValue,
    int skipCount = 0,
    CancellationToken cancellationToken = default
);

        Task<long> GetCountByComponentIdAsync(Guid componentId, CancellationToken cancellationToken = default);

        Task<List<ComponentItemWithNavigationProperties>> GetListWithNavigationPropertiesByComponentIdAsync(
            Guid componentId,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<ComponentItemWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<ComponentItemWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            bool? isDefault = null,
            Guid? materialId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<ComponentItem>> GetListAsync(
                    string? filterText = null,
                    bool? isDefault = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            bool? isDefault = null,
            Guid? materialId = null,
            CancellationToken cancellationToken = default);
    }
}