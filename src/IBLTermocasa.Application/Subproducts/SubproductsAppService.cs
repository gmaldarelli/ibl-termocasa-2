using IBLTermocasa.Shared;
using IBLTermocasa.Products;
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
using IBLTermocasa.Subproducts;

namespace IBLTermocasa.Subproducts
{
    [RemoteService(IsEnabled = false)]
    [Authorize(IBLTermocasaPermissions.Subproducts.Default)]
    public abstract class SubproductsAppServiceBase : IBLTermocasaAppService
    {

        protected ISubproductRepository _subproductRepository;
        protected SubproductManager _subproductManager;
        protected IRepository<Product, Guid> _productRepository;

        public SubproductsAppServiceBase(ISubproductRepository subproductRepository, SubproductManager subproductManager, IRepository<Product, Guid> productRepository)
        {

            _subproductRepository = subproductRepository;
            _subproductManager = subproductManager; _productRepository = productRepository;
        }

        public virtual async Task<PagedResultDto<SubproductDto>> GetListByProductIdAsync(GetSubproductListInput input)
        {
            var subproducts = await _subproductRepository.GetListByProductIdAsync(
                input.ProductId,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount);

            return new PagedResultDto<SubproductDto>
            {
                TotalCount = await _subproductRepository.GetCountByProductIdAsync(input.ProductId),
                Items = ObjectMapper.Map<List<Subproduct>, List<SubproductDto>>(subproducts)
            };
        }
        public virtual async Task<PagedResultDto<SubproductWithNavigationPropertiesDto>> GetListWithNavigationPropertiesByProductIdAsync(GetSubproductListInput input)
        {
            var subproducts = await _subproductRepository.GetListWithNavigationPropertiesByProductIdAsync(
                input.ProductId,
                input.Sorting,
                input.MaxResultCount,
                input.SkipCount);

            return new PagedResultDto<SubproductWithNavigationPropertiesDto>
            {
                TotalCount = await _subproductRepository.GetCountByProductIdAsync(input.ProductId),
                Items = ObjectMapper.Map<List<SubproductWithNavigationProperties>, List<SubproductWithNavigationPropertiesDto>>(subproducts)
            };
        }

        public virtual async Task<PagedResultDto<SubproductWithNavigationPropertiesDto>> GetListAsync(GetSubproductsInput input)
        {
            var totalCount = await _subproductRepository.GetCountAsync(input.FilterText, input.OrderMin, input.OrderMax, input.Name, input.IsSingleProduct, input.Mandatory);
            var items = await _subproductRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.OrderMin, input.OrderMax, input.Name, input.IsSingleProduct, input.Mandatory, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<SubproductWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<SubproductWithNavigationProperties>, List<SubproductWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<SubproductWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<SubproductWithNavigationProperties, SubproductWithNavigationPropertiesDto>
                (await _subproductRepository.GetWithNavigationPropertiesAsync(id));
        }

        public virtual async Task<SubproductDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Subproduct, SubproductDto>(await _subproductRepository.GetAsync(id));
        }

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetProductLookupAsync(LookupRequestDto input)
        {
            var query = (await _productRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null &&
                         x.Name.Contains(input.Filter));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Product>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Product>, List<LookupDto<Guid>>>(lookupData)
            };
        }

        [Authorize(IBLTermocasaPermissions.Subproducts.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _subproductRepository.DeleteAsync(id);
        }

        [Authorize(IBLTermocasaPermissions.Subproducts.Create)]
        public virtual async Task<SubproductDto> CreateAsync(SubproductCreateDto input)
        {

            var subproduct = await _subproductManager.CreateAsync(input.ProductId,
            input.SingleProductId, input.Order, input.Name, input.IsSingleProduct, input.Mandatory
            );

            return ObjectMapper.Map<Subproduct, SubproductDto>(subproduct);
        }

        [Authorize(IBLTermocasaPermissions.Subproducts.Edit)]
        public virtual async Task<SubproductDto> UpdateAsync(Guid id, SubproductUpdateDto input)
        {

            var subproduct = await _subproductManager.UpdateAsync(
            id, input.ProductId,
            input.SingleProductId, input.Order, input.Name, input.IsSingleProduct, input.Mandatory
            );

            return ObjectMapper.Map<Subproduct, SubproductDto>(subproduct);
        }
    }
}