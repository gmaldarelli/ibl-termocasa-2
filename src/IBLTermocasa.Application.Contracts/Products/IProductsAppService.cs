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
    public interface IProductsAppService : IApplicationService
    {

        Task<PagedResultDto<ProductDto>> GetListAsync(GetProductsInput input, bool includeDetails = false);

        Task<ProductDto> GetAsync(Guid id, bool includeDetails = false);

        Task<PagedResultDto<LookupDto<Guid>>> GetComponentLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetQuestionTemplateLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<ProductDto> CreateAsync(ProductCreateDto input);

        Task<ProductDto> UpdateAsync(Guid id, ProductUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(ProductExcelDownloadDto input);

        Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync(); Task DeleteByIdsAsync(List<Guid> productIds);

        Task DeleteAllAsync(GetProductsInput input);
    }
}