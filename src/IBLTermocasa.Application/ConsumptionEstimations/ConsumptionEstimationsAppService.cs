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
using IBLTermocasa.ConsumptionEstimations;
using IBLTermocasa.Products;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using IBLTermocasa.Shared;

namespace IBLTermocasa.ConsumptionEstimations
{
    [RemoteService(IsEnabled = false)]
    [Authorize(IBLTermocasaPermissions.ConsumptionEstimations.Default)]
    public class ConsumptionEstimationsAppService : IBLTermocasaAppService, IConsumptionEstimationsAppService
    {
        protected IDistributedCache<ConsumptionEstimationExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        protected IConsumptionEstimationRepository _consumptionEstimationRepository;
        protected ConsumptionEstimationManager _consumptionEstimationManager;
        protected ProductsAppService _productsAppService;

        public ConsumptionEstimationsAppService(IConsumptionEstimationRepository consumptionEstimationRepository, 
            ConsumptionEstimationManager consumptionEstimationManager, 
            IDistributedCache<ConsumptionEstimationExcelDownloadTokenCacheItem, string> excelDownloadTokenCache
            , ProductsAppService productsAppService
            )
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _productsAppService = productsAppService;
            _consumptionEstimationRepository = consumptionEstimationRepository;
            _consumptionEstimationManager = consumptionEstimationManager;
        }

        public virtual async Task<PagedResultDto<ConsumptionEstimationDto>> GetListAsync(GetConsumptionEstimationsInput input)
        {
            var totalCount = await _consumptionEstimationRepository.GetCountAsync(input.FilterText, input.IdProduct);
            var items = await _consumptionEstimationRepository.GetListAsync(input.FilterText, input.IdProduct, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<ConsumptionEstimationDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<ConsumptionEstimation>, List<ConsumptionEstimationDto>>(items)
            };
        }

        public virtual async Task<ConsumptionEstimationDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<ConsumptionEstimation, ConsumptionEstimationDto>(await _consumptionEstimationRepository.GetAsync(id));
        }

        [Authorize(IBLTermocasaPermissions.ConsumptionEstimations.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _consumptionEstimationRepository.DeleteAsync(id);
        }

        [Authorize(IBLTermocasaPermissions.ConsumptionEstimations.Create)]
        public virtual async Task<ConsumptionEstimationDto> CreateAsync(ConsumptionEstimationCreateDto input)
        {
            var product = await _productsAppService.GetAsync(input.IdProduct);
            foreach (var productComponent in product.ProductComponents)
            {
                productComponent.ParentId = product.Id;
                productComponent.ParentPlaceHolder = product.PlaceHolder;
                var consumptionProduct =new ConsumptionProductDto(Guid.NewGuid(), 
                    productComponent.Id,
                    productComponent.PlaceHolder,
                    $"{productComponent.Code} - {productComponent.Name}"
                    );
                input.ConsumptionProduct.Add(consumptionProduct);
            }

            if (product.IsAssembled)
            {
                var listSubProduct = await _productsAppService.GetListByIdAsync(product.SubProducts.Select(x => x.ProductId).ToList());

                foreach (var dto in listSubProduct)
                {
                    var selectedSubProduct = product.SubProducts.FirstOrDefault(x => x.ProductId == dto.Id);
                    if(selectedSubProduct is null){ continue; }
                    selectedSubProduct.ParentId = product.Id;
                    selectedSubProduct.ParentPlaceHolder = product.PlaceHolder;

                    foreach (var productComponent in dto.ProductComponents)
                    {
                        productComponent.ParentId = selectedSubProduct.Id;
                        productComponent.ParentPlaceHolder = selectedSubProduct.PlaceHolder;
                        var consumptionProduct = new ConsumptionProductDto(Guid.NewGuid(), 
                            productComponent.Id, 
                            productComponent.PlaceHolder,
                            $"{productComponent.Code} - {productComponent.Name}" );
                        input.ConsumptionProduct.Add(consumptionProduct);
                    }
                }
            }
            var consumptionEstimation = ObjectMapper.Map<ConsumptionEstimationCreateDto, ConsumptionEstimation>(input);
            return ObjectMapper.Map<ConsumptionEstimation, ConsumptionEstimationDto>(
                await _consumptionEstimationManager.CreateAsync(consumptionEstimation));
        }

        [Authorize(IBLTermocasaPermissions.ConsumptionEstimations.Edit)]
        public virtual async Task<ConsumptionEstimationDto> UpdateAsync(Guid id, ConsumptionEstimationUpdateDto input)
        {
            var requestForQuotationDto = ObjectMapper.Map<ConsumptionEstimationUpdateDto, ConsumptionEstimation>(input);
            var entity = await _consumptionEstimationManager.UpdateAsync(id, requestForQuotationDto);
            var dto = ObjectMapper.Map<ConsumptionEstimation, ConsumptionEstimationDto>(entity);
            return dto;
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(ConsumptionEstimationExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _consumptionEstimationRepository.GetListAsync(input.FilterText, input.ProductId);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<ConsumptionEstimation>, List<ConsumptionEstimationExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "ConsumptionEstimations.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public virtual async Task<IBLTermocasa.Shared.DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new ConsumptionEstimationExcelDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new IBLTermocasa.Shared.DownloadTokenResultDto
            {
                Token = token
            };
        }
        [Authorize(IBLTermocasaPermissions.ConsumptionEstimations.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> consumptionestimationIds)
        {
            await _consumptionEstimationRepository.DeleteManyAsync(consumptionestimationIds);
        }

        [Authorize(IBLTermocasaPermissions.ConsumptionEstimations.Delete)]
        public virtual async Task DeleteAllAsync(GetConsumptionEstimationsInput input)
        {
            await _consumptionEstimationRepository.DeleteAllAsync(input.FilterText, input.IdProduct);
        }
        [Authorize(IBLTermocasaPermissions.ConsumptionEstimations.Create)]
        public virtual async Task<ConsumptionEstimationDto> GetAsyncByProduct(Guid idProduct)
        {
            
            /*var product = await _productsAppService.GetAsync(id);
            if (product != null)    
            {
                throw new UserFriendlyException("Product not found");
            }*/

            try
            {

                var entity = await _consumptionEstimationRepository.FirstOrDefaultAsync(ix => ix.IdProduct == idProduct);
                if (entity == null)
                {
                    return await this.CreateAsync(new ConsumptionEstimationCreateDto
                    {
                        IdProduct = idProduct
                    });
                } else
                {
                    return ObjectMapper.Map<ConsumptionEstimation, ConsumptionEstimationDto>(entity);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}