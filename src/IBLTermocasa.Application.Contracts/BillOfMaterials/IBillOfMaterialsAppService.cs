using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IBLTermocasa.Common;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using IBLTermocasa.Shared;

namespace IBLTermocasa.BillOfMaterials
{
    public interface IBillOfMaterialsAppService : IApplicationService
    {

        Task<PagedResultDto<BillOfMaterialDto>> GetListAsync(GetBillOfMaterialsInput input);

        Task<BillOfMaterialDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<BillOfMaterialDto> CreateAsync(BillOfMaterialCreateDto input);

        Task<BillOfMaterialDto> UpdateAsync(Guid id, BillOfMaterialUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(BillOfMaterialExcelDownloadDto input);

        Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync(); Task DeleteByIdsAsync(List<Guid> billofmaterialIds);

        Task DeleteAllAsync(GetBillOfMaterialsInput input);
        Task<List<ViewElementPropertyDto<object>>> GenerateBillOfMaterial(Guid id);
        Task<List<BomItemDto>> CalculateConsumption(Guid id, List<BomItemDto> listItems);
        Task<List<BomItemDto>> CalculateConsumption(Guid id);
    }
}