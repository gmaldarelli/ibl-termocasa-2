using System;
using System.Collections.Generic;
using System.Linq;
using IBLTermocasa.Common;
using IBLTermocasa.Components;
using IBLTermocasa.QuestionTemplates;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace IBLTermocasa.Products
{
    public class ProductDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        public string Code { get; set; } = null!;
        public string? ParentPlaceHolder { get; set; } = null;
        public string PlaceHolder =>  PlaceHolderUtils.GetPlaceHolder(PlaceHolderType.PRODUCT, Code, ParentPlaceHolder);
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsAssembled { get; set; }
        public bool IsInternal { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;

        public List<SubProductDto> SubProducts { get; set; } = new();
        public List<ProductComponentDto> ProductComponents { get; set; } = new();
        public List<ProductQuestionTemplateDto> ProductQuestionTemplates { get; set; } = new();
        public object CodeAndName => $"{Code} - {Name}";

        public List<SubProductDto> SubProductReorder(SubProductDto subProduct)
        {
            List<SubProductDto> list = this.SubProducts.OrderBy(x => x.Order).ToList();
            if (SubProducts.Count == 0)
            {
                subProduct.Order = 1;
                list.Add(subProduct);
                return list;
            }
            var order = subProduct.Order;
            list.ForEach(item =>
            {
                if (item.Order >= order)
                {
                    item.Order++;
                }
            });
            list.Add(subProduct);
            list = list.OrderBy(x => x.Order).ToList();
            return list;
        }

        public List<ProductComponentDto> ProductComponentsReorder(ProductComponentDto productComponent)
        {
            List<ProductComponentDto> list = this.ProductComponents.OrderBy(x => x.Order).ToList();
            if (ProductComponents.Count == 0)
            {
                productComponent.Order = 1;
                list.Add(productComponent);
                return list;
            }
            var order = productComponent.Order;
            list.ForEach(item =>
            {
                if (item.Order >= order)
                {
                    item.Order++;
                }
            });
            list.Add(productComponent);
            list = list.OrderBy(x => x.Order).ToList();
            return list;
        }

        public List<ProductQuestionTemplateDto> ProductQuestionTemplatesReorder(ProductQuestionTemplateDto productQuestionTemplate)
        {
            List<ProductQuestionTemplateDto> list = this.ProductQuestionTemplates.OrderBy(x => x.Order).ToList();
            if (ProductQuestionTemplates.Count == 0)
            {
                productQuestionTemplate.Order = 1;
                list.Add(productQuestionTemplate);
                return list;
            }
            var order = productQuestionTemplate.Order;
            list.ForEach(item =>
            {
                if (item.Order >= order)
                {
                    item.Order++;
                }
            });
            list.Add(productQuestionTemplate);
            list = list.OrderBy(x => x.Order).ToList();
            return list;
        }
    }
}