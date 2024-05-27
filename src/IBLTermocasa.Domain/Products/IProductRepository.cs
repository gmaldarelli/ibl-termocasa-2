using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace IBLTermocasa.Products
{
    public interface IProductRepository : IRepository<Product, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? code = null,
            string? name = null,
            string? description = null,
            bool? isAssembled = null,
            bool? isInternal = null,
            CancellationToken cancellationToken = default);
        Task<ProductWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );

        Task<List<ProductWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? code = null,
            string? name = null,
            string? description = null,
            bool? isAssembled = null,
            bool? isInternal = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<Product>> GetListAsync(
                    string? filterText = null,
                    string? code = null,
                    string? name = null,
                    string? description = null,
                    bool? isAssembled = null,
                    bool? isInternal = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? code = null,
            string? name = null,
            string? description = null,
            bool? isAssembled = null,
            bool? isInternal = null,
            CancellationToken cancellationToken = default);
    }
}