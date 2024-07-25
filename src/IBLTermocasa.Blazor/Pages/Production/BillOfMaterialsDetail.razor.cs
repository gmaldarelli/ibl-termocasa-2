using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IBLTermocasa.BillOfMaterials;
using IBLTermocasa.Blazor.Components.BillOfMaterial;
using IBLTermocasa.Permissions;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;

namespace IBLTermocasa.Blazor.Pages.Production;

public partial class BillOfMaterialsDetail
{
    [Parameter] public string? Id { get; set; }
    
    protected List<BreadcrumbItem> BreadcrumbItems = new List<BreadcrumbItem>();
    protected PageToolbar Toolbar { get; } = new PageToolbar();
    private bool CanEditBillOfMaterials { get; set; }
    private bool CanDeleteBillOfMaterials { get; set; }
    private BillOfMaterialDto BillOfMaterial { get; set; }
    public BillOfMaterialsInput BillOfMaterialsInputComponent { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if(Id != null && Guid.TryParse(Id, out _) && Id != Guid.Empty.ToString())
        {
            Guid id = Guid.Parse(Id);
            BillOfMaterial = await LoadBillOfMaterialAsync(id, true);
        }
        else
        {
            NavigationManager.NavigateTo("/bill-of-materials");
        }
        await SetBreadcrumbItemsAsync();
        await SetPermissionsAsync();
    }

    private async Task<BillOfMaterialDto> LoadBillOfMaterialAsync(Guid id, bool b)
    {
        if (Id != null)
        {
            return await BillOfMaterialsAppService.GetAsync(Guid.Parse(Id));
        }
        NavigationManager.NavigateTo("/bill-of-materials");
        return null;
    }
    
    private async Task SetPermissionsAsync()
    {
        CanEditBillOfMaterials = await AuthorizationService
            .IsGrantedAsync(IBLTermocasaPermissions.BillOfMaterials.Edit);
        CanDeleteBillOfMaterials = await AuthorizationService
            .IsGrantedAsync(IBLTermocasaPermissions.BillOfMaterials.Delete);
    }

    protected virtual ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:BillOfMaterials"], "/bill-of-materials"));
        BreadcrumbItems.Add(new BreadcrumbItem($"{L["Menu:BillOfMaterials"]} - {BillOfMaterial.BomNumber} ", $"/bill-of-materials-detail/{BillOfMaterial.Id}"));
        return ValueTask.CompletedTask;
    }
}