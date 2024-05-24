using IBLTermocasa.Types;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace IBLTermocasa.Organizations
{
    public interface IOrganizationRepository : IRepository<Organization, Guid>
    {
        Task<OrganizationWithNavigationProperties> GetWithNavigationPropertiesAsync(
    Guid id,
    CancellationToken cancellationToken = default
);

        Task<List<OrganizationWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? code = null,
            string? name = null,
            OrganizationType? organizationType = null,
            string? mailInfo = null,
            string? phoneInfo = null,
            string? tags = null,
            Guid? industryId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<Organization>> GetListAsync(
                    string? filterText = null,
                    string? code = null,
                    string? name = null,
                    OrganizationType? organizationType = null,
                    string? mailInfo = null,
                    string? phoneInfo = null,
                    string? tags = null,
                    string? sorting = null,
                    int maxResultCount = int.MaxValue,
                    int skipCount = 0,
                    CancellationToken cancellationToken = default
                );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? code = null,
            string? name = null,
            OrganizationType? organizationType = null,
            string? mailInfo = null,
            string? phoneInfo = null,
            string? tags = null,
            Guid? industryId = null,
            CancellationToken cancellationToken = default);
    }
}