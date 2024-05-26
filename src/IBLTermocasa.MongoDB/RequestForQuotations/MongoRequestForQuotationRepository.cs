using IBLTermocasa.RequestForQuotations;
using IBLTermocasa.Organizations;
using IBLTermocasa.Contacts;
using Volo.Abp.Identity;
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

namespace IBLTermocasa.RequestForQuotations
{
    public class MongoRequestForQuotationRepository : MongoDbRepository<IBLTermocasaMongoDbContext, RequestForQuotation, Guid>, IRequestForQuotationRepository
    {
        public MongoRequestForQuotationRepository(IMongoDbContextProvider<IBLTermocasaMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public virtual async Task<RequestForQuotationWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var requestForQuotation = await (await GetMongoQueryableAsync(cancellationToken))
                .FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));

            var identityUser = await ((await GetDbContextAsync(cancellationToken)).Database.GetCollection<IdentityUser>(AbpIdentityDbProperties.DbTablePrefix + "Users").AsQueryable()).FirstOrDefaultAsync(e => e.Id == requestForQuotation.AgentId, cancellationToken: cancellationToken);
            var contact = await (await GetMongoQueryableAsync<Contact>(cancellationToken)).FirstOrDefaultAsync(e => e.Id == requestForQuotation.ContactId, cancellationToken: cancellationToken);
            var organization = await (await GetMongoQueryableAsync<Organization>(cancellationToken)).FirstOrDefaultAsync(e => e.Id == requestForQuotation.OrganizationId, cancellationToken: cancellationToken);

            return new RequestForQuotationWithNavigationProperties
            {
                RequestForQuotation = requestForQuotation,
                IdentityUser = identityUser,
                Contact = contact,
                Organization = organization,

            };
        }

        public virtual async Task<List<RequestForQuotationWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, quoteNumber, workSite, city, organizationProperty, contactProperty, phoneInfo, mailInfo, discountMin, discountMax, description, status, agentId, contactId, organizationId);
            var requestForQuotations = await query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? RequestForQuotationConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                .As<IMongoQueryable<RequestForQuotation>>()
                .PageBy<RequestForQuotation, IMongoQueryable<RequestForQuotation>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            var dbContext = await GetDbContextAsync(cancellationToken);
            return requestForQuotations.Select(s => new RequestForQuotationWithNavigationProperties
            {
                RequestForQuotation = s,
                IdentityUser = ApplyDataFilters<IMongoQueryable<IdentityUser>, IdentityUser>(dbContext.Database.GetCollection<IdentityUser>(AbpIdentityDbProperties.DbTablePrefix + "Users").AsQueryable()).FirstOrDefault(e => e.Id == s.AgentId),
                Contact = ApplyDataFilters<IMongoQueryable<Contact>, Contact>(dbContext.Collection<Contact>().AsQueryable()).FirstOrDefault(e => e.Id == s.ContactId),
                Organization = ApplyDataFilters<IMongoQueryable<Organization>, Organization>(dbContext.Collection<Organization>().AsQueryable()).FirstOrDefault(e => e.Id == s.OrganizationId),

            }).ToList();
        }

        public virtual async Task<List<RequestForQuotation>> GetListAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, quoteNumber, workSite, city, organizationProperty, contactProperty, phoneInfo, mailInfo, discountMin, discountMax, description, status);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? RequestForQuotationConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<RequestForQuotation>>()
                .PageBy<RequestForQuotation, IMongoQueryable<RequestForQuotation>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<long> GetCountAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, quoteNumber, workSite, city, organizationProperty, contactProperty, phoneInfo, mailInfo, discountMin, discountMax, description, status, agentId, contactId, organizationId);
            return await query.As<IMongoQueryable<RequestForQuotation>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<RequestForQuotation> ApplyFilter(
            IQueryable<RequestForQuotation> query,
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
            Guid? organizationId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.QuoteNumber!.Contains(filterText!) || e.WorkSite!.Contains(filterText!) || e.City!.Contains(filterText!) || e.OrganizationProperty!.Contains(filterText!) || e.ContactProperty!.Contains(filterText!) || e.PhoneInfo!.Contains(filterText!) || e.MailInfo!.Contains(filterText!) || e.Description!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(quoteNumber), e => e.QuoteNumber.Contains(quoteNumber))
                    .WhereIf(!string.IsNullOrWhiteSpace(workSite), e => e.WorkSite.Contains(workSite))
                    .WhereIf(!string.IsNullOrWhiteSpace(city), e => e.City.Contains(city))
                    .WhereIf(!string.IsNullOrWhiteSpace(organizationProperty), e => e.OrganizationProperty.Contains(organizationProperty))
                    .WhereIf(!string.IsNullOrWhiteSpace(contactProperty), e => e.ContactProperty.Contains(contactProperty))
                    .WhereIf(!string.IsNullOrWhiteSpace(phoneInfo), e => e.PhoneInfo.Contains(phoneInfo))
                    .WhereIf(!string.IsNullOrWhiteSpace(mailInfo), e => e.MailInfo.Contains(mailInfo))
                    .WhereIf(discountMin.HasValue, e => e.Discount >= discountMin!.Value)
                    .WhereIf(discountMax.HasValue, e => e.Discount <= discountMax!.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description.Contains(description))
                    .WhereIf(status.HasValue, e => e.Status == status)
                    .WhereIf(agentId != null && agentId != Guid.Empty, e => e.AgentId == agentId)
                    .WhereIf(contactId != null && contactId != Guid.Empty, e => e.ContactId == contactId)
                    .WhereIf(organizationId != null && organizationId != Guid.Empty, e => e.OrganizationId == organizationId);
        }
    }
}