using Asp.Versioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using IBLTermocasa.QuestionTemplates;
using Volo.Abp.Content;
using IBLTermocasa.Shared;

namespace IBLTermocasa.Controllers.QuestionTemplates
{
    [RemoteService]
    [Area("app")]
    [ControllerName("QuestionTemplate")]
    [Route("api/app/question-templates")]

    public class QuestionTemplateController : AbpController, IQuestionTemplatesAppService
    {
        protected IQuestionTemplatesAppService _questionTemplatesAppService;

        public QuestionTemplateController(IQuestionTemplatesAppService questionTemplatesAppService)
        {
            _questionTemplatesAppService = questionTemplatesAppService;
        }

        [HttpGet]
        public virtual Task<PagedResultDto<QuestionTemplateDto>> GetListAsync(GetQuestionTemplatesInput input)
        {
            return _questionTemplatesAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("{id}")]
        public virtual Task<QuestionTemplateDto> GetAsync(Guid id)
        {
            return _questionTemplatesAppService.GetAsync(id);
        }

        [HttpPost]
        public virtual Task<QuestionTemplateDto> CreateAsync(QuestionTemplateCreateDto input)
        {
            return _questionTemplatesAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<QuestionTemplateDto> UpdateAsync(Guid id, QuestionTemplateUpdateDto input)
        {
            return _questionTemplatesAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _questionTemplatesAppService.DeleteAsync(id);
        }

        [HttpGet]
        [Route("as-excel-file")]
        public virtual Task<IRemoteStreamContent> GetListAsExcelFileAsync(QuestionTemplateExcelDownloadDto input)
        {
            return _questionTemplatesAppService.GetListAsExcelFileAsync(input);
        }

        [HttpGet]
        [Route("download-token")]
        public virtual Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            return _questionTemplatesAppService.GetDownloadTokenAsync();
        }
        
        [HttpGet]
        [Route("get-list-by-guids")]
        public virtual Task<List<QuestionTemplateDto>> GetListByGuidsAsync(List<Guid> questionTemplateIds)
        {
            return _questionTemplatesAppService.GetListByGuidsAsync(questionTemplateIds);
        }
    }
}