using IBLTermocasa.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace IBLTermocasa.Subproducts
{
    public interface ISubproductsAppService : IApplicationService
    {
        Task<PagedResultDto<SubproductDto>> GetListByProductIdAsync(GetSubproductListInput input);
        Task<PagedResultDto<SubproductWithNavigationPropertiesDto>> GetListWithNavigationPropertiesByProductIdAsync(GetSubproductListInput input);

        Task<PagedResultDto<SubproductWithNavigationPropertiesDto>> GetListAsync(GetSubproductsInput input);

        Task<SubproductWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<SubproductDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetProductLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<SubproductDto> CreateAsync(SubproductCreateDto input);

        Task<SubproductDto> UpdateAsync(Guid id, SubproductUpdateDto input);
    }
}