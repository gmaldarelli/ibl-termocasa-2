using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IBLTermocasa.Blazor.Components.Product;
using IBLTermocasa.Permissions;
using IBLTermocasa.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.Http.Client;

namespace IBLTermocasa.Blazor.Pages.Crm;

public partial class ProductDetail
{
    [Inject]
    public IProductsAppService ProductService { get; set; }
    [Inject]
    public IAuthorizationService AuthorizationService { get; set; }
    
    
    [Parameter] public string? Id { get; set; }
    
    
    protected List<BreadcrumbItem> BreadcrumbItems = new List<BreadcrumbItem>();
    protected PageToolbar Toolbar { get; } = new PageToolbar();
    private bool CanCreateProduct { get; set; }
    private bool CanEditProduct { get; set; }
    private bool CanDeleteProduct { get; set; }
    private bool IsNew = true;
    private ProductDto Product { get; set; }
    
    public MudBlazor.Position Position { get; set; } = MudBlazor.Position.Left;
    public ProductInput ProductInputComponent { get; set; }


    protected override async Task OnInitializedAsync()
    {
        if(Id != null && Guid.TryParse(Id, out _) && Id != Guid.Empty.ToString())
        {
            IsNew = false;
            Guid id = Guid.Parse(Id);
            Product = await LoadProductAsync(id, true);
        }
        else
        {
            IsNew = true;
            Product = new ProductDto();
        }
        await SetBreadcrumbItemsAsync();
        await SetPermissionsAsync();
    }

    protected virtual async Task<ProductDto> LoadProductAsync(Guid id, bool includeDetails = false)
    {
        try
        {
            var result = await ProductService.GetAsync(id, includeDetails);
            return result;
        }
        catch (AbpRemoteCallException e)
        {
            await UiMessageService.Warn(L["ProductNotFound"]);
            NavigationManager.NavigateTo("/products");
        }
        return null;
    }   


    private async Task SetPermissionsAsync()
    {
        CanCreateProduct = await AuthorizationService
            .IsGrantedAsync(IBLTermocasaPermissions.Products.Create);
        CanEditProduct = await AuthorizationService
            .IsGrantedAsync(IBLTermocasaPermissions.Products.Edit);
        CanDeleteProduct = await AuthorizationService
            .IsGrantedAsync(IBLTermocasaPermissions.Products.Delete);
    }

    protected virtual ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:Products"], "/products")
        );
        if (Product != null)
        {
            if (IsNew)
            {
                BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:NewProduct"]));
            }
            else
            {
                BreadcrumbItems.Add(new BreadcrumbItem($"{L["Menu:Product"]} - {Product.Name} ", $"/product/{Product.Id}"));
            }
        }
        return ValueTask.CompletedTask;
    }


    private async void HandleProductSaved(ProductDto obj)
    {
        //TODO: Implement this method
    }

    private async void HandleProductCancel(ProductDto obj)
    {
        //TODO: Implement this method
    }
}