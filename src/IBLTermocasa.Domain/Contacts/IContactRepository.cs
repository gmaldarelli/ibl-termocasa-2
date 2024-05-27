using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace IBLTermocasa.Contacts
{
    public interface IContactRepository : IRepository<Contact, Guid>
    {
        Task<List<Contact>> GetListAsync(
            string? filterText = null,
            string? title = null,
            string? name = null,
            string? surname = null,
            string? confidentialName = null,
            string? jobRole = null,
            string? mailInfo = null,
            string? phoneInfo = null,
            string? addressInfo = null,
            string? tag = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string? filterText = null,
            string? title = null,
            string? name = null,
            string? surname = null,
            string? confidentialName = null,
            string? jobRole = null,
            string? mailInfo = null,
            string? phoneInfo = null,
            string? addressInfo = null,
            string? tag = null,
            CancellationToken cancellationToken = default);
    }
}