using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace IBLTermocasa.ProfessionalProfiles
{
    public interface IProfessionalProfileRepository : IRepository<ProfessionalProfile, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? name = null,
            double? standardPriceMin = null,
            double? standardPriceMax = null,
            CancellationToken cancellationToken = default);
        Task<List<ProfessionalProfile>> GetListAsync(
                    string? filterText = null,
                    string? name = null,
                    double? standardPrice = null,
                    double? standardPriceMin = null,
                    double? standardPriceMax = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? name = null,
            double? standardPrice = null,
            double? standardPriceMin = null,
            double? standardPriceMax = null,
            CancellationToken cancellationToken = default);
    }
}