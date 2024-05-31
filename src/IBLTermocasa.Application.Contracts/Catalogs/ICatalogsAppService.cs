using IBLTermocasa.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using IBLTermocasa.Shared;

namespace IBLTermocasa.Catalogs
{
    public interface ICatalogsAppService : IApplicationService
    {

        Task<PagedResultDto<CatalogWithNavigationPropertiesDto>> GetListAsync(GetCatalogsInput input);

        Task<CatalogWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<CatalogDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetProductLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<CatalogDto> CreateAsync(CatalogCreateDto input);

        Task<CatalogDto> UpdateAsync(Guid id, CatalogUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(CatalogExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync();
        Task<PagedResultDto<CatalogWithNavigationPropertiesDto>> GetListCatalogWithProducts(GetCatalogsInput getCatalogsInput);
    }
}