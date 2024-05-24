using IBLTermocasa.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using IBLTermocasa.Shared;

namespace IBLTermocasa.Products
{
    public partial interface IProductsAppService : IApplicationService
    {

        Task<PagedResultDto<ProductWithNavigationPropertiesDto>> GetListAsync(GetProductsInput input);

        Task<ProductWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<ProductDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetComponentLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<ProductDto> CreateAsync(ProductCreateDto input);

        Task<ProductDto> UpdateAsync(Guid id, ProductUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(ProductExcelDownloadDto input);

        Task<DownloadTokenResultDto> GetDownloadTokenAsync(); Task DeleteByIdsAsync(List<Guid> productIds);

        Task DeleteAllAsync(GetProductsInput input);
    }
}