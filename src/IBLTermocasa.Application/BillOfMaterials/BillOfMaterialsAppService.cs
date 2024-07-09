using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using IBLTermocasa.Permissions;
using IBLTermocasa.BillOfMaterials;
using IBLTermocasa.Common;
using IBLTermocasa.Common.Formulas;
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
using static IBLTermocasa.Types.AnswerType;

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
        private async Task<List<BomItem>?> getBillOfMaterialItems(RequestForQuotation rfq, List<Product> products, List<Component> components)
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















        public virtual async Task<List<BomItemDto>> CalculateConsumption(Guid id)
        {
            var billOfMaterial = await _billOfMaterialRepository.GetAsync(id);
            List<BomItemDto> listItems = ObjectMapper.Map<List<BomItem>, List<BomItemDto>>(billOfMaterial.ListItems);
            return await this.CalculateConsumption(id, listItems);
        }
        
        
        
        public virtual async Task<List<BomItemDto>> CalculateConsumption(Guid id, List<BomItemDto> listItems)
        {
            var billOfMaterial = await _billOfMaterialRepository.GetAsync(id);
            var rfq = await _requestForQuotationRepository.GetAsync(billOfMaterial.RequestForQuotationProperty.Id);
            var productIds = billOfMaterial.ListItems.SelectMany(x => x.BomProductItems.Select(y => y.ProductId)).ToList();
            var products = await _productRepository.GetListAsync(x => productIds.Contains(x.Id));
            var componentIds = products.SelectMany(x => x.ProductComponents.Select(y => y.ComponentId)).ToList();
            var components = await _componentRepository.GetListAsync(x => componentIds.Contains(x.Id));
            
            Dictionary<string,ProductComponent> formulaElementCodes =  
                this.ExtractFormulaProductComponentCodes(billOfMaterial, products, components);
            Dictionary<string,object> formulaQuestionTemplateCodes =  
                this.ExtractFormulaQuestionTemplateCodes(rfq, products, components);
            var  cunsumptionFormulaElements = 
                this.ExtractCunsumptionFormulaElement(billOfMaterial, products, components);
            formulaQuestionTemplateCodes.ToList().ForEach(x =>
            {
                cunsumptionFormulaElements.ForEach(y =>
                {
                    y.SetVariableValue(x.Key, x.Value);
                });
            });
            cunsumptionFormulaElements =  this.CompleteCunsumptionFormulaElement(cunsumptionFormulaElements);
            cunsumptionFormulaElements.ForEach(x =>
            {
                Console.WriteLine($">>>>>>>>>>>>>>>>>>>>>>{x.PlaceHolder} = {x.Value}");
            });

            foreach (var item in cunsumptionFormulaElements)
            {
                listItems.SelectMany(x => x.BomProductItems).ToList().ForEach(x =>
                {
                    x.BomComponents.ForEach(y =>
                    {
                        if (y.ComponentId == item.ProductItemId)
                        {
                            try
                            {
                                y.Quantity = decimal.TryParse(item.Value.ToString(), out decimal value) ? value : 0;
                            }
                            catch (Exception e)
                            {
                                y.Quantity = 0;
                            }
                        }
                    });
                });
            }
            
            
            /*foreach (var item in listItems)
            {
                foreach (var productItem in item.BomProductItems)
                {
                    foreach (var component in productItem.BomComponents)
                    {
                        cunsumptionFormulaElements.Where(x => 
                            x.ProductItemId == productItem.ProductItemId  && x.Code.Equals(component.Co)).ToList();
                        var product = products.FirstOrDefault(x => x.Id == productItem.ProductId);
                        if (product is null)
                        {
                            continue;
                        }
                       var productComponent = product.ProductComponents.FirstOrDefault(x => x.ComponentId == component.ComponentId);
                        if(productComponent is null)
                        {
                            continue;
                        }
                    }
                }
            }*/
            
            return listItems;
        }

        private List<ProductComponentCunsumptionFormulaElement> CompleteCunsumptionFormulaElement(List<ProductComponentCunsumptionFormulaElement> cunsumptionFormulaElements)
        {
            for (int i = 0; i < cunsumptionFormulaElements.Count; i++)
            {
                foreach (var item in cunsumptionFormulaElements)
                {

                    foreach (var subItem in cunsumptionFormulaElements)
                    {
                        if (!subItem.IsValueAvailable)
                        {
                            subItem.SetVariableValue(item.PlaceHolder, item.Value);
                        }
                    }
                }
            }
            
            return cunsumptionFormulaElements;
        }

        private List<ProductComponentCunsumptionFormulaElement> ExtractCunsumptionFormulaElement(BillOfMaterial billOfMaterial,
            List<Product> products, List<Component> components)
        {
            List<ProductComponentCunsumptionFormulaElement> formulaElementCodes = new List<ProductComponentCunsumptionFormulaElement>();
            foreach (var item in billOfMaterial.ListItems)
            {
                foreach (var productItem in item.BomProductItems)
                {
                    var product = products.FirstOrDefault(x => x.Id == productItem.ProductId);
                    if (product is null)
                    {
                        continue;
                    }
                    string productCode = $"P[{product.Code}]";
                    foreach (var productComponent in product.ProductComponents)
                    {
                        string componentCode = $"{{{productCode}.C[{productComponent.Code}]}}";
                        //formulaElementCodes.Add(new ProductComponentCunsumptionFormulaElement(productComponent.ComponentId, productItem.Id, componentCode, productComponent.Code, productComponent.ConsumptionCalculation));
                    }

                    foreach (var subProduct in product.SubProducts)
                    {
                        var subProductItem = products.FirstOrDefault(x => x.Id == subProduct.ProductId);
                        string subProductCode = $"P[{product.Code}].P[{subProduct.Code}]";
                        foreach (var productComponent in subProductItem.ProductComponents)
                        {
                            string componentCode = $"{{{subProductCode}.C[{productComponent.Code}]}}";
                            //formulaElementCodes.Add(new ProductComponentCunsumptionFormulaElement(productComponent.ComponentId, productItem.Id, componentCode, productComponent.Code, productComponent.ConsumptionCalculation));
                        }
                    }
                }
            }
            return formulaElementCodes;
        }

        private Dictionary<string, ProductComponent> ExtractFormulaProductComponentCodes( BillOfMaterial billOfMaterial, List<Product> products, List<Component> components)
        {
            Dictionary<string, ProductComponent> formulaElementCodes = new Dictionary<string, ProductComponent>();
            foreach (var item in billOfMaterial.ListItems)
            {
                foreach (var productItem in item.BomProductItems)
                {
                    var product = products.FirstOrDefault(x => x.Id == productItem.ProductId);
                    if (product is null)
                    {
                        continue;
                    }
                    string productCode = $"P[{product.Code}]";
                    foreach (var productComponent in product.ProductComponents)
                    {
                        string componentCode = $"{{{productCode}.C[{productComponent.Code}]}}";
                        formulaElementCodes.Add(componentCode, productComponent);
                    }
                    
                    foreach (var subProduct in product.SubProducts)
                    {
                        var subProductItem = products.FirstOrDefault(x => x.Id == subProduct.ProductId);
                        string subProductCode = $"P[{product.Code}].P[{subProduct.Code}]";
                        foreach (var productComponent in subProductItem.ProductComponents)
                        {
                            string componentCode = $"{{{subProductCode}.C[{productComponent.Code}]}}";
                            formulaElementCodes.Add(componentCode, productComponent);
                        }
                    }
                }
            }
            return formulaElementCodes;
        }
        
        private Dictionary<string, object> ExtractFormulaQuestionTemplateCodes(RequestForQuotation rfq, List<Product> products, List<Component> components)
        {
            Dictionary<string, object> formulaQuestionTemplateCodes = new Dictionary<string, object>();
            foreach (var item in rfq.RequestForQuotationItems)
            {
                foreach (var productItem in item.ProductItems)
                {
                    var product = products.FirstOrDefault(x => x.Id == productItem.ProductId);
                    if (product is null)
                    {
                        continue;
                    }
                    string productCode = $"P[{product.Code}]";
                    foreach (var questionTemplate in product.ProductQuestionTemplates)
                    {
                        var answer = productItem.Answers.FirstOrDefault(x => x.QuestionId == questionTemplate.QuestionTemplateId)?? null;
                        string questionCode = $"{{{productCode}.Q[{questionTemplate.Code}]}}";
                        var answerAnswerValue = answer?.AnswerValue;
                        object value = null;
                        if (answer is not null)
                        {
                            switch (answer.AnswerType)
                            {
                                case BOOLEAN:
                                    value = bool.TryParse(answer.AnswerValue, out bool boolValue) && boolValue;
                                    break;
                                case DATE:
                                    value = DateTime.TryParse(answerAnswerValue, out DateTime dateValue) ? dateValue : DateTime.MinValue;
                                    break;
                                case LARGE_TEXT:
                                    value = answerAnswerValue;
                                    break;
                                case NUMBER:
                                    value = double.TryParse(answerAnswerValue, out double doubleValue) ? doubleValue : 0;
                                    break;
                                case TEXT:
                                    value= answerAnswerValue;
                                    break;
                                case CHOICE:
                                    value = answerAnswerValue;
                                    break;
                                
                            }
                            formulaQuestionTemplateCodes.Add(questionCode, value);
                        }
                    }

                    foreach (var subProduct in product.SubProducts)
                    {
                        var subProductItem = products.FirstOrDefault(x => x.Id == subProduct.ProductId);
                        string subProductCode = $"P[{product.Code}].P[{subProduct.Code}]";
                        foreach (var questionTemplate in subProductItem.ProductQuestionTemplates)
                        {
                            string questionCode = $"{{{subProductCode}.Q[{questionTemplate.Code}]}}";
                            formulaQuestionTemplateCodes.Add(questionCode, questionTemplate);
                        }
                    }
                }
            }
            return formulaQuestionTemplateCodes;
        }

        
        
    public List<string> ExtractVariables(string formula)
    {
        var matches = Regex.Matches(formula, @"\{([^}]+)\}");
        var variables = new List<string>();
        foreach (Match match in matches)
        {
            variables.Add(match.Groups[1].Value);
        }
        return variables;
    }
    
    
    
    
    
    /*
    private object applyFormula(ProductComponent productComponent, RequestForQuotationItem? requestForQuotationItem, RequestForQuotation rfq, List<Product> products, List<Component> components)
    {
        var formula = productComponent.ConsumptionCalculation;
        if (formula is null || formula.IsNullOrEmpty() || requestForQuotationItem is null)
        {
            return 0;
        }
        var quantity = 0;
        var variables = ExtractVariables(formula);
        var quantities = GetQuantities(variables, rfq, products, components);
        return null;
    }
    */
    /*
    public List<KeyValuePair<string, object>> GetQuantities(List<string> variables, RequestForQuotation rfq, List<Product> products, List<Component> components)
    {
        var quantities = new List<KeyValuePair<string, object>>();
        foreach (var variable in variables)
        {
            //data una variabie tipo {P[MONO].P[Spalletta].C[GUI]} e un oggetto Product in input legga il valore della quantità del componente
            var quantity = getValue(variable, rfq, products, components);
            quantities.Add(new KeyValuePair<string, object>(variable, quantity));
        }
        return quantities;
    }
    */

    /*
    private object getValue(string variable, RequestForQuotation rfq, List<Product> products, List<Component> components)
    {
        var items = variable.Split(new[] { "." }, StringSplitOptions.None);
        var adv = (string[])items.Clone();
        var productcode = items[0].Replace("P","").Replace("[", "").Replace("]", "");
        Product _product = null;
        var product = products.FirstOrDefault(x => x.Code.Equals(productcode));
        if(product == null)
        {
            return 0;
        }
        if(adv.Length == 1)
        {
            return 0;
        }
        _product = product;
        adv = adv.Skip(1).Take(adv.Length - 1).ToArray();
        string subpart = adv[0].Substring(0, 1);
        if(subpart == "P")
        {
            var subproductcode = adv[0].Replace("P","").Replace("[", "").Replace("]", "");
            if (items.Length == 2)
            {
                throw new Exception($"Value path is not correct for the variable {variable}");
            }

            var id = _product.SubProducts.FirstOrDefault(x => x.Code.Equals(subproductcode))!.ProductIds.FirstOrDefault();
            
            _product =  _productRepository.GetAsync(id).Result;
            adv = adv.Skip(1).Take(adv.Length - 1).ToArray();
        }
        subpart = adv[0].Substring(0, 1);
        if(subpart == "C")
        {
            var componentcode = adv[0].Replace("C","").Replace("[", "").Replace("]", "");
            Component _component = _product.ProductComponents.FirstOrDefault(x => x.Code.Equals(componentcode));
            return _component.Quantity;
        }
        if(subpart == "Q")
        {
            var _questionTemplate = items[1].Replace("Q","").Replace("[", "").Replace("]", "");
            QuestionTempate _question = _product.QuestionTemplates.Find(x => x.Code.Equals(_questionTemplate));
            return _question.Answer.AnswerValue;
        }
        throw new Exception($"Value path is not correct for the variable {variable}");
    }
    */
        
        
        
        
        
        
        
        
        
        
        
        
        
    }
}