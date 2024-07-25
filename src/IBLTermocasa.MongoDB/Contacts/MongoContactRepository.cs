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

namespace IBLTermocasa.Contacts
{
    public class MongoContactRepository : MongoDbRepository<IBLTermocasaMongoDbContext, Contact, Guid>, IContactRepository
    {
        public MongoContactRepository(IMongoDbContextProvider<IBLTermocasaMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public virtual async Task<List<Contact>> GetListAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, title, name, surname, confidentialName, jobRole, mailInfo, phoneInfo, addressInfo, tag);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ContactConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<Contact>>()
                .PageBy<Contact, IMongoQueryable<Contact>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<long> GetCountAsync(
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
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, title, name, surname, confidentialName, jobRole, mailInfo, phoneInfo, addressInfo, tag);
            return await query.As<IMongoQueryable<Contact>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Contact> ApplyFilter(
        IQueryable<Contact> query,
        string? filterText = null,
        string? title = null,
        string? name = null,
        string? surname = null,
        string? confidentialName = null,
        string? jobRole = null,
        string? mailInfo = null,
        string? phoneInfo = null,
        string? addressInfo = null,
        string? tag = null)
        {
            filterText = filterText?.ToLower();
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => 
                    e.Title != null && e.Title.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase)
                    || e.Name.ToLower().Contains(filterText!, StringComparison.CurrentCultureIgnoreCase)
                    || e.Surname.ToLower().Contains(filterText!, StringComparison.CurrentCultureIgnoreCase)
                    || e.ConfidentialName != null && e.ConfidentialName.ToLower().Contains(filterText!, StringComparison.CurrentCultureIgnoreCase)
                    || e.JobRole != null && e.JobRole.ToLower().Contains(filterText!, StringComparison.CurrentCultureIgnoreCase)
                    || e.MailInfo.MailItems.Any(x => x.Email.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase)) 
                    || e.PhoneInfo.PhoneItems.Any(x => x.Number.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase)) 
                    || e.Tags.Any(t => t.Contains(filterText!, StringComparison.CurrentCultureIgnoreCase)))
                .WhereIf(!string.IsNullOrWhiteSpace(title), e => e.Title != null && e.Title.Contains(title!, StringComparison.CurrentCultureIgnoreCase))
                .WhereIf(!string.IsNullOrWhiteSpace(name), e => e.Name.Contains(name!, StringComparison.CurrentCultureIgnoreCase))
                .WhereIf(!string.IsNullOrWhiteSpace(surname), e => e.Surname.Contains(surname!, StringComparison.CurrentCultureIgnoreCase))
                .WhereIf(!string.IsNullOrWhiteSpace(confidentialName), e => e.ConfidentialName != null && e.ConfidentialName.Contains(confidentialName!, StringComparison.CurrentCultureIgnoreCase))
                .WhereIf(!string.IsNullOrWhiteSpace(jobRole), e => e.JobRole != null && e.JobRole.Contains(jobRole!, StringComparison.CurrentCultureIgnoreCase))
                .WhereIf(!string.IsNullOrWhiteSpace(mailInfo), e => e.MailInfo.MailItems.Any(x => x.Email.Contains(mailInfo!, StringComparison.CurrentCultureIgnoreCase)))
                .WhereIf(!string.IsNullOrWhiteSpace(phoneInfo), e => e.PhoneInfo.PhoneItems.Any(x => x.Number.Contains(phoneInfo!, StringComparison.CurrentCultureIgnoreCase)))
                .WhereIf(!string.IsNullOrWhiteSpace(tag), e => e.Tags.Any(t => t.Contains(tag!, StringComparison.CurrentCultureIgnoreCase)));
        }
    }
}