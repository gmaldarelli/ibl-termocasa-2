using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using IBLTermocasa.Shared;

namespace IBLTermocasa.Quotations
{
    public interface IQuotationsAppService : IApplicationService
    {

        Task<PagedResultDto<QuotationDto>> GetListAsync(GetQuotationsInput input);

        Task<QuotationDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<QuotationDto> CreateAsync(QuotationCreateDto input);

        Task<QuotationDto> UpdateAsync(Guid id, QuotationUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(QuotationExcelDownloadDto input);

        Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}