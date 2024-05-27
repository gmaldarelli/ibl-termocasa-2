using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    }
}