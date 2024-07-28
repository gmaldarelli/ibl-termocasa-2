using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using IBLTermocasa.BillOfMaterials;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using IBLTermocasa.Permissions;
using IBLTermocasa.RequestForQuotations;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using IBLTermocasa.Shared;
using IBLTermocasa.Types;

namespace IBLTermocasa.Quotations
{
    [RemoteService(IsEnabled = false)]
    [Authorize(IBLTermocasaPermissions.Quotations.Default)]
    public class QuotationsAppService : IBLTermocasaAppService, IQuotationsAppService
    {
        protected IDistributedCache<QuotationExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        protected IQuotationRepository _quotationRepository;
        protected QuotationManager _quotationManager;
        protected IBillOfMaterialRepository _billOfMaterialRepository;
        protected BillOfMaterialManager _billOfMaterialManager;
        protected IRequestForQuotationRepository _requestForQuotationRepository;

        public QuotationsAppService(IQuotationRepository quotationRepository, QuotationManager quotationManager, IDistributedCache<QuotationExcelDownloadTokenCacheItem, string> excelDownloadTokenCache, IBillOfMaterialRepository billOfMaterialRepository, BillOfMaterialManager billOfMaterialManager, IRequestForQuotationRepository requestForQuotationRepository)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _billOfMaterialRepository = billOfMaterialRepository;
            _billOfMaterialManager = billOfMaterialManager;
            _requestForQuotationRepository = requestForQuotationRepository;
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

            return new RemoteStreamContent(memoryStream, "Quotations.xlsx", "public Task GenerateQuotation(Guid id);application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
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
        
        public virtual async Task<QuotationDto> GenerateQuotation(Guid id)
        {
            var bom = await _billOfMaterialRepository.GetAsync(id);
            var rfq = await _requestForQuotationRepository.GetAsync(bom.RequestForQuotationProperty.Id);
           
            var quotation = new Quotation(
                id: Guid.NewGuid(), 
                idRFQ: rfq.Id, 
                idBOM: bom.Id, 
                code: bom.BomNumber.Replace("BOM","QUOT"), 
                name: $"Quotation for {rfq.QuoteNumber} of date {rfq.DateDocument.ToString()}", 
                creationDate: DateTime.Now,
                sentDate: null, 
                quotationValidDate: null, 
                status: QuotationStatus.NEW, 
                confirmedDate: null, 
                depositRequired: true, 
                depositRequiredValue: 0, 
                quotationItems: new List<QuotationItem>(),
                discount: rfq.Discount,
                markUp: IBLTermocasaConsts.Markup);
            foreach (var rfqRequestForQuotationItem in rfq.RequestForQuotationItems)
            {
                var parenProductItem = rfqRequestForQuotationItem.ProductItems.FirstOrDefault(x => x.ParentId is null);
                var bomItem = bom.ListItems.FirstOrDefault(x => x.RequestForQuotationItemId == rfqRequestForQuotationItem.Id);
                double materialCost = 0;
                double laborCost = 0;
                double totalCost = 0;
                if (bomItem is null)
                {
                    throw new UserFriendlyException("BOM Item not found for RFQ Item");
                }
                foreach (var bomProductItem in bomItem.BomProductItems)
                {
                    foreach (var component in bomProductItem.BomComponents)
                    {
                        materialCost += (double)component.Price;
                    }

                    foreach (var bowItem in bomProductItem.BowItems)
                    {
                        laborCost += (double)bowItem.Price;
                    }
                }
                int quantity = rfqRequestForQuotationItem.Quantity;
                totalCost = (materialCost *  (double)quantity) + (laborCost *  (double)quantity);
                List<double> markUps = quotation.MarkUps;
                double discount = (double)rfq.Discount;
                double sellingPrice1 = totalCost * (1 + (markUps[0] / 100));
                double sellingPrice2 = sellingPrice1 * (1 + (markUps[1] / 100));
                double sellingPrice3 = sellingPrice2 * (1 + (markUps[2] / 100));
                double finalSellingPrice = sellingPrice3 * (100- discount) / 100;
                double markup = (sellingPrice3 - totalCost) / totalCost * 100;
                quotation.QuotationItems.Add(new QuotationItem(
                    Guid.NewGuid(), 
                    rfqRequestForQuotationItem.Id, 
                    bomItem.Id, 
                    parenProductItem.ProductId, 
                    "", 
                    parenProductItem.ProductName, 
                    laborCost, materialCost, totalCost, sellingPrice3, markup, discount, finalSellingPrice, quantity));
            }
            
            var quotationResult =  ObjectMapper.Map<Quotation, QuotationDto>(
                await _quotationManager.CreateAsync(quotation));
            bom.Status = BomStatusType.RFP_GENERATED;
            await _billOfMaterialRepository.UpdateAsync(bom);
            return quotationResult;
        }
    }
}