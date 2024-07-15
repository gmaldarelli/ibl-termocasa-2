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
using Volo.Abp.AspNetCore.Components.Messages;
using ChartOptions = Blazorise.Charts.ChartOptions;

namespace IBLTermocasa.Blazor.Components.BillOfMaterial;

public partial class BillOfMaterialsInput
{
    public MudForm formBillOfMaterials { get; set; }
    public bool successValidationBillOfMaterials { get; set; }
    public string[] errorsValidationBillOfMaterials { get; set; } = { };
    
    List<BillOfMaterialsMudDataGridItem> BillOfMaterialsMudDataGridItems { get; set; } = new List<BillOfMaterialsMudDataGridItem>();
    List<BillOfWorksMudDataGridItem> BillOfWorksMudDataGridItems { get; set; } = new List<BillOfWorksMudDataGridItem>();
    Dictionary<Guid,List<MaterialDto>> MaterialDictionary { get; set; } = new Dictionary<Guid, List<MaterialDto>>();
    public Position MudTabsPosition { get; set; } = Position.Top;
    [Parameter] public BillOfMaterialDto BillOfMaterials { get; set; }

    public MudDataGrid<BillOfMaterialsMudDataGridItem> BillOfMaterialsMudDataGrid { get; set; }
    public MudDataGrid<BillOfWorksMudDataGridItem> BillOfWorksMudDataGrid { get; set; }
    public MudChart MaterialsCostChart { get; set; }
    public MudChart WorksCostChart { get; set; }
    public double[] MaterialsCostData { get; set; } = new double[] { };
    public string[] MaterialsCostLabels { get; set; } = new string[] { };
    public double[] WorksCostData { get; set; } = new double[] { };
    public string[] WorksCostLabels { get; set; } = new string[] { };
    public ChartOptions MaterialsCostsChartOptions { get; set; }
    public ChartOptions WorksCostsChartOptions { get; set; }

    public MudBlazor.ChartOptions WorksCostChartOptions { get; set; } = new MudBlazor.ChartOptions()
    {

    };

    protected override async Task OnParametersSetAsync()
    {
        if(BillOfMaterials == null)
        {
            return;
        }
        this.LoadBillOfMaterialsMudDataGridItems();
        this.LoadBillOfWorksMudDataGridItems();
        await  base.OnParametersSetAsync();
    }

    private void LoadBillOfWorksMudDataGridItems()
    {
        
        List<BillOfWorksMudDataGridItem> items = new List<BillOfWorksMudDataGridItem>();
        foreach (var listItem in BillOfMaterials.ListItems)
        {
            foreach (var bomProductItem in listItem.BomProductItems)
            {
                foreach (var bowItem in bomProductItem.BowItems)
                {
                    var item = new BillOfWorksMudDataGridItem
                    {
                        RequestForQuotationItemId = listItem.RequestForQuotationItemId,
                        RequestForQuotationItemQuantity = listItem.Quantity,
                        BomProductItemId = bomProductItem.Id,
                        ProductItemId = bomProductItem.ProductItemId,
                        ProductId = bomProductItem.ProductId,
                        ProductItemName = bomProductItem.ProductItemName,
                        ProductItemProductId = bomProductItem.ProductItemId,
                        ParentBOMProductItemId = bomProductItem.ParentBOMProductItemId,
                        Code = bowItem.Code,
                        Name = bowItem.Name,
                        WorkTime = bowItem.WorkTime,
                        IdProfessionalProfile = bowItem.IdProfessionalProfile,
                        Price = bowItem.Price,
                        HourPrice = bowItem.HourPrice
                    };
                    Console.WriteLine(":::::::::::::::::::::::::::::::BillOfWorksMudDataGridItem: " + item.ToString());
                    Console.WriteLine($"::::::::::::::::::::::::::::::::bomProductItem.ProductItemName: {bomProductItem.ProductItemName} bomComponent.ComponentName: {bowItem.Name}");

                    items.Add(item);
                }
            }


        }
        BillOfWorksMudDataGridItems = items;
        // devo riempire WorksCostData e  WorksCostLabels per mostrare il grafico dei costi del lavoro WorksCostChart raggruppando per Name
        var worksCostData = BillOfWorksMudDataGridItems.GroupBy(x => x.Name).Select(x => x.Sum(y => y.Price)).ToArray();
        var worksCostLabels = BillOfWorksMudDataGridItems.GroupBy(x => x.Name).Select(x => x.Key).ToArray();
        WorksCostData = worksCostData;
        WorksCostLabels = worksCostLabels;
        StateHasChanged();
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
                    var item = new BillOfMaterialsMudDataGridItem
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
                    };
                    items.Add(item);
                    Console.WriteLine(":::::::::::::::::::::::::::::::BillOfMaterialsMudDataGridItem: " + item.ToString());
                    Console.WriteLine($"::::::::::::::::::::::::::::::::bomProductItem.ProductItemName: {bomProductItem.ProductItemName} bomComponent.ComponentName: {bomComponent.ComponentName}");
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
                if (bomi.Price is 0 && bomi.Quantity is not 0)
                {
                    bomi.Price = bomi.RequestForQuotationItemQuantity * bomi.Quantity * bomi.MaterialPrice;
                }
            }
        }
        BillOfMaterialsMudDataGridItems = items;
        // devo riempire MaterialsCostData e  MaterialsCostLabels per mostrare il grafico dei costi dei materiali MaterialsCostChart raggruppando per MaterialName
        var materialsCostData = BillOfMaterialsMudDataGridItems.GroupBy(x => x.MaterialName).Select(x => x.Sum(y => Convert.ToDouble(y.Price))).ToArray();
        var materialsCostLabels = BillOfMaterialsMudDataGridItems.GroupBy(x => x.MaterialName).Select(x => x.Key).ToArray();
        MaterialsCostData = materialsCostData;
        MaterialsCostLabels = materialsCostLabels;
        Console.WriteLine("::::::::::::::::::::::::::::::::MaterialsCostData: " + string.Join(",", MaterialsCostData));
        Console.WriteLine("::::::::::::::::::::::::::::::::MaterialsCostLabels: " + string.Join(",", MaterialsCostLabels));
        StateHasChanged();
        Console.WriteLine("::::::::::::::::::::::::::::::::MaterialDictionaryKeys.Count: " + MaterialDictionary.Keys.Count);
    }
    
    private string GroupMaterialsClassFunc(GroupDefinition<BillOfMaterialsMudDataGridItem> item)
    {
        return item.Grouping.Key?.ToString() == "Nonmetal" || item.Grouping.Key?.ToString() == "Other"
            ? "mud-theme-warning"
            : string.Empty;
    }
    
    private string GroupWorksClassFunc(GroupDefinition<BillOfWorksMudDataGridItem> item)
    {
        return item.Grouping.Key?.ToString() == "Nonmetal" || item.Grouping.Key?.ToString() == "Other"
            ? "mud-theme-warning"
            : string.Empty;
    }
    private void CommittedItemChanges(BillOfMaterialsMudDataGridItem contextItem)
    {
        Console.WriteLine("::::::::::::::::::::::::::::::::CommittedItemChanges");
        contextItem.MaterialPrice = MaterialDictionary[contextItem.BomComponentId].FirstOrDefault(x => x.Id == contextItem.MaterialId)!.StandardPrice;
        contextItem.MeasureUnit = MaterialDictionary[contextItem.BomComponentId].FirstOrDefault(x => x.Id == contextItem.MaterialId)!.MeasureUnit;
        contextItem.Price = contextItem.RequestForQuotationItemQuantity * contextItem.Quantity * contextItem.MaterialPrice;
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

    private async Task OnCalculateConsumption(MouseEventArgs obj)
    {
        ReloadItemsFromDataGrid();
        BillOfMaterials.ListItems = await BillOfMaterialsAppService.CalculateConsumption(BillOfMaterials.Id, BillOfMaterials.ListItems);
        this.LoadBillOfMaterialsMudDataGridItems();
        this.LoadBillOfWorksMudDataGridItems();
        //this.ReloadItemsFromDataGrid();        
        BillOfMaterialsMudDataGrid.ReloadServerData();
        StateHasChanged();
        
    }

    private void ReloadItemsFromDataGrid()
    {
        if (BillOfMaterials.ListItems != null)
        {
            BillOfMaterials.ListItems.ForEach(x =>
                {
                    x.BomProductItems.ForEach(y =>
                        {
                            y.BomComponents.ForEach(z =>
                                {
                                    z.MaterialId = BillOfMaterialsMudDataGridItems.FirstOrDefault(a =>
                                        a.BomComponentId == z.ComponentId &&
                                        a.ProductItemId == y.ProductItemId
                                    )!.MaterialId;
                                    z.Quantity = BillOfMaterialsMudDataGridItems.FirstOrDefault(a =>
                                        a.BomComponentId == z.ComponentId &&
                                        a.ProductItemId == y.ProductItemId
                                    )!.Quantity;
                                    z.Price = BillOfMaterialsMudDataGridItems.FirstOrDefault(a =>
                                        a.BomComponentId == z.ComponentId &&
                                        a.ProductItemId == y.ProductItemId
                                    )!.Price;
                                }
                            );
                        }
                    );
                }
            );
        }
    }

    private async Task OnSaveCalculateConsumption(MouseEventArgs obj)
    {
        ReloadItemsFromDataGrid();
        var message = L["BillOfMaterialSaveMaterialMessage"];
        if (!await UiMessageService.Confirm(message))
        {
            return;
        }
        else
        {
            if (BillOfMaterials.Status == BomStatusType.CREATED)
            {
                BillOfMaterials.Status = BomStatusType.MATERIALS_BILLED;
            }
            {
                
            }
            var updateDto = ObjectMapper.Map<BillOfMaterialDto, BillOfMaterialUpdateDto>(BillOfMaterials);
            BillOfMaterials = await BillOfMaterialsAppService.UpdateAsync(BillOfMaterials.Id, updateDto);
            await BillOfMaterialsMudDataGrid.ReloadServerData();
            StateHasChanged();
        }
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

    public override string ToString()
    {
        //Stampa in ToStrin tutti gli attributi della classe
        return $"RequestForQuotationItemId: {RequestForQuotationItemId} RequestForQuotationItemQuantity: {RequestForQuotationItemQuantity} BomProductItemId: {BomProductItemId} ProductItemId: {ProductItemId} ProductItemName: {ProductItemName} ProductItemProductId: {ProductItemProductId} ParentBOMProductItemId: {ParentBOMProductItemId} BomComponentId: {BomComponentId} BomComponentName: {BomComponentName} MaterialId: {MaterialId} MaterialName: {MaterialName} MaterialPrice: {MaterialPrice} Quantity: {Quantity} MeasureUnit: {MeasureUnit} Price: {Price}";
    
    }
}

public class BillOfWorksMudDataGridItem
{
    public Guid RequestForQuotationItemId { get; set; }
    public int RequestForQuotationItemQuantity { get; set; }
    public Guid BomProductItemId { get; set; }
    public Guid ProductItemId { get; set; }
    public string ProductItemName { get; set; }
    public Guid ProductItemProductId { get; set; }
    public Guid? ParentBOMProductItemId { get; set; }
    public Guid IdProfessionalProfile { get; set; }
    public Guid ProductId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public string? ConsumptionWorkFormula { get; set; }
    public int WorkTime { get; set; }
    public virtual double HourPrice { get; set; }

    public override string ToString()
    {
        //Stampa in ToStrin tutti gli attributi della classe
        return $"RequestForQuotationItemId: {RequestForQuotationItemId} RequestForQuotationItemQuantity: {RequestForQuotationItemQuantity} BomProductItemId: {BomProductItemId} ProductItemId: {ProductItemId} ProductItemName: {ProductItemName} ProductItemProductId: {ProductItemProductId} ParentBOMProductItemId: {ParentBOMProductItemId} IdProfessionalProfile: {IdProfessionalProfile} ProductId: {ProductId} Code: {Code} Name: {Name} Price: {Price} ConsumptionWorkFormula: {ConsumptionWorkFormula} WorkTime: {WorkTime} HourPrice: {HourPrice}";
    }
}