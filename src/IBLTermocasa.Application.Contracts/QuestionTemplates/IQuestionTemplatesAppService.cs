using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;
using IBLTermocasa.Shared;

namespace IBLTermocasa.QuestionTemplates
{
    public interface IQuestionTemplatesAppService : IApplicationService
    {

        Task<PagedResultDto<QuestionTemplateDto>> GetListAsync(GetQuestionTemplatesInput input);

        Task<QuestionTemplateDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<QuestionTemplateDto> CreateAsync(QuestionTemplateCreateDto input);

        Task<QuestionTemplateDto> UpdateAsync(Guid id, QuestionTemplateUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(QuestionTemplateExcelDownloadDto input);

        Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();
        List<QuestionTemplateDto> GetListByGuidsAsync(List<Guid> questionTemplateIds);
    }
}