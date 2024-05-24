using IBLTermocasa.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using IBLTermocasa.Shared;

namespace IBLTermocasa.Interactions
{
    public interface IInteractionsAppService : IApplicationService
    {

        Task<PagedResultDto<InteractionWithNavigationPropertiesDto>> GetListAsync(GetInteractionsInput input);

        Task<InteractionWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<InteractionDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetIdentityUserLookupAsync(LookupRequestDto input);

        Task<PagedResultDto<LookupDto<Guid>>> GetOrganizationUnitLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<InteractionDto> CreateAsync(InteractionCreateDto input);

        Task<InteractionDto> UpdateAsync(Guid id, InteractionUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(InteractionExcelDownloadDto input);

        Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}