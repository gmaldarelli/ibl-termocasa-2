using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace IBLTermocasa.Industries
{
    public interface IIndustriesAppService : IApplicationService
    {

        Task<PagedResultDto<IndustryDto>> GetListAsync(GetIndustriesInput input);

        Task<IndustryDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<IndustryDto> CreateAsync(IndustryCreateDto input);

        Task<IndustryDto> UpdateAsync(Guid id, IndustryUpdateDto input); Task DeleteByIdsAsync(List<Guid> industryIds);

        Task DeleteAllAsync(GetIndustriesInput input);
    }
}