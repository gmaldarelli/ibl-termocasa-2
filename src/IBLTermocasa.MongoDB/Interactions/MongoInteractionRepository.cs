using IBLTermocasa.Types;
using Volo.Abp.Identity;
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

namespace IBLTermocasa.Interactions
{
    public class MongoInteractionRepository : MongoDbRepository<IBLTermocasaMongoDbContext, Interaction, Guid>, IInteractionRepository
    {
        public MongoInteractionRepository(IMongoDbContextProvider<IBLTermocasaMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public virtual async Task<InteractionWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var interaction = await (await GetMongoQueryableAsync(cancellationToken))
                .FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));

            var identityUser = await ((await GetDbContextAsync(cancellationToken)).Database.GetCollection<IdentityUser>(AbpIdentityDbProperties.DbTablePrefix + "Users").AsQueryable()).FirstOrDefaultAsync(e => e.Id == interaction.WriterUserId, cancellationToken: cancellationToken);
            var organizationUnit = await ((await GetDbContextAsync(cancellationToken)).Database.GetCollection<OrganizationUnit>(AbpIdentityDbProperties.DbTablePrefix + "OrganizationUnits").AsQueryable()).FirstOrDefaultAsync(e => e.Id == interaction.NotificationOrganizationUnitId, cancellationToken: cancellationToken);
            var identityUser1 = await ((await GetDbContextAsync(cancellationToken)).Database.GetCollection<IdentityUser>(AbpIdentityDbProperties.DbTablePrefix + "Users").AsQueryable()).FirstOrDefaultAsync(e => e.Id == interaction.IdentityUserId, cancellationToken: cancellationToken);

            return new InteractionWithNavigationProperties
            {
                Interaction = interaction,
                IdentityUser = identityUser,
                OrganizationUnit = organizationUnit,
                IdentityUser1 = identityUser1,

            };
        }

        public virtual async Task<List<InteractionWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            InteractionType? interactionType = null,
            DateTime? interactionDateMin = null,
            DateTime? interactionDateMax = null,
            string? content = null,
            string? referenceObject = null,
            string? writerNotes = null,
            Guid? writerUserId = null,
            Guid? identityUserId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, interactionType, interactionDateMin, interactionDateMax, content, referenceObject, writerNotes, writerUserId, identityUserId);
            var interactions = await query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? InteractionConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                .As<IMongoQueryable<Interaction>>()
                .PageBy<Interaction, IMongoQueryable<Interaction>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            var dbContext = await GetDbContextAsync(cancellationToken);
            return interactions.Select(s => new InteractionWithNavigationProperties
            {
                Interaction = s,
                IdentityUser = ApplyDataFilters<IMongoQueryable<IdentityUser>, IdentityUser>(dbContext.Database.GetCollection<IdentityUser>(AbpIdentityDbProperties.DbTablePrefix + "Users").AsQueryable()).FirstOrDefault(e => e.Id == s.WriterUserId),
                OrganizationUnit = ApplyDataFilters<IMongoQueryable<OrganizationUnit>, OrganizationUnit>(dbContext.Database.GetCollection<OrganizationUnit>(AbpIdentityDbProperties.DbTablePrefix + "OrganizationUnits").AsQueryable()).FirstOrDefault(e => e.Id == s.NotificationOrganizationUnitId),
                IdentityUser1 = ApplyDataFilters<IMongoQueryable<IdentityUser>, IdentityUser>(dbContext.Database.GetCollection<IdentityUser>(AbpIdentityDbProperties.DbTablePrefix + "Users").AsQueryable()).FirstOrDefault(e => e.Id == s.IdentityUserId),

            }).ToList();
        }

        public virtual async Task<List<Interaction>> GetListAsync(
            string? filterText = null,
            InteractionType? interactionType = null,
            DateTime? interactionDateMin = null,
            DateTime? interactionDateMax = null,
            string? content = null,
            string? referenceObject = null,
            string? writerNotes = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, interactionType, interactionDateMin, interactionDateMax, content, referenceObject, writerNotes);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? InteractionConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<Interaction>>()
                .PageBy<Interaction, IMongoQueryable<Interaction>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            InteractionType? interactionType = null,
            DateTime? interactionDateMin = null,
            DateTime? interactionDateMax = null,
            string? content = null,
            string? referenceObject = null,
            string? writerNotes = null,
            Guid? writerUserId = null,
            Guid? identityUserId = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, interactionType, interactionDateMin, interactionDateMax, content, referenceObject, writerNotes, writerUserId, identityUserId);
            return await query.As<IMongoQueryable<Interaction>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<Interaction> ApplyFilter(
            IQueryable<Interaction> query,
            string? filterText = null,
            InteractionType? interactionType = null,
            DateTime? interactionDateMin = null,
            DateTime? interactionDateMax = null,
            string? content = null,
            string? referenceObject = null,
            string? writerNotes = null,
            Guid? writerUserId = null,
            Guid? identityUserId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Content!.Contains(filterText!) || e.ReferenceObject!.Contains(filterText!) || e.WriterNotes!.Contains(filterText!))
                    .WhereIf(interactionType.HasValue, e => e.InteractionType == interactionType)
                    .WhereIf(interactionDateMin.HasValue, e => e.InteractionDate >= interactionDateMin!.Value)
                    .WhereIf(interactionDateMax.HasValue, e => e.InteractionDate <= interactionDateMax!.Value)
                    .WhereIf(!string.IsNullOrWhiteSpace(content), e => e.Content.Contains(content))
                    .WhereIf(!string.IsNullOrWhiteSpace(referenceObject), e => e.ReferenceObject.Contains(referenceObject))
                    .WhereIf(!string.IsNullOrWhiteSpace(writerNotes), e => e.WriterNotes.Contains(writerNotes))
                    .WhereIf(writerUserId != null && writerUserId != Guid.Empty, e => e.WriterUserId == writerUserId)
                    .WhereIf(identityUserId != null && identityUserId != Guid.Empty, e => e.IdentityUserId == identityUserId);
        }
    }
}