using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IBLTermocasa.Materials;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using IBLTermocasa.Shared;

namespace IBLTermocasa.Components
{
    public interface IComponentsAppService : IApplicationService
    {

        Task<PagedResultDto<ComponentDto>> GetListAsync(GetComponentsInput input);

        Task<ComponentDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<ComponentDto> CreateAsync(ComponentCreateDto input);

        Task<ComponentDto> UpdateAsync(Guid id, ComponentUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(ComponentExcelDownloadDto input);

        Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();
        Task<ComponentDto>  DeleteComponentItemAsync(Guid componentId, Guid componentItemId);
        Task<ComponentDto> UpdateComponentItemAsync(Guid componentId, List<ComponentItemDto> componentItems);
        Task<ComponentDto> CreateComponentItemAsync(Guid componentId, List<ComponentItemDto> componentItems);
        Task<PagedResultDto<LookupDto<Guid>>> GetMaterialLookupAsync(LookupRequestDto lookupRequestDto);
        Task<Dictionary<Guid,List<MaterialDto>>> GetMaterialDictionaryAsync(List<Guid> componentIds);
    }
}

