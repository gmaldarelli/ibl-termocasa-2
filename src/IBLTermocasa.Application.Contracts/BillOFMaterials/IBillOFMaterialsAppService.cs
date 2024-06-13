using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using IBLTermocasa.Shared;

namespace IBLTermocasa.BillOFMaterials
{
    public interface IBillOFMaterialsAppService : IApplicationService
    {

        Task<PagedResultDto<BillOFMaterialDto>> GetListAsync(GetBillOFMaterialsInput input);

        Task<BillOFMaterialDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<BillOFMaterialDto> CreateAsync(BillOFMaterialCreateDto input);

        Task<BillOFMaterialDto> UpdateAsync(Guid id, BillOFMaterialUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(BillOFMaterialExcelDownloadDto input);

        Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync(); Task DeleteByIdsAsync(List<Guid> billofmaterialIds);

        Task DeleteAllAsync(GetBillOFMaterialsInput input);
    }
}