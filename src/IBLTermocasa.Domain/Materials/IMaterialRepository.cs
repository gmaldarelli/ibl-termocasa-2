using IBLTermocasa.Types;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace IBLTermocasa.Materials
{
    public interface IMaterialRepository : IRepository<Material, Guid>
    {
        Task<List<Material>> GetListAsync(
            string? filterText = null,
            string? code = null,
            string? name = null,
            SourceType? sourceType = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? code = null,
            string? name = null,
            SourceType? sourceType = null,
            CancellationToken cancellationToken = default);
    }
}