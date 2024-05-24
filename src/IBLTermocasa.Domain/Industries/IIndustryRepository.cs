using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace IBLTermocasa.Industries
{
    public interface IIndustryRepository : IRepository<Industry, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? code = null,
            string? description = null,
            CancellationToken cancellationToken = default);
        Task<List<Industry>> GetListAsync(
                    string? filterText = null,
                    string? code = null,
                    string? description = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? code = null,
            string? description = null,
            CancellationToken cancellationToken = default);
    }
}