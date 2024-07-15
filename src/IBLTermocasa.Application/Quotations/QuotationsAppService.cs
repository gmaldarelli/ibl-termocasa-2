using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using IBLTermocasa.Permissions;
using IBLTermocasa.Quotations;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using IBLTermocasa.Shared;

namespace IBLTermocasa.Quotations
{
    [RemoteService(IsEnabled = false)]
    [Authorize(IBLTermocasaPermissions.Quotations.Default)]
    public class QuotationsAppService : IBLTermocasaAppService, IQuotationsAppService
    {
        protected IDistributedCache<QuotationExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        protected IQuotationRepository _quotationRepository;
        protected QuotationManager _quotationManager;

        public QuotationsAppService(IQuotationRepository quotationRepository, QuotationManager quotationManager, IDistributedCache<QuotationExcelDownloadTokenCacheItem, string> excelDownloadTokenCache)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _quotationRepository = quotationRepository;
            _quotationManager = quotationManager;
        }

        public virtual async Task<PagedResultDto<QuotationDto>> GetListAsync(GetQuotationsInput input)
        {
            var totalCount = await _quotationRepository.GetCountAsync(input.FilterText, input.Code, input.Name, input.SentDateMin, input.SentDateMax, input.QuotationValidDateMin, input.QuotationValidDateMax, input.ConfirmedDateMin, input.ConfirmedDateMax, input.Status, input.DepositRequired, input.DepositRequiredValueMin, input.DepositRequiredValueMax);
            var items = await _quotationRepository.GetListAsync(input.FilterText, input.Code, input.Name, input.SentDateMin, input.SentDateMax, input.QuotationValidDateMin, input.QuotationValidDateMax, input.ConfirmedDateMin, input.ConfirmedDateMax, input.Status, input.DepositRequired, input.DepositRequiredValueMin, input.DepositRequiredValueMax);

            return new PagedResultDto<QuotationDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Quotation>, List<QuotationDto>>(items)
            };
        }

        public virtual async Task<QuotationDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Quotation, QuotationDto>(await _quotationRepository.GetAsync(id));
        }

        [Authorize(IBLTermocasaPermissions.Quotations.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _quotationRepository.DeleteAsync(id);
        }

        [Authorize(IBLTermocasaPermissions.Quotations.Create)]
        public virtual async Task<QuotationDto> CreateAsync(QuotationCreateDto input)
        {
            var requestForQuotation = ObjectMapper.Map<QuotationCreateDto, Quotation>(input);
            return ObjectMapper.Map<Quotation, QuotationDto>(
                await _quotationManager.CreateAsync(requestForQuotation));
        }

        [Authorize(IBLTermocasaPermissions.Quotations.Edit)]
        public virtual async Task<QuotationDto> UpdateAsync(Guid id, QuotationUpdateDto input)
        {
            var requestForQuotationDto = ObjectMapper.Map<QuotationUpdateDto, Quotation>(input);
            return ObjectMapper.Map<Quotation, QuotationDto>(
                await _quotationManager.UpdateAsync(id, requestForQuotationDto));
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(QuotationExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _quotationRepository.GetListAsync(input.FilterText, input.Code, input.Name, input.SentDateMin, input.SentDateMax, input.QuotationValidDateMin, input.QuotationValidDateMax, input.ConfirmedDateMin, input.ConfirmedDateMax, input.Status, input.DepositRequired, input.DepositRequiredValueMin, input.DepositRequiredValueMax);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<Quotation>, List<QuotationExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Quotations.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public virtual async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new QuotationExcelDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new IBLTermocasa.Shared.DownloadTokenResultDto
            {
                Token = token
            };
        }
    }
}