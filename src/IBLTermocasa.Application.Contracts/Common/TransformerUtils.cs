using System;
using System.Collections.Generic;
using System.Linq;
using IBLTermocasa.Products;

namespace IBLTermocasa.Common;

public class TransformerUtils
{
    public PlaceHolderTreeItemData GenerateTreeItems(ProductDto rootProduct,  
        List<ProductDto> subProducts,
        Dictionary<PlaceHolderType, string> icons)
    {
         var treeItemData =new PlaceHolderTreeItemData(
             product: rootProduct,
            parent: null,
            icon: icons[PlaceHolderType.PRODUCT], 
            isExpanded: true, 
            treeItems: new HashSet<PlaceHolderTreeItemData>());
         var children = GenerateSubTreeItems(rootProduct.SubProducts, rootProduct.ProductComponents, rootProduct.ProductQuestionTemplates, subProducts, treeItemData, icons);
         treeItemData.TreeItems = children;
         return treeItemData;
    }

    private  HashSet<PlaceHolderTreeItemData> GenerateSubTreeItems(List<SubProductDto> productSubProducts,
        List<ProductComponentDto> productProductComponents,
        List<ProductQuestionTemplateDto> productProductQuestionTemplates, 
        List<ProductDto> subProducts,
        PlaceHolderTreeItemData? parent, Dictionary<PlaceHolderType, string> icons)
    {
        Console.WriteLine("Generating sub tree items"); 
        var treeItems = new HashSet<PlaceHolderTreeItemData>();
        if (productSubProducts is { Count: > 0 })
        {
            var subProductIds = productSubProducts.Select(subProduct => subProduct.ProductId).ToList();
          foreach (var id in subProductIds)
            {
                var subProduct = subProducts.FirstOrDefault(p => p.Id == id);
                if(subProduct == null)
                {
                    continue;
                }
                var treeItemData = new PlaceHolderTreeItemData(
                    product: subProduct,
                    parent: parent ?? null,
                    icon: icons[PlaceHolderType.PRODUCT],
                    isExpanded: false,
                    treeItems: new HashSet<PlaceHolderTreeItemData>());
                Console.WriteLine("Generating sub tree items1.4"); 
                treeItemData.TreeItems = GenerateSubTreeItems(subProduct.SubProducts, subProduct.ProductComponents,
                    subProduct.ProductQuestionTemplates, subProducts, treeItemData, icons);
                
                Console.WriteLine("Generating sub tree items2"); 
                treeItems.Add(treeItemData);
            }
        }
        if (productProductComponents is { Count: > 0 })
        {
            var componentIds = productProductComponents.Select(component => component.ComponentId).ToList();
            foreach (var id in componentIds)
            {
                var productComponent = productProductComponents.FirstOrDefault(c => c.ComponentId == id);
                if (productComponent == null)
                {
                    continue;
                }
                var treeItemData = new PlaceHolderTreeItemData(
                    productComponent: productComponent,
                    parent: parent,
                    icon: icons[PlaceHolderType.PRODUCT_COMPONENT],
                    isExpanded: false,
                    treeItems: new HashSet<PlaceHolderTreeItemData>());
                treeItems.Add(treeItemData);
            }
        }

        if (productProductQuestionTemplates is { Count: > 0 })
        {
            var questionTemplateIds = productProductQuestionTemplates
                .Select(questionTemplate => questionTemplate.QuestionTemplateId).ToList();
            foreach (var id in questionTemplateIds)
            {
                var productQuestionTemplate = productProductQuestionTemplates.FirstOrDefault(q => q.QuestionTemplateId == id);
                if (productQuestionTemplate == null)
                {
                    continue;
                }
                var treeItemData = new PlaceHolderTreeItemData(
                    productQuestionTemplate: productQuestionTemplate,
                    parent: parent,
                    icon: icons[PlaceHolderType.PRODUCT_QUESTION_TEMPLATE],
                    isExpanded: false,
                    treeItems: new HashSet<PlaceHolderTreeItemData>());
                treeItems.Add(treeItemData);
            }
        }
        Console.WriteLine("Sub tree items generated successfully count " + treeItems.Count );
        return treeItems;
    }
}
public class PlaceHolderTreeItemData
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    
    public string PlaceHolder { get; set; }
    public string Name { get; set; }
    public string Prefix { get; set; }
    public string Icon { get; set; }
    public PlaceHolderType Type { get; set; }
    public bool IsExpanded { get; set; }

    public PlaceHolderTreeItemData? Parent { get; set; }
    public HashSet<PlaceHolderTreeItemData> TreeItems { get; set; }
    public string? CodeAndName => $"{Code} - {Name}";
   
    public PlaceHolderTreeItemData(ProductDto product,
        PlaceHolderTreeItemData? parent, string icon, bool isExpanded = false,
        HashSet<PlaceHolderTreeItemData>? treeItems = null)
    {
        Id = product.Id;
        Type = PlaceHolderType.PRODUCT;
        Code = product.Code;
        Name = product.Name;
        Prefix = PlaceHolderType.PRODUCT.GetPrefix();
        PlaceHolder = product.PlaceHolder;
        Parent = parent;
        Icon = icon;
        IsExpanded = isExpanded;
        TreeItems = treeItems ?? [];
    }
    public PlaceHolderTreeItemData(ProductComponentDto productComponent,
        PlaceHolderTreeItemData? parent, string icon, bool isExpanded = false,
        HashSet<PlaceHolderTreeItemData>? treeItems = null)
    {
        Id = productComponent.Id;
        Type = PlaceHolderType.PRODUCT_COMPONENT;
        Code = productComponent.Code;
        PlaceHolder = productComponent.PlaceHolder;
        Name = productComponent.Name;
        Prefix = PlaceHolderType.PRODUCT_COMPONENT.GetPrefix();
        Parent = parent;
        Icon = icon;
        IsExpanded = isExpanded;
        TreeItems = treeItems ?? [];
    }
    public PlaceHolderTreeItemData(ProductQuestionTemplateDto productQuestionTemplate,
        PlaceHolderTreeItemData? parent, string icon, bool isExpanded = false,
        HashSet<PlaceHolderTreeItemData>? treeItems = null)
    {
        Id = productQuestionTemplate.Id;
        Type = PlaceHolderType.PRODUCT_COMPONENT;
        Code = productQuestionTemplate.Code;
        PlaceHolder = productQuestionTemplate.PlaceHolder;
        Name = productQuestionTemplate.Name;
        Prefix = PlaceHolderType.PRODUCT_COMPONENT.GetPrefix();
        Parent = parent;
        Icon = icon;
        IsExpanded = isExpanded;
        TreeItems = treeItems ?? [];
    }
}