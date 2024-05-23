using IBLTermocasa.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace IBLTermocasa.ComponentItems
{
    public partial interface IComponentItemsAppService : IApplicationService
    {
        Task<PagedResultDto<ComponentItemDto>> GetListByComponentIdAsync(GetComponentItemListInput input);
        Task<PagedResultDto<ComponentItemWithNavigationPropertiesDto>> GetListWithNavigationPropertiesByComponentIdAsync(GetComponentItemListInput input);

        Task<PagedResultDto<ComponentItemWithNavigationPropertiesDto>> GetListAsync(GetComponentItemsInput input);

        Task<ComponentItemWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<ComponentItemDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetMaterialLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<ComponentItemDto> CreateAsync(ComponentItemCreateDto input);

        Task<ComponentItemDto> UpdateAsync(Guid id, ComponentItemUpdateDto input);
    }
}