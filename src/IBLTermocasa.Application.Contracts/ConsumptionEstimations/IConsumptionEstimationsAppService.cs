using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using IBLTermocasa.Shared;

namespace IBLTermocasa.ConsumptionEstimations
{
    public interface IConsumptionEstimationsAppService : IApplicationService
    {

        Task<PagedResultDto<ConsumptionEstimationDto>> GetListAsync(GetConsumptionEstimationsInput input);

        Task<ConsumptionEstimationDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<ConsumptionEstimationDto> CreateAsync(ConsumptionEstimationCreateDto input);

        Task<ConsumptionEstimationDto> UpdateAsync(Guid id, ConsumptionEstimationUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(ConsumptionEstimationExcelDownloadDto input);

        Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync(); Task DeleteByIdsAsync(List<Guid> consumptionestimationIds);

        Task DeleteAllAsync(GetConsumptionEstimationsInput input);
        Task<ConsumptionEstimationDto> GetAsyncByProduct(Guid idProduct);
    }
}