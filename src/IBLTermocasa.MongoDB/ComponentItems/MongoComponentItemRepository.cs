using IBLTermocasa.Materials;
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

namespace IBLTermocasa.ComponentItems
{
    public abstract class MongoComponentItemRepositoryBase : MongoDbRepository<IBLTermocasaMongoDbContext, ComponentItem, Guid>
    {
        public MongoComponentItemRepositoryBase(IMongoDbContextProvider<IBLTermocasaMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public virtual async Task<List<ComponentItem>> GetListByComponentIdAsync(
                   Guid componentId,
                   string? sorting = null,
                   int maxResultCount = int.MaxValue,
                   int skipCount = 0,
                   CancellationToken cancellationToken = default)
        {
            IQueryable<ComponentItem> query = (await GetMongoQueryableAsync(cancellationToken)).Where(x => x.ComponentId == componentId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ComponentItemConsts.GetDefaultSorting(false) : sorting);

            return await query
                .As<IMongoQueryable<ComponentItem>>()
                .PageBy<ComponentItem, IMongoQueryable<ComponentItem>>(skipCount, maxResultCount)
                .ToListAsync(cancellationToken);
        }

        public virtual async Task<long> GetCountByComponentIdAsync(Guid componentId, CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync(cancellationToken)).Where(x => x.ComponentId == componentId).CountAsync(cancellationToken);
        }

        public virtual async Task<List<ComponentItemWithNavigationProperties>> GetListWithNavigationPropertiesByComponentIdAsync(
    Guid componentId,
    string? sorting = null,
    int maxResultCount = int.MaxValue,
    int skipCount = 0,
    CancellationToken cancellationToken = default)
        {
            var query = (await GetMongoQueryableAsync(cancellationToken)).Where(x => x.ComponentId == componentId);
            var componentItems = await query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ComponentItemConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                .As<IMongoQueryable<ComponentItem>>()
                .PageBy<ComponentItem, IMongoQueryable<ComponentItem>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            var dbContext = await GetDbContextAsync(cancellationToken);
            return componentItems.Select(s => new ComponentItemWithNavigationProperties
            {
                ComponentItem = s,
                Material = ApplyDataFilters<IMongoQueryable<Material>, Material>(dbContext.Collection<Material>().AsQueryable()).FirstOrDefault(e => e.Id == s.MaterialId),

            }).ToList();
        }

        public virtual async Task<ComponentItemWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var componentItem = await (await GetMongoQueryableAsync(cancellationToken))
                .FirstOrDefaultAsync(e => e.Id == id, GetCancellationToken(cancellationToken));

            var material = await (await GetMongoQueryableAsync<Material>(cancellationToken)).FirstOrDefaultAsync(e => e.Id == componentItem.MaterialId, cancellationToken: cancellationToken);

            return new ComponentItemWithNavigationProperties
            {
                ComponentItem = componentItem,
                Material = material,

            };
        }

        public virtual async Task<List<ComponentItemWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            bool? isDefault = null,
            Guid? materialId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, isDefault, materialId);
            var componentItems = await query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ComponentItemConsts.GetDefaultSorting(false) : sorting.Split('.').Last())
                .As<IMongoQueryable<ComponentItem>>()
                .PageBy<ComponentItem, IMongoQueryable<ComponentItem>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            var dbContext = await GetDbContextAsync(cancellationToken);
            return componentItems.Select(s => new ComponentItemWithNavigationProperties
            {
                ComponentItem = s,
                Material = ApplyDataFilters<IMongoQueryable<Material>, Material>(dbContext.Collection<Material>().AsQueryable()).FirstOrDefault(e => e.Id == s.MaterialId),

            }).ToList();
        }

        public virtual async Task<List<ComponentItem>> GetListAsync(
            string? filterText = null,
            bool? isDefault = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, isDefault);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ComponentItemConsts.GetDefaultSorting(false) : sorting);
            return await query.As<IMongoQueryable<ComponentItem>>()
                .PageBy<ComponentItem, IMongoQueryable<ComponentItem>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            bool? isDefault = null,
            Guid? materialId = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, isDefault, materialId);
            return await query.As<IMongoQueryable<ComponentItem>>().LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<ComponentItem> ApplyFilter(
            IQueryable<ComponentItem> query,
            string? filterText = null,
            bool? isDefault = null,
            Guid? materialId = null)
        {
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => true)
                    .WhereIf(isDefault.HasValue, e => e.IsDefault == isDefault)
                    .WhereIf(materialId != null && materialId != Guid.Empty, e => e.MaterialId == materialId);
        }
    }
}