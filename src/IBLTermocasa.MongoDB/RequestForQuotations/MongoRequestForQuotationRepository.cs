using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using IBLTermocasa.Common;
using IBLTermocasa.Contacts;
using IBLTermocasa.MongoDB;
using IBLTermocasa.Organizations;
using IBLTermocasa.Types;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.Identity;
using Volo.Abp.MongoDB;

namespace IBLTermocasa.RequestForQuotations
{
    public class MongoRequestForQuotationRepository :
        MongoDbRepository<IBLTermocasaMongoDbContext, RequestForQuotation, Guid>, IRequestForQuotationRepository
    {
        public MongoRequestForQuotationRepository(IMongoDbContextProvider<IBLTermocasaMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public virtual async Task<RequestForQuotationWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id,
            CancellationToken cancellationToken = default)
        {
            var requestForQuotation = await (await GetMongoQueryableAsync(cancellationToken))
                .FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));

            var identityUser =
                await ((await GetDbContextAsync(cancellationToken)).Database
                        .GetCollection<IdentityUser>(AbpIdentityDbProperties.DbTablePrefix + "Users").AsQueryable())
                    .FirstOrDefaultAsync(e => e.Id == requestForQuotation.AgentProperty.Id, cancellationToken: cancellationToken);
            var contact =
                await (await GetMongoQueryableAsync<Contact>(cancellationToken)).FirstOrDefaultAsync(
                    e => e.Id == requestForQuotation.ContactProperty.Id, cancellationToken: cancellationToken);
            var organization =
                await (await GetMongoQueryableAsync<Organization>(cancellationToken)).FirstOrDefaultAsync(
                    e => e.Id == requestForQuotation.OrganizationProperty.Id, cancellationToken: cancellationToken);

            return new RequestForQuotationWithNavigationProperties
            {
                RequestForQuotation = requestForQuotation,
                IdentityUser = identityUser,
                Contact = contact,
                Organization = organization,
            };
        }

        public virtual async Task<List<RequestForQuotationWithNavigationProperties>>
            GetListWithNavigationPropertiesAsync(
                string? filterText = null,
                string? quoteNumber = null,
                string? workSite = null,
                string? city = null,
                AgentProperty? agentProperty = null,
                OrganizationProperty? organizationProperty = null,
                ContactProperty? contactProperty = null,
                PhoneInfo? phoneInfo = null,
                MailInfo? mailInfo = null,
                decimal? discountMin = null,
                decimal? discountMax = null,
                string? description = null,
                RfqStatus? status = null,
                string? sorting = null,
                int maxResultCount = int.MaxValue,
                int skipCount = 0,
                CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, quoteNumber,
                workSite, city, agentProperty, organizationProperty, contactProperty, phoneInfo, mailInfo, discountMin, discountMax,
                description, status);
            var requestForQuotations = await query.OrderBy(string.IsNullOrWhiteSpace(sorting)
                    ? RequestForQuotationConsts.GetDefaultSorting(false)
                    : sorting.Split('.').Last())
                .As<IMongoQueryable<RequestForQuotation>>()
                .PageBy<RequestForQuotation, IMongoQueryable<RequestForQuotation>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            var dbContext = await GetDbContextAsync(cancellationToken);
            return requestForQuotations.Select(s => new RequestForQuotationWithNavigationProperties
            {
                RequestForQuotation = s,
                IdentityUser =
                    ApplyDataFilters<IMongoQueryable<IdentityUser>, IdentityUser>(dbContext.Database
                            .GetCollection<IdentityUser>(AbpIdentityDbProperties.DbTablePrefix + "Users").AsQueryable())
                        .FirstOrDefault(e => e.Id == s.AgentProperty.Id),
                Contact =
                    ApplyDataFilters<IMongoQueryable<Contact>, Contact>(dbContext.Collection<Contact>().AsQueryable())
                        .FirstOrDefault(e => e.Id == s.ContactProperty.Id),
                Organization =
                    ApplyDataFilters<IMongoQueryable<Organization>, Organization>(dbContext.Collection<Organization>()
                        .AsQueryable()).FirstOrDefault(e => e.Id == s.OrganizationProperty.Id),
            }).ToList();
        }

        public virtual async Task<List<RequestForQuotation>> GetListAsync(
            string? filterText = null,
            string? quoteNumber = null,
            string? workSite = null,
            string? city = null,
            AgentProperty? agentProperty = null,
            OrganizationProperty? organizationProperty = null,
            ContactProperty? contactProperty = null,
            PhoneInfo? phoneInfo = null,
            MailInfo? mailInfo = null,
            decimal? discountMin = null,
            decimal? discountMax = null,
            string? description = null,
            RfqStatus? status = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, quoteNumber,
                workSite, city, agentProperty, organizationProperty, contactProperty, phoneInfo, mailInfo, discountMin, discountMax,
                description, status);
            return await query.OrderBy(string.IsNullOrWhiteSpace(sorting)
                    ? RequestForQuotationConsts.GetDefaultSorting(false)
                    : sorting.Split('.').Last())
                .As<IMongoQueryable<RequestForQuotation>>()
                .PageBy<RequestForQuotation, IMongoQueryable<RequestForQuotation>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? quoteNumber = null,
            string? workSite = null,
            string? city = null,
            AgentProperty? agentProperty = null,
            OrganizationProperty? organizationProperty = null,
            ContactProperty? contactProperty = null,
            PhoneInfo? phoneInfo = null,
            MailInfo? mailInfo = null,
            decimal? discountMin = null,
            decimal? discountMax = null,
            string? description = null,
            RfqStatus? status = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, quoteNumber,
                workSite, city, agentProperty, organizationProperty, contactProperty, phoneInfo, mailInfo, discountMin, discountMax,
                description, status);
            return await query.As<IMongoQueryable<RequestForQuotation>>()
                .LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<RequestForQuotation> ApplyFilter(
            IQueryable<RequestForQuotation> query,
            string? filterText = null,
            string? quoteNumber = null,
            string? workSite = null,
            string? city = null,
            AgentProperty? agentProperty = null,
            OrganizationProperty? organizationProperty = null,
            ContactProperty? contactProperty = null,
            PhoneInfo? phoneInfo = null,
            MailInfo? mailInfo = null,
            decimal? discountMin = null,
            decimal? discountMax = null,
            string? description = null,
            RfqStatus? status = null)
        {
            filterText = filterText?.ToLower();
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e =>
                    (e.QuoteNumber != null && e.QuoteNumber.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase)) ||
                    (e.WorkSite != null && e.WorkSite.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase)) ||
                    (e.City != null && e.City.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase)) ||
                    (e.AgentProperty != null && e.AgentProperty.Name.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase)) ||
                    (e.OrganizationProperty != null && e.OrganizationProperty.Name.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase)) ||
                    (e.ContactProperty != null && e.ContactProperty.Name.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase)) ||
                    (e.PhoneInfo != null && e.PhoneInfo.PhoneItems.Any(item => item.Number.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase))) ||
                    (e.MailInfo != null && e.MailInfo.MailItems.Any(item => item.Email.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase))) ||
                    (e.Description != null && e.Description.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase)))
                .WhereIf(!string.IsNullOrWhiteSpace(quoteNumber),
                    e => e.QuoteNumber.Contains(quoteNumber!, StringComparison.CurrentCultureIgnoreCase))
                .WhereIf(!string.IsNullOrWhiteSpace(workSite), e => e.WorkSite != null && e.WorkSite.Contains(workSite!, StringComparison.CurrentCultureIgnoreCase))
                .WhereIf(!string.IsNullOrWhiteSpace(city), e => e.City != null && e.City.Contains(city!, StringComparison.CurrentCultureIgnoreCase))
                .WhereIf(agentProperty != null && !agentProperty.Name.IsNullOrWhiteSpace(),
                    e => e.AgentProperty != null && e.AgentProperty.Name != null && e.AgentProperty.Name.Contains(agentProperty!.Name!, StringComparison.CurrentCultureIgnoreCase))
                .WhereIf(organizationProperty != null && !organizationProperty.Name.IsNullOrWhiteSpace(),
                    e => e.OrganizationProperty != null && e.OrganizationProperty.Name != null && e.OrganizationProperty.Name.Contains(organizationProperty!.Name!, StringComparison.CurrentCultureIgnoreCase))
                .WhereIf(contactProperty != null && !contactProperty.Name.IsNullOrWhiteSpace(),
                    e => e.ContactProperty != null && e.ContactProperty.Name != null && e.ContactProperty.Name.Contains(contactProperty!.Name!, StringComparison.CurrentCultureIgnoreCase))
                .WhereIf(phoneInfo != null && !phoneInfo.PhoneItems.First().Number.IsNullOrWhiteSpace(),
                    e => e.PhoneInfo != null && e.PhoneInfo.PhoneItems.Any(item =>
                        item.Number.Contains(phoneInfo!.PhoneItems.First().Number, StringComparison.CurrentCultureIgnoreCase)))
                .WhereIf(mailInfo != null,
                    e => e.MailInfo != null &&
                         e.MailInfo.MailItems.Any(item => item.Email.Contains(mailInfo.MailItems.First().Email)))
                .WhereIf(discountMin.HasValue, e => e.Discount >= discountMin.Value)
                .WhereIf(discountMax.HasValue, e => e.Discount <= discountMax.Value)
                .WhereIf(!string.IsNullOrWhiteSpace(description),
                    e => e.Description != null && e.Description.Contains(description!, StringComparison.CurrentCultureIgnoreCase))
                .WhereIf(status.HasValue, e => e.Status == status);
        }
    }
}