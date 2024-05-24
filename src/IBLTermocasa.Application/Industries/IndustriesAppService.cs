using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using IBLTermocasa.Permissions;
using IBLTermocasa.Industries;

namespace IBLTermocasa.Industries
{
    [RemoteService(IsEnabled = false)]
    [Authorize(IBLTermocasaPermissions.Industries.Default)]
    public class IndustriesAppService : IBLTermocasaAppService, IIndustriesAppService
    {

        protected IIndustryRepository _industryRepository;
        protected IndustryManager _industryManager;

        public IndustriesAppService(IIndustryRepository industryRepository, IndustryManager industryManager)
        {

            _industryRepository = industryRepository;
            _industryManager = industryManager;
        }

        public virtual async Task<PagedResultDto<IndustryDto>> GetListAsync(GetIndustriesInput input)
        {
            var totalCount = await _industryRepository.GetCountAsync(input.FilterText, input.Code, input.Description);
            var items = await _industryRepository.GetListAsync(input.FilterText, input.Code, input.Description, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<IndustryDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Industry>, List<IndustryDto>>(items)
            };
        }

        public virtual async Task<IndustryDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Industry, IndustryDto>(await _industryRepository.GetAsync(id));
        }

        [Authorize(IBLTermocasaPermissions.Industries.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _industryRepository.DeleteAsync(id);
        }

        [Authorize(IBLTermocasaPermissions.Industries.Create)]
        public virtual async Task<IndustryDto> CreateAsync(IndustryCreateDto input)
        {

            var industry = await _industryManager.CreateAsync(
            input.Code, input.Description
            );

            return ObjectMapper.Map<Industry, IndustryDto>(industry);
        }

        [Authorize(IBLTermocasaPermissions.Industries.Edit)]
        public virtual async Task<IndustryDto> UpdateAsync(Guid id, IndustryUpdateDto input)
        {

            var industry = await _industryManager.UpdateAsync(
            id,
            input.Code, input.Description, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<Industry, IndustryDto>(industry);
        }
        [Authorize(IBLTermocasaPermissions.Industries.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> industryIds)
        {
            await _industryRepository.DeleteManyAsync(industryIds);
        }

        [Authorize(IBLTermocasaPermissions.Industries.Delete)]
        public virtual async Task DeleteAllAsync(GetIndustriesInput input)
        {
            await _industryRepository.DeleteAllAsync(input.FilterText, input.Code, input.Description);
        }
    }
}