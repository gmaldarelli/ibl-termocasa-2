using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace IBLTermocasa.Subproducts
{
    public partial interface ISubproductRepository : IRepository<Subproduct, Guid>
    {
        Task<List<Subproduct>> GetListByProductIdAsync(
    Guid productId,
    string? sorting = null,
    int maxResultCount = int.MaxValue,
    int skipCount = 0,
    CancellationToken cancellationToken = default
);

        Task<long> GetCountByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);

        Task<List<SubproductWithNavigationProperties>> GetListWithNavigationPropertiesByProductIdAsync(
            Guid productId,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<SubproductWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<SubproductWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            int? orderMin = null,
            int? orderMax = null,
            string? name = null,
            bool? isSingleProduct = null,
            bool? mandatory = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<Subproduct>> GetListAsync(
                    string? filterText = null,
                    int? orderMin = null,
                    int? orderMax = null,
                    string? name = null,
                    bool? isSingleProduct = null,
                    bool? mandatory = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            int? orderMin = null,
            int? orderMax = null,
            string? name = null,
            bool? isSingleProduct = null,
            bool? mandatory = null,
            CancellationToken cancellationToken = default);
    }
}