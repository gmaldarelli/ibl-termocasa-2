using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using IBLTermocasa.Permissions;
using IBLTermocasa.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using MiniExcelLibs;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Content;

namespace IBLTermocasa.QuestionTemplates
{
    [RemoteService(IsEnabled = false)]
    [Authorize(IBLTermocasaPermissions.QuestionTemplates.Default)]
    public class QuestionTemplatesAppService : IBLTermocasaAppService, IQuestionTemplatesAppService
    {
        protected IDistributedCache<QuestionTemplateExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        protected IQuestionTemplateRepository _questionTemplateRepository;
        protected QuestionTemplateManager _questionTemplateManager;

        public QuestionTemplatesAppService(IQuestionTemplateRepository questionTemplateRepository, QuestionTemplateManager questionTemplateManager, IDistributedCache<QuestionTemplateExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _questionTemplateRepository = questionTemplateRepository;
            _questionTemplateManager = questionTemplateManager;
        }

        public virtual async Task<PagedResultDto<QuestionTemplateDto>> GetListAsync(GetQuestionTemplatesInput input)
        {
            var totalCount = await _questionTemplateRepository.GetCountAsync(input.FilterText, input.Code, input.QuestionText, input.AnswerType, input.ChoiceValue);
            var items = await _questionTemplateRepository.GetListAsync(input.FilterText, input.Code, input.QuestionText, input.AnswerType, input.ChoiceValue, input.Sorting, input.MaxResultCount, input.SkipCount);
            _questionTemplateRepository.GetListAsync(x => x.Code == input.Code);
            return new PagedResultDto<QuestionTemplateDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<QuestionTemplate>, List<QuestionTemplateDto>>(items)
            };
        }

        public virtual async Task<QuestionTemplateDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<QuestionTemplate, QuestionTemplateDto>(await _questionTemplateRepository.GetAsync(id));
        }

        [Authorize(IBLTermocasaPermissions.QuestionTemplates.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _questionTemplateRepository.DeleteAsync(id);
        }

        [Authorize(IBLTermocasaPermissions.QuestionTemplates.Create)]
        public virtual async Task<QuestionTemplateDto> CreateAsync(QuestionTemplateCreateDto input)
        {

            var questionTemplate = await _questionTemplateManager.CreateAsync(
            input.Code, input.QuestionText, input.AnswerType, input.ChoiceValue
            );

            return ObjectMapper.Map<QuestionTemplate, QuestionTemplateDto>(questionTemplate);
        }

        [Authorize(IBLTermocasaPermissions.QuestionTemplates.Edit)]
        public virtual async Task<QuestionTemplateDto> UpdateAsync(Guid id, QuestionTemplateUpdateDto input)
        {

            var questionTemplate = await _questionTemplateManager.UpdateAsync(
            id,
            input.Code, input.QuestionText, input.AnswerType, input.ChoiceValue, input.ConcurrencyStamp
            );

            return ObjectMapper.Map<QuestionTemplate, QuestionTemplateDto>(questionTemplate);
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(QuestionTemplateExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _questionTemplateRepository.GetListAsync(input.FilterText, input.Code, input.QuestionText, input.AnswerType, input.ChoiceValue);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<QuestionTemplate>, List<QuestionTemplateExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "ProductQuestionTemplates.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public virtual async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new QuestionTemplateExcelDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new DownloadTokenResultDto
            {
                Token = token
            };
        }
        
        public virtual async Task<List<QuestionTemplateDto>> GetListByGuidsAsync(List<Guid> questionTemplateIds)
        {
            return ObjectMapper.Map<List<QuestionTemplate>, List<QuestionTemplateDto>>( await _questionTemplateRepository.GetListAsync(x => questionTemplateIds.Contains(x.Id)));
        }
    }
}