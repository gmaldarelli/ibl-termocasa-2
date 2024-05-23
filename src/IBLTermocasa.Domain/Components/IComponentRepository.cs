using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace IBLTermocasa.Components
{
    public partial interface IComponentRepository : IRepository<Component, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? name = null,
            CancellationToken cancellationToken = default);
        Task<List<Component>> GetListAsync(
                    string? filterText = null,
                    string? name = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? name = null,
            CancellationToken cancellationToken = default);
    }
}