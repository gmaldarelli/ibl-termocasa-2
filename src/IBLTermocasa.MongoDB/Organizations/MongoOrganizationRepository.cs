using IBLTermocasa.Types;
using IBLTermocasa.Industries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using IBLTermocasa.MongoDB;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;
using MongoDB.Driver.Linq;
using MongoDB.Driver;

namespace IBLTermocasa.Organizations
{
    public class MongoOrganizationRepository : MongoDbRepository<IBLTermocasaMongoDbContext, Organization, Guid>, IOrganizationRepository
    {
        public MongoOrganizationRepository(IMongoDbContextProvider<IBLTermocasaMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public virtual async Task<OrganizationWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var organization = await (await GetMongoQueryableAsync(cancellationToken))
                .FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));

            var industry = await (await GetMongoQueryableAsync<Industry>(cancellationToken)).FirstOrDefaultAsync(e => e.Id == organization.IndustryId, cancellationToken: cancellationToken);

            return new OrganizationWithNavigationProperties
            {
                Organization = organization,
                Industry = industry,

            };
        }

        public virtual async Task<List<OrganizationWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            string? code = null,
            string? name = null,
            OrganizationType? organizationType = null,
            string? mailInfo = null,
            string? phoneInfo = null,
            string? tags = null,
            Guid? industryId = null,
            OrganizationType? organizationTypePreFiilter = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, code, name, organizationType, mailInfo, phoneInfo, tags, industryId, organizationTypePreFiilter);
            var organizations = await query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? OrganizationConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                .As<IMongoQueryable<Organization>>()
                .PageBy<Organization, IMongoQueryable<Organization>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            var dbContext = await GetDbContextAsync(cancellationToken);
            return organizations.Select(s => new OrganizationWithNavigationProperties
            {
                Organization = s,
                Industry = ApplyDataFilters<IMongoQueryable<Industry>, Industry>(dbContext.Collection<Industry>().AsQueryable()).FirstOrDefault(e => e.Id == s.IndustryId),

            }).ToList();
        }

        public virtual async Task<List<Organization>> GetListAsync(
            string? filterText = null,
            string? code = null,
            string? name = null,
            OrganizationType? organizationType = null,
            string? mailInfo = null,
            string? phoneInfo = null,
            string? tags = null,
            OrganizationType? organizationTypePreFiilter = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, code, name, organizationType, mailInfo, phoneInfo, tags, null, organizationTypePreFiilter);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? OrganizationConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<Organization>>()
                .PageBy<Organization, IMongoQueryable<Organization>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? code = null,
            string? name = null,
            OrganizationType? organizationType = null,
            string? mailInfo = null,
            string? phoneInfo = null,
            string? tags = null,
            Guid? industryId = null,
            OrganizationType? organizationTypePreFiilter = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, code, name, organizationType, mailInfo, phoneInfo, tags, industryId, organizationTypePreFiilter);
            return await query.As<IMongoQueryable<Organization>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Organization> ApplyFilter(
            IQueryable<Organization> query,
            string? filterText = null,
            string? code = null,
            string? name = null,
            OrganizationType? organizationType = null,
            string? mailInfo = null,
            string? phoneInfo = null,
            string? tags = null,
            Guid? industryId = null,
            OrganizationType? organizationTypePreFiilter = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText),
                    e => (
                        e.Code!.Contains(filterText!) 
                        ||  e.Name!.Contains(filterText!) 
                        ||  e.Tags!.Contains(filterText!) 
                        || e.PhoneInfo.PhoneItems.Any(x => x.Number.Contains(filterText!))
                        || e.MailInfo.MailItems.Any(x => x.Email.Contains(filterText!))
                        ))
                .WhereIf(!string.IsNullOrWhiteSpace(code), e => e.Code.Contains(code))
                .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name))
                .WhereIf(organizationType.HasValue, e => e.OrganizationType == organizationType)
                .WhereIf(!string.IsNullOrWhiteSpace(phoneInfo), e => e.PhoneInfo.PhoneItems.Any(x => x.Number.Contains(phoneInfo)))
                .WhereIf(!string.IsNullOrWhiteSpace(mailInfo), e => e.MailInfo.MailItems.Any(x => x.Email.Contains(mailInfo)))
                .WhereIf(!string.IsNullOrWhiteSpace(tags), e => e.Tags.Contains(tags))
                .WhereIf(industryId != null && industryId != Guid.Empty, e => e.IndustryId == industryId);
        }
    }
}