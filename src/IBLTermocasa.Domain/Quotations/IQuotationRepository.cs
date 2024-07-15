using IBLTermocasa.Types;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace IBLTermocasa.Quotations
{
    public interface IQuotationRepository : IRepository<Quotation, Guid>
    {
        Task<List<Quotation>> GetListAsync(
            string? filterText = null,
            string? code = null,
            string? name = null,
            DateTime? sentDateMin = null,
            DateTime? sentDateMax = null,
            DateTime? quotationValidDateMin = null,
            DateTime? quotationValidDateMax = null,
            DateTime? confirmedDateMin = null,
            DateTime? confirmedDateMax = null,
            QuotationStatus? status = null,
            bool? depositRequired = null,
            double? depositRequiredValueMin = null,
            double? depositRequiredValueMax = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? code = null,
            string? name = null,
            DateTime? sentDateMin = null,
            DateTime? sentDateMax = null,
            DateTime? quotationValidDateMin = null,
            DateTime? quotationValidDateMax = null,
            DateTime? confirmedDateMin = null,
            DateTime? confirmedDateMax = null,
            QuotationStatus? status = null,
            bool? depositRequired = null,
            double? depositRequiredValueMin = null,
            double? depositRequiredValueMax = null,
            CancellationToken cancellationToken = default);
    }
}