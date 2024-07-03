using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;
using IBLTermocasa.MongoDB;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;
using MongoDB.Driver.Linq;
using MongoDB.Driver;

namespace IBLTermocasa.ProfessionalProfiles
{
    public class MongoProfessionalProfileRepository : MongoDbRepository<IBLTermocasaMongoDbContext, ProfessionalProfile, Guid>, IProfessionalProfileRepository
    {
        public MongoProfessionalProfileRepository(IMongoDbContextProvider<IBLTermocasaMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public virtual async Task DeleteAllAsync(
            string? filterText = null,
                        string? name = null,
            double? standardPriceMin = null,
            double? standardPriceMax = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, name, standardPriceMin, standardPriceMax);

            var ids = query.Select(x => x.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<ProfessionalProfile>> GetListAsync(
            string? filterText = null,
            string? name = null,
            double? standardPrice = null,
            double? standardPriceMin = null,
            double? standardPriceMax = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, name, standardPrice, standardPriceMin, standardPriceMax);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? ProfessionalProfileConsts.GetDefaultSorting(false) : sorting);
            return await query.OrderBy(string.IsNullOrWhiteSpace(sorting)
                    ? ProfessionalProfileConsts.GetDefaultSorting(false)
                    : sorting.Split('.').Last())
                .As<IMongoQueryable<ProfessionalProfile>>()
                .PageBy<ProfessionalProfile, IMongoQueryable<ProfessionalProfile>>(skipCount, maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null,
            string? name = null,
            double? standardPrice = null,
            double? standardPriceMin = null,
            double? standardPriceMax = null,
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter((await GetMongoQueryableAsync(cancellationToken)), filterText, name, standardPrice, standardPriceMin, standardPriceMax);
            return await query.As<IMongoQueryable<ProfessionalProfile>>()
                .LongCountAsync(GetCancellationToken(cancellationToken));
        }

        protected virtual IQueryable<ProfessionalProfile> ApplyFilter(
            IQueryable<ProfessionalProfile> query,
            string? filterText = null,
            string? name = null,
            double? standardPrice = null,
            double? standardPriceMin = null,
            double? standardPriceMax = null)
        {
            //Ritornerà la query con i filtri applicati in base ai parametri passati in input che non sia case sensitive
            return query
                .WhereIf(!string.IsNullOrWhiteSpace(filterText), e =>
                    (e.Name != null && e.Name.Contains(filterText)))
                .WhereIf(!string.IsNullOrWhiteSpace(name),
                    e => e.Name != null && e.Name.Contains(name));
        }
    }
}