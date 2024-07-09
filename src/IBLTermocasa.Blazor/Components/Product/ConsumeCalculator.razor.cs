using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using IBLTermocasa.Components;
using IBLTermocasa.Products;
using IBLTermocasa.QuestionTemplates;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using NUglify.Helpers;

namespace IBLTermocasa.Blazor.Components.Product;

public partial class ConsumeCalculator
{
    [Inject]
    public IProductsAppService ProductService { get; set; }
    
    [Inject]
    public IQuestionTemplatesAppService QuestionTemplatesAppService { get; set; }
    
    [Inject]
    public IComponentsAppService ComponentsAppService { get; set; }
    
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    
    [Parameter] public ProductDto Product { get; set; }
    [Parameter] public Dictionary<string,string> Formulas { get; set; } 
    public HashSet<TreeItemData> TreeItems { get; set; }
    public TreeItemData RootItem { get; set; }
    public HashSet<TreeItemData> SelectedValues { get; set; }
    public MudTreeView<TreeItemData> ProductMudTreeView { get; set; }
    public ProductComponentDto SelectedProductComponent { get; set; }
    public bool dense { get; set; } = true;
    public bool open { get; set; } = true;
    public DrawerClipMode clipMode { get; set; } = DrawerClipMode.Never;
    public bool preserveOpenState { get; set; } = false;

    protected override async Task OnParametersSetAsync()
    {
        if(Product != null && Formulas != null)
        {
            Console.WriteLine("Product is not null proceeding to generate tree items");
            //await FIllFormulas();
            await GenerateTreeItems();
            ReadyToRendered = true;
            StateHasChanged();
        }
    }

    public bool ReadyToRendered { get; set; }

    private async Task FIllFormulas()
    {
        Product.ProductComponents.ForEach(component =>
        {
            Formulas.Add(component.Code, "");
        });
    }

    private async Task GenerateTreeItems()
    {
        TreeItems = new HashSet<TreeItemData>();
         var treeItemData =new TreeItemData(Guid.NewGuid(), 
            typeof(ProductDto), 
            Product.Code, 
            Product.Name, 
            prefix: "P",
            parent: null,
            icon: MudBlazor.Icons.Material.Filled.GifBox, 
            isExpanded: true, 
            treeItems: new HashSet<TreeItemData>());
         var children = await GenerateSubTreeItems(Product.SubProducts, Product.ProductComponents, Product.ProductQuestionTemplates, treeItemData);
         treeItemData.TreeItems = children;
        TreeItems.Add(treeItemData);
        RootItem = treeItemData;
    }

    private async Task<HashSet<TreeItemData>> GenerateSubTreeItems(List<SubProductDto> productSubProducts,
        List<ProductComponentDto> productProductComponents,
        List<ProductQuestionTemplateDto> productProductQuestionTemplates, TreeItemData? parent)
    {
        Console.WriteLine("Generating sub tree items"); 
        var _treeItems = new HashSet<TreeItemData>();
        if (productSubProducts is { Count: > 0 })
        {
            var subProductIds = productSubProducts.Select(subProduct => subProduct.ProductId).ToList();
            Console.WriteLine("Generating sub tree items1"); 
            foreach (var id in subProductIds)
            {
                Console.WriteLine($"Generating sub tree items1.2 {id}"); 
                var subProduct = await ProductService.GetAsync(id);
                if(subProduct == null)
                {
                    
                    Console.WriteLine($"Generating sub tree items1.2.1 {id} subProduct is null"); 
                    continue;
                }
                Console.WriteLine("Generating sub tree items1.3"); 
                var treeItemData = new TreeItemData(Guid.NewGuid(),
                    typeof(SubProductDto),
                    subProduct.Code,
                    subProduct.Name,
                    prefix: "P",
                    parent: parent ?? null,
                    icon: MudBlazor.Icons.Material.Filled.GifBox,
                    isExpanded: false,
                    treeItems: new HashSet<TreeItemData>());
                Console.WriteLine("Generating sub tree items1.4"); 
                treeItemData.TreeItems = await GenerateSubTreeItems(subProduct.SubProducts, subProduct.ProductComponents,
                    subProduct.ProductQuestionTemplates, treeItemData);
                
                Console.WriteLine("Generating sub tree items2"); 
                _treeItems.Add(treeItemData);
            }
        }

        if (productProductComponents is { Count: > 0 })
        {
            var componentIds = productProductComponents.Select(component => component.ComponentId).ToList();
            foreach (var id in componentIds)
            {
                //var component = await ComponentsAppService.GetAsync(id);
                var productComponent = productProductComponents.FirstOrDefault(c => c.ComponentId == id);
                var treeItemData = new TreeItemData(Guid.NewGuid(),
                    typeof(ProductComponentDto),
                    productComponent!.Code,
                    productComponent.Name,
                    prefix: "C",
                    parent: parent,
                    icon: MudBlazor.Icons.Material.Filled.Compost,
                    isExpanded: false,
                    treeItems: new HashSet<TreeItemData>());
                _treeItems.Add(treeItemData);
            }
        }

        if (productProductQuestionTemplates is { Count: > 0 })
        {
            var questionTemplateIds = productProductQuestionTemplates
                .Select(questionTemplate => questionTemplate.QuestionTemplateId).ToList();
            foreach (var id in questionTemplateIds)
            {
                //var questionTemplate = await QuestionTemplatesAppService.GetAsync(id);
                var productQuestionTemplate = productProductQuestionTemplates.FirstOrDefault(q => q.QuestionTemplateId == id);
                var treeItemData = new TreeItemData(Guid.NewGuid(),
                    typeof(ProductQuestionTemplateDto),
                    productQuestionTemplate!.Code,
                    name: productQuestionTemplate.Name,
                    prefix: "Q",
                    parent: parent,
                    icon: MudBlazor.Icons.Material.Filled.QuestionAnswer,
                    isExpanded: false,
                    treeItems: new HashSet<TreeItemData>());
                _treeItems.Add(treeItemData);
            }
        }
        Console.WriteLine("Sub tree items generated successfully count " + _treeItems.Count );
        return _treeItems;
    }

    void ToggleDrawer()
    {
        open = !open;
    }

    private Task OnSelectedFormula(ProductComponentDto item)
    {
        SelectedFormula = item;
        return Task.CompletedTask;
    }

    public ProductComponentDto? SelectedFormula { get; set; }

    private Task AddToSelectedFormula(TreeItemData item)
    {
        if(SelectedFormula != null)
        {
            string formula = $"{this.CodeSequence(item)}";
            formula = "{" + formula + "}";
            if (!Formulas[SelectedFormula.Code].IsNullOrEmpty())
            {
                formula = Formulas[SelectedFormula.Code]  + " " + formula;
            }
            Console.WriteLine("::::::::::::::::::::::::::::::::::::::Selected formula code: " + SelectedFormula.Code);
            Formulas[SelectedFormula.Code] = formula;
            StateHasChanged();
        }
        Console.WriteLine("::::::::::::::::::::::::::::::::::::::Selected formula is null");
        return Task.CompletedTask;
    }

    private string CodeSequence(TreeItemData item)
    {
        if (item.Parent != null)
        {
            return CodeSequence(item.Parent) + "." + item.Prefix + "[" + item.Code + "]";
        } else
        {
            return item.Prefix + "[" + item.Code + "]";
        }
    }

    private void Save(MouseEventArgs obj)
    {
        MudDialog.Close(DialogResult.Ok(Formulas));
    }
}
public class TreeItemData
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Prefix { get; set; }
    public string Icon { get; set; }
    public Type Type { get; set; }
    public bool IsExpanded { get; set; }
    
    public TreeItemData? Parent { get; set; }
    public HashSet<TreeItemData> TreeItems { get; set; }
    public string? CodeAndName => $"{Code} - {Name}";

    public TreeItemData(Guid id)
    {
        Id = id;
    }
    public TreeItemData(Guid id, Type type, string code, string name, string prefix,  TreeItemData? parent, string icon,  bool isExpanded = false, HashSet<TreeItemData>? treeItems = null)
    {
        Id = id;
        Type = type;
        Code = code;
        Name = name;
        Prefix = prefix;
        Parent = parent;
        Icon = icon;
        IsExpanded = isExpanded;
        TreeItems = treeItems ?? [];
    }
}