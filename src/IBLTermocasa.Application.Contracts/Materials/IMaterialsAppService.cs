using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using IBLTermocasa.Shared;

namespace IBLTermocasa.Materials
{
    public interface IMaterialsAppService : IApplicationService
    {

        Task<PagedResultDto<MaterialDto>> GetListAsync(GetMaterialsInput input);

        Task<MaterialDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<MaterialDto> CreateAsync(MaterialCreateDto input);

        Task<MaterialDto> UpdateAsync(Guid id, MaterialUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(MaterialExcelDownloadDto input);

        Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}