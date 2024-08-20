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
        this.ApplyParentToSubProducts(rootProduct, subProducts);
        this.ApplyParentToComponentsAndQuestionTemplate(rootProduct);
        var treeItemData = new PlaceHolderTreeItemData(
            product: rootProduct,
            parent: null,
            icon: icons[PlaceHolderType.PRODUCT],
            isExpanded: true,
            treeItems: new HashSet<PlaceHolderTreeItemData>());
        var children = GenerateSubTreeItems(rootProduct.SubProducts, rootProduct.ProductComponents,
            rootProduct.ProductQuestionTemplates, subProducts, treeItemData, icons);
        treeItemData.TreeItems = children;
        return treeItemData;
    }

    private void ApplyParentToComponentsAndQuestionTemplate(ProductDto rootProduct)
    {
        foreach (var productComponent in rootProduct.ProductComponents)
        {
            productComponent.ParentPlaceHolder = rootProduct.PlaceHolder;
        }

        foreach (var productQuestionTemplate in rootProduct.ProductQuestionTemplates)
        {
            productQuestionTemplate.ParentPlaceHolder = rootProduct.PlaceHolder;
        }
    }

    private void ApplyParentToSubProducts(ProductDto rootProduct, List<ProductDto> subProducts)
    {
        foreach (var subProduct in subProducts)
        {
            subProduct.ParentPlaceHolder = rootProduct.PlaceHolder;
            foreach (var productComponent in subProduct.ProductComponents)
            {
                productComponent.ParentPlaceHolder = subProduct.PlaceHolder;
            }

            foreach (var productQuestionTemplate in subProduct.ProductQuestionTemplates)
            {
                productQuestionTemplate.ParentPlaceHolder = subProduct.PlaceHolder;
            }
        }
    }

    private HashSet<PlaceHolderTreeItemData> GenerateSubTreeItems(
        List<SubProductDto> productSubProducts,
        List<ProductComponentDto> productProductComponents,
        List<ProductQuestionTemplateDto> productProductQuestionTemplates,
        List<ProductDto> subProducts,
        PlaceHolderTreeItemData? parent, Dictionary<PlaceHolderType, string> icons)
    {
        Console.WriteLine("Generating sub tree items");

        var subProductDict = subProducts.ToDictionary(p => p.Id);
        var treeItems = new HashSet<PlaceHolderTreeItemData>();

        if (productSubProducts.Any())
        {
            foreach (var subProduct in productSubProducts)
            {
                if (subProductDict.TryGetValue(subProduct.ProductId, out var product))
                {
                    var treeItemData = new PlaceHolderTreeItemData(
                        product: product,
                        parent: parent,
                        icon: icons[PlaceHolderType.PRODUCT],
                        isExpanded: false,
                        treeItems: new HashSet<PlaceHolderTreeItemData>());

                    Console.WriteLine("Generating sub tree items1.4");
                    treeItemData.TreeItems = GenerateSubTreeItems(
                        product.SubProducts, product.ProductComponents, product.ProductQuestionTemplates,
                        subProducts, treeItemData, icons);

                    Console.WriteLine("Generating sub tree items2");
                    treeItems.Add(treeItemData);
                }
            }
        }

        if (productProductComponents.Any())
        {
            foreach (var component in productProductComponents)
            {
                var treeItemData = new PlaceHolderTreeItemData(
                    productComponent: component,
                    parent: parent,
                    icon: icons[PlaceHolderType.PRODUCT_COMPONENT],
                    isExpanded: false,
                    treeItems: new HashSet<PlaceHolderTreeItemData>());
                treeItems.Add(treeItemData);
            }
        }

        if (productProductQuestionTemplates.Any())
        {
            foreach (var questionTemplate in productProductQuestionTemplates)
            {
                var treeItemData = new PlaceHolderTreeItemData(
                    productQuestionTemplate: questionTemplate,
                    parent: parent,
                    icon: icons[PlaceHolderType.PRODUCT_QUESTION_TEMPLATE],
                    isExpanded: false,
                    treeItems: new HashSet<PlaceHolderTreeItemData>());
                treeItems.Add(treeItemData);
            }
        }

        Console.WriteLine("Sub tree items generated successfully count " + treeItems.Count);
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