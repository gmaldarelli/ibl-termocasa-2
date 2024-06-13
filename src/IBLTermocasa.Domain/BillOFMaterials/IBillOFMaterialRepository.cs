using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IBLTermocasa.Common;
using Volo.Abp.Domain.Repositories;

namespace IBLTermocasa.BillOFMaterials
{
    public interface IBillOFMaterialRepository : IRepository<BillOFMaterial, Guid>
    {

        Task DeleteAllAsync(
            string? filterText = null,
            string? name = null,
            RequestForQuotationProperty? requestForQuotationId = null,
            CancellationToken cancellationToken = default);
        Task<List<BillOFMaterial>> GetListAsync(
                    string? filterText = null,
                    string? name = null,
                    RequestForQuotationProperty? requestForQuotationId = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? name = null,
            RequestForQuotationProperty? requestForQuotationId = null,
            CancellationToken cancellationToken = default);
    }
}