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
using IBLTermocasa.BillOfMaterials;
using IBLTermocasa.Common;
using IBLTermocasa.Components;
using IBLTermocasa.Materials;
using IBLTermocasa.Products;
using IBLTermocasa.RequestForQuotations;
using MiniExcelLibs;
using Volo.Abp.Content;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Microsoft.Extensions.Caching.Distributed;
using IBLTermocasa.Shared;
using IBLTermocasa.Types;

namespace IBLTermocasa.BillOfMaterials
{
    [RemoteService(IsEnabled = false)]
    [Authorize(IBLTermocasaPermissions.BillOfMaterials.Default)]
    public class BillOfMaterialsAppService : IBLTermocasaAppService, IBillOfMaterialsAppService
    {
        protected IDistributedCache<BillOfMaterialExcelDownloadTokenCacheItem, string> _excelDownloadTokenCache;
        protected IBillOfMaterialRepository _billOfMaterialRepository;
        protected BillOfMaterialManager _billOfMaterialManager;
        
        protected IRequestForQuotationRepository _requestForQuotationRepository;
        protected IProductRepository _productRepository;
        protected IComponentRepository _componentRepository;
        protected IMaterialRepository _materialRepository;

        public BillOfMaterialsAppService(IBillOfMaterialRepository billOfMaterialRepository, 
            BillOfMaterialManager billOfMaterialManager, 
            IDistributedCache<BillOfMaterialExcelDownloadTokenCacheItem, string> excelDownloadTokenCache, 
            IRequestForQuotationRepository requestForQuotationRepository, 
            IProductRepository productRepository, 
            IComponentRepository componentRepository, IMaterialRepository materialRepository)
        {
            _excelDownloadTokenCache = excelDownloadTokenCache;
            _requestForQuotationRepository = requestForQuotationRepository;
            _productRepository = productRepository;
            _componentRepository = componentRepository;
            _materialRepository = materialRepository;
            _billOfMaterialRepository = billOfMaterialRepository;
            _billOfMaterialManager = billOfMaterialManager;
        }

        public virtual async Task<PagedResultDto<BillOfMaterialDto>> GetListAsync(GetBillOfMaterialsInput input)
        {
            var totalCount = await _billOfMaterialRepository.GetCountAsync(input.FilterText, input.Name, input.RequestForQuotationProperty);
            var items = await _billOfMaterialRepository.GetListAsync(input.FilterText, input.Name, input.RequestForQuotationProperty, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<BillOfMaterialDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<BillOfMaterial>, List<BillOfMaterialDto>>(items)
            };
        }

        public virtual async Task<BillOfMaterialDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<BillOfMaterial, BillOfMaterialDto>(await _billOfMaterialRepository.GetAsync(id));
        }

        [Authorize(IBLTermocasaPermissions.BillOfMaterials.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _billOfMaterialRepository.DeleteAsync(id);
        }

        [Authorize(IBLTermocasaPermissions.BillOfMaterials.Create)]
        public virtual async Task<BillOfMaterialDto> CreateAsync(BillOfMaterialCreateDto input)
        {
            var billOfMaterial = ObjectMapper.Map<BillOfMaterialCreateDto, BillOfMaterial>(input);
            return ObjectMapper.Map<BillOfMaterial, BillOfMaterialDto>(await _billOfMaterialManager.CreateAsync(billOfMaterial));
        }

        [Authorize(IBLTermocasaPermissions.BillOfMaterials.Edit)]
        public virtual async Task<BillOfMaterialDto> UpdateAsync(Guid id, BillOfMaterialUpdateDto input)
        {
            var billOfMaterial = ObjectMapper.Map<BillOfMaterialUpdateDto, BillOfMaterial>(input);
            return ObjectMapper.Map<BillOfMaterial, BillOfMaterialDto>(await _billOfMaterialManager.UpdateAsync(id, billOfMaterial));
        }

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(BillOfMaterialExcelDownloadDto input)
        {
            var downloadToken = await _excelDownloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var items = await _billOfMaterialRepository.GetListAsync(input.FilterText, input.BomNumber, input.RequestForQuotationId);

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(ObjectMapper.Map<List<BillOfMaterial>, List<BillOfMaterialExcelDto>>(items));
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "BillOfMaterials.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public virtual async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await _excelDownloadTokenCache.SetAsync(
                token,
                new BillOfMaterialExcelDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new IBLTermocasa.Shared.DownloadTokenResultDto
            {
                Token = token
            };
        }
        [Authorize(IBLTermocasaPermissions.BillOfMaterials.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> billofmaterialIds)
        {
            await _billOfMaterialRepository.DeleteManyAsync(billofmaterialIds);
        }

        [Authorize(IBLTermocasaPermissions.BillOfMaterials.Delete)]
        public virtual async Task DeleteAllAsync(GetBillOfMaterialsInput input)
        {
            await _billOfMaterialRepository.DeleteAllAsync(input.FilterText, input.Name, input.RequestForQuotationProperty);
        }

        public virtual async Task<List<ViewElementPropertyDto<object>>> GenerateBillOfMaterial(Guid id)
        {
            var rfq = await _requestForQuotationRepository.GetAsync(id);
            List<Guid> productIds = rfq.RequestForQuotationItems.SelectMany(item => item.ProductItems.Select(productItem => productItem.ProductId)).ToList();
            var products = await _productRepository.GetListAsync( x => productIds.Contains(x.Id));
            var productComponentIds = products.SelectMany(product => product.ProductComponents.Select(component => component.ComponentId));
            var components = await  _componentRepository.GetListAsync(x =>
                productComponentIds.Contains(x.Id));
            var billOfMaterial = new BillOfMaterial(
                id: Guid.NewGuid(),
                requestForQuotationProperty: new RequestForQuotationProperty(
                    id: rfq.Id,
                    name: rfq.QuoteNumber,
                    organizationName: rfq.OrganizationProperty?.Name,
                    rfqDateDocument : rfq.DateDocument ?? rfq.CreationTime,
                    rfqNumber: rfq.QuoteNumber
                    ),
                listItems: await this.getBillOfMaterialItems(rfq, products, components),
                bomNumber: $"BOM-{rfq.QuoteNumber}-{DateTime.Now:yyyy-MM-dd}",
                notes: ""
            );
             var saved = await _billOfMaterialManager.CreateAsync(billOfMaterial);
             rfq.Status = RfqStatus.IN_PROGRESS_BOM;
             await _requestForQuotationRepository.UpdateAsync(rfq);
             return
             [
                 new ViewElementPropertyDto<object>("Id", saved.Id),
                 new ViewElementPropertyDto<object>("BomNumber", saved.BomNumber),
                 new ViewElementPropertyDto<object>("RfqId", rfq.Id),
                 new ViewElementPropertyDto<object>("RfqCustomerName", rfq.OrganizationProperty!.Name!),
                 new ViewElementPropertyDto<object>("RfqNumber", rfq.QuoteNumber),
                 new ViewElementPropertyDto<object>("BomDate", saved.CreationTime),
                 new ViewElementPropertyDto<object>("RfqDate", rfq.DateDocument ?? rfq.CreationTime)
             ];
        }

        private async Task<List<BomItem>?> getBillOfMaterialItems(RequestForQuotation rfq, List<Product> products,
            List<Component> components)
        {
            List<BomItem> items = new List<BomItem>();

             foreach (var rfqItem in rfq.RequestForQuotationItems)
            {
                var product = products.FirstOrDefault(x => x.Id == rfqItem.ProductItems.FirstOrDefault()!.ProductId);
                if (product != null)
                {
                    items.Add(new BomItem(
                        id: Guid.NewGuid(),
                        requestForQuotationItemId:  rfqItem.Id,
                        quantity: rfqItem.Quantity,
                        bomProductItems: await this.getBillOfMaterialBOMProductItems(rfqItem, product, components)
                        )
                    );
                }
            }

            return items;
        }

        private async Task<List<BomProductItem>> getBillOfMaterialBOMProductItems(RequestForQuotationItem rfqItem, Product product,
            List<Component> components)
        {
            List<BomProductItem> productItems = new List<BomProductItem>();
            foreach (var productItem in rfqItem.ProductItems)
            {
                productItems.Add(new BomProductItem(
                    id: Guid.NewGuid(),
                    productItemId: productItem.Id,
                    productItemName: product.Name,
                    productId:  product.Id,
                    parentBomProductItemId: productItem.ParentId,
                    bomComponents: await this.getBillOfMaterialComponents(productItem, product, components)
                ));
            }

            return productItems;
        }

        private async Task<List<BomComponent>> getBillOfMaterialComponents(ProductItem productItem, Product product,
            List<Component> sourceComponents)
        {
            
            List<BomComponent> components = new List<BomComponent>();
            var componentsWhitDefalutMaterial = sourceComponents.Where(x => x.ComponentItems.Any(y => y.IsDefault)).ToList();
            var materialIds = componentsWhitDefalutMaterial.Select(x => x.ComponentItems.FirstOrDefault(y => y.IsDefault)!.MaterialId).ToList();
            var materials = await _materialRepository.GetListAsync(x => materialIds.Contains(x.Id));
            
            foreach (var productComponents in product.ProductComponents)
            {
                var component = sourceComponents.FirstOrDefault(x => x.Id == productComponents.ComponentId);
                var isDefaultMaterialSelected = component.ComponentItems.Any(x => x.IsDefault);
                bool isDefault = componentsWhitDefalutMaterial.Contains(component);
                Material material = isDefault ? materials.FirstOrDefault(x => x.Id == component.ComponentItems.FirstOrDefault(y => y.IsDefault)!.MaterialId) : null;
                components.Add(new BomComponent(
                    id: Guid.NewGuid(),
                    componentId: component.Id,
                    componentName: component.Name,
                    materialId: (material is not null) ? material.Id : Guid.Empty,
                    materialName: ((material is not null) ? material.Name : null) ?? string.Empty,
                    materialPrice: (material is not null) ? material.StandardPrice : Decimal.Zero,
                    measureUnit: (material is not null) ? material.MeasureUnit : MeasureUnit.PZ,
                    quantity: 0,
                    price: 0
                ));
            }
            return components;
        }
    }
}