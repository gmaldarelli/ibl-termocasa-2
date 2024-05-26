using IBLTermocasa.RequestForQuotations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace IBLTermocasa.RequestForQuotations
{
    public interface IRequestForQuotationRepository : IRepository<RequestForQuotation, Guid>
    {
        Task<RequestForQuotationWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<RequestForQuotationWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? quoteNumber = null,
            string? workSite = null,
            string? city = null,
            string? organizationProperty = null,
            string? contactProperty = null,
            string? phoneInfo = null,
            string? mailInfo = null,
            decimal? discountMin = null,
            decimal? discountMax = null,
            string? description = null,
            Status? status = null,
            Guid? agentId = null,
            Guid? contactId = null,
            Guid? organizationId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<RequestForQuotation>> GetListAsync(
                    string? filterText = null,
                    string? quoteNumber = null,
                    string? workSite = null,
                    string? city = null,
                    string? organizationProperty = null,
                    string? contactProperty = null,
                    string? phoneInfo = null,
                    string? mailInfo = null,
                    decimal? discountMin = null,
                    decimal? discountMax = null,
                    string? description = null,
                    Status? status = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? quoteNumber = null,
            string? workSite = null,
            string? city = null,
            string? organizationProperty = null,
            string? contactProperty = null,
            string? phoneInfo = null,
            string? mailInfo = null,
            decimal? discountMin = null,
            decimal? discountMax = null,
            string? description = null,
            Status? status = null,
            Guid? agentId = null,
            Guid? contactId = null,
            Guid? organizationId = null,
            CancellationToken cancellationToken = default);
    }
}