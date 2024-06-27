using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IBLTermocasa.BillOfMaterials;
using IBLTermocasa.Materials;
using IBLTermocasa.Types;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace IBLTermocasa.Blazor.Components.BillOfMaterial;

public partial class BillOfMaterialsInput
{
    public MudForm formBillOfMaterials { get; set; }
    public bool successValidationBillOfMaterials { get; set; }
    public string[] errorsValidationBillOfMaterials { get; set; } = { };
    
    List<BillOfMaterialsMudDataGridItem> BillOfMaterialsMudDataGridItems { get; set; } = new List<BillOfMaterialsMudDataGridItem>();
    Dictionary<Guid,List<MaterialDto>> MaterialDictionary { get; set; } = new Dictionary<Guid, List<MaterialDto>>();
    public Position MudTabsPosition { get; set; } = Position.Left;
    [Parameter] public BillOfMaterialDto BillOfMaterials { get; set; }

    public MudDataGrid<BillOfMaterialsMudDataGridItem> BillOfMaterialsMudDataGrid { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        if(BillOfMaterials == null)
        {
            return;
        }
        this.LoadBillOfMaterialsMudDataGridItems();
        await  base.OnParametersSetAsync();
    }

    private async void LoadBillOfMaterialsMudDataGridItems()
    {
        List<BillOfMaterialsMudDataGridItem> items = new List<BillOfMaterialsMudDataGridItem>();
        foreach (var listItem in BillOfMaterials.ListItems)
        {
            foreach (var bomProductItem in listItem.BomProductItems)
            {
                foreach (var bomComponent in bomProductItem.BomComponents)
                {
                    items.Add(new BillOfMaterialsMudDataGridItem
                    {
                        RequestForQuotationItemId = listItem.RequestForQuotationItemId,
                        RequestForQuotationItemQuantity = listItem.Quantity,
                        BomProductItemId = bomProductItem.Id,
                        ProductItemId = bomProductItem.ProductItemId,
                        ProductItemName = bomProductItem.ProductItemName,
                        ProductItemProductId = bomProductItem.ProductItemId,
                        ParentBOMProductItemId = bomProductItem.ParentBOMProductItemId,
                        BomComponentId = bomComponent.ComponentId,
                        BomComponentName = bomComponent.ComponentName,
                        MaterialId = bomComponent.MaterialId,
                        MaterialName = bomComponent.MaterialName,
                        MaterialPrice = bomComponent.Price,
                        Quantity = bomComponent.Quantity,
                        MeasureUnit = bomComponent.MeasureUnit,
                        Price = bomComponent.Price
                    });
                }
            }
        }
        List<Guid> componentIds = items.Select(x => x.BomComponentId).Distinct().ToList();
        MaterialDictionary = await ComponentsAppService.GetMaterialDictionaryAsync(componentIds);
        foreach (var bomi in items)
        {
            if(bomi.MaterialId != Guid.Empty)
            {
                bomi.MaterialPrice = MaterialDictionary[bomi.BomComponentId].FirstOrDefault(x => x.Id == bomi.MaterialId)!.StandardPrice;
                bomi.MeasureUnit = MaterialDictionary[bomi.BomComponentId].FirstOrDefault(x => x.Id == bomi.MaterialId)!.MeasureUnit;
            }
        }
        
        
        BillOfMaterialsMudDataGridItems = items;
        StateHasChanged();
        Console.WriteLine("::::::::::::::::::::::::::::::::MaterialDictionaryKeys.Count: " + MaterialDictionary.Keys.Count);
    }
    
    private string GroupClassFunc(GroupDefinition<BillOfMaterialsMudDataGridItem> item)
    {
        return item.Grouping.Key?.ToString() == "Nonmetal" || item.Grouping.Key?.ToString() == "Other"
            ? "mud-theme-warning"
            : string.Empty;
    }

    private async void OnMaterialIdChanged1(BillOfMaterialsMudDataGridItem obj)
    {
        obj.MaterialPrice = MaterialDictionary[obj.BomComponentId].FirstOrDefault(x => x.Id == obj.MaterialId)!.StandardPrice;
        obj.MeasureUnit = MaterialDictionary[obj.BomComponentId].FirstOrDefault(x => x.Id == obj.MaterialId)!.MeasureUnit;
        obj.Price = obj.Quantity * obj.MaterialPrice;
        StateHasChanged();
    }

    private Task OnMaterialChanged(BillOfMaterialsMudDataGridItem contextItem)
    {
        contextItem.MaterialPrice = MaterialDictionary[contextItem.BomComponentId].FirstOrDefault(x => x.Id == contextItem.MaterialId)!.StandardPrice;
        contextItem.MeasureUnit = MaterialDictionary[contextItem.BomComponentId].FirstOrDefault(x => x.Id == contextItem.MaterialId)!.MeasureUnit;
        contextItem.Price = contextItem.Quantity * contextItem.MaterialPrice;
        return Task.CompletedTask;
    }


    private void CommittedItemChanges(BillOfMaterialsMudDataGridItem contextItem)
    {
        Console.WriteLine("::::::::::::::::::::::::::::::::CommittedItemChanges");
        contextItem.MaterialPrice = MaterialDictionary[contextItem.BomComponentId].FirstOrDefault(x => x.Id == contextItem.MaterialId)!.StandardPrice;
        contextItem.MeasureUnit = MaterialDictionary[contextItem.BomComponentId].FirstOrDefault(x => x.Id == contextItem.MaterialId)!.MeasureUnit;
        contextItem.Price = contextItem.Quantity * contextItem.MaterialPrice;
        foreach (var billOfMaterialsMudDataGridItem in BillOfMaterialsMudDataGridItems)
        {
            if(billOfMaterialsMudDataGridItem.BomComponentId == contextItem.BomComponentId)
            {
                billOfMaterialsMudDataGridItem.MaterialId = contextItem.MaterialId;
                billOfMaterialsMudDataGridItem.MaterialName = contextItem.MaterialName;
                billOfMaterialsMudDataGridItem.MaterialPrice = contextItem.MaterialPrice;
                billOfMaterialsMudDataGridItem.MeasureUnit = contextItem.MeasureUnit;
                billOfMaterialsMudDataGridItem.Quantity = contextItem.Quantity;
                billOfMaterialsMudDataGridItem.Price = contextItem.Price;
            }
        }

        BillOfMaterialsMudDataGrid.ReloadServerData();
        StateHasChanged();
    }

    private void OnCalculateConsumption(MouseEventArgs obj)
    {
        Console.WriteLine("::::::::::::::::::::::::::::::::Calculate consumption");
        
    }

    private Task OnMaterialChanged2(BillOfMaterialsMudDataGridItem contextItem)
    {
        contextItem.MaterialPrice = MaterialDictionary[contextItem.BomComponentId].FirstOrDefault(x => x.Id == contextItem.MaterialId)!.StandardPrice;
        contextItem.MeasureUnit = MaterialDictionary[contextItem.BomComponentId].FirstOrDefault(x => x.Id == contextItem.MaterialId)!.MeasureUnit;
        contextItem.Price = contextItem.Quantity * contextItem.MaterialPrice;
        return Task.CompletedTask;
    }

    private void OnMaterialChanged3(BillOfMaterialsMudDataGridItem contextItem)
    {
        contextItem.MaterialPrice = MaterialDictionary[contextItem.BomComponentId].FirstOrDefault(x => x.Id == contextItem.MaterialId)!.StandardPrice;
        contextItem.MeasureUnit = MaterialDictionary[contextItem.BomComponentId].FirstOrDefault(x => x.Id == contextItem.MaterialId)!.MeasureUnit;
        contextItem.Price = contextItem.Quantity * contextItem.MaterialPrice;
    }
}

public class BillOfMaterialsMudDataGridItem
{
    public Guid RequestForQuotationItemId { get; set; }
    public int RequestForQuotationItemQuantity { get; set; }
    public Guid BomProductItemId { get; set; }
    public Guid ProductItemId { get; set; }
    public string ProductItemName { get; set; }
    public Guid ProductItemProductId { get; set; }
    public Guid? ParentBOMProductItemId { get; set; }
    public Guid BomComponentId { get; set; }
    public string BomComponentName { get; set; }
    public Guid MaterialId { get; set; }
    public string MaterialName { get; set; }
    public decimal MaterialPrice { get; set; }
    public decimal Quantity { get; set; }
    public MeasureUnit MeasureUnit { get; set; }
    public decimal Price { get; set; }
}