using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace IBLTermocasa.ConsumptionEstimations
{
    public interface IConsumptionEstimationRepository : IRepository<ConsumptionEstimation, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            Guid? productId = null,
            CancellationToken cancellationToken = default);
        Task<List<ConsumptionEstimation>> GetListAsync(
                    string? filterText = null,
                    Guid? productId = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            Guid? productId = null,
            CancellationToken cancellationToken = default);
    }
}