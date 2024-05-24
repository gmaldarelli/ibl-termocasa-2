using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Data;

namespace IBLTermocasa.Industries
{
    public class IndustryManager : DomainService
    {
        protected IIndustryRepository _industryRepository;

        public IndustryManager(IIndustryRepository industryRepository)
        {
            _industryRepository = industryRepository;
        }

        public virtual async Task<Industry> CreateAsync(
        string code, string? description = null)
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            Check.Length(code, nameof(code), IndustryConsts.CodeMaxLength);
            Check.Length(description, nameof(description), IndustryConsts.DescriptionMaxLength);

            var industry = new Industry(
             GuidGenerator.Create(),
             code, description
             );

            return await _industryRepository.InsertAsync(industry);
        }

        public virtual async Task<Industry> UpdateAsync(
            Guid id,
            string code, string? description = null, [CanBeNull] string? concurrencyStamp = null
        )
        {
            Check.NotNullOrWhiteSpace(code, nameof(code));
            Check.Length(code, nameof(code), IndustryConsts.CodeMaxLength);
            Check.Length(description, nameof(description), IndustryConsts.DescriptionMaxLength);

            var industry = await _industryRepository.GetAsync(id);

            industry.Code = code;
            industry.Description = description;

            industry.SetConcurrencyStampIfNotNull(concurrencyStamp);
            return await _industryRepository.UpdateAsync(industry);
        }

    }
}