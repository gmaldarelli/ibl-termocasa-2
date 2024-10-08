using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Blazorise;
using Blazorise.DataGrid;
using IBLTermocasa.Permissions;
using IBLTermocasa.Products;
using IBLTermocasa.Shared;
using Microsoft.AspNetCore.Authorization;
using MudBlazor;
using NUglify.Helpers;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;
using SortDirection = Blazorise.SortDirection;


namespace IBLTermocasa.Blazor.Pages.Inventory;

public partial class Products
{
    protected List<BreadcrumbItem> BreadcrumbItems = new();

    protected PageToolbar Toolbar { get; } = new();
    private IReadOnlyList<ProductDto> ProductList { get; set; }
    private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
    private int CurrentPage { get; set; } = 1;
    private string CurrentSorting { get; set; } = string.Empty;
    private int TotalCount { get; set; }
    private bool CanCreateProduct { get; set; }
    private bool CanEditProduct { get; set; }
    private bool CanDeleteProduct { get; set; }
    private ProductCreateDto NewProduct { get; set; }
    private ProductUpdateDto EditingProduct { get; set; }
    private GetProductsInput Filter { get; set; }
    private ProductDto? SelectedProduct;
    private string _searchString;
    private IReadOnlyList<LookupDto<Guid>> Components { get; set; } = new List<LookupDto<Guid>>();
    private IReadOnlyList<LookupDto<Guid>> QuestionTemplates { get; set; } = new List<LookupDto<Guid>>();

    private MudDataGrid<ProductDto> ProductMudDataGrid { get; set; }

    private List<ProductDto> SelectedProducts { get; set; } = new();
    private bool AllProductsSelected { get; set; }

    public Products()
    {
        NewProduct = new ProductCreateDto();
        EditingProduct = new ProductUpdateDto();
        Filter = new GetProductsInput
        {
            MaxResultCount = PageSize,
            SkipCount = (CurrentPage - 1) * PageSize,
            Sorting = CurrentSorting
        };
        ProductList = new List<ProductDto>();
    }

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        await GetComponentLookupAsync();
        await GetQuestionTemplateLookupAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await SetBreadcrumbItemsAsync();
            StateHasChanged();
        }
    }

    protected virtual ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:Products"]));
        return ValueTask.CompletedTask;
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

    private async Task GetProductsAsync()
    {
        Filter.MaxResultCount = PageSize;
        Filter.SkipCount = (CurrentPage - 1) * PageSize;
        Filter.Sorting = CurrentSorting;

        var result = await ProductsAppService.GetListAsync(Filter);
        ProductList = result.Items;
        TotalCount = (int)result.TotalCount;

        await ClearSelection();
    }

    private async Task<GridData<ProductDto>> LoadGridData(GridState<ProductDto> state)
    {
        state.SortDefinitions.ForEach(sortDef =>
        {
            if (sortDef.Descending)
            {
                CurrentSorting = $" {sortDef.SortBy} DESC";
            }
            else
            {
                CurrentSorting = $" {sortDef.SortBy} ";
            }
        });
        Filter.SkipCount = state.Page * state.PageSize;
        Filter.Sorting = CurrentSorting;
        Filter.MaxResultCount = state.PageSize;
        Filter.FilterText = _searchString;
        var firstOrDefault =
            ProductMudDataGrid.FilterDefinitions.FirstOrDefault(x => x.Column is
                { PropertyName: nameof(ProductDto.IsAssembled) });
        if (firstOrDefault != null)
        {
            Filter.IsAssembled = (bool?)firstOrDefault.Value;
        }

        var firstOrDefault1 =
            ProductMudDataGrid.FilterDefinitions.FirstOrDefault(x => x.Column is
                { PropertyName: nameof(ProductDto.IsInternal) });
        if (firstOrDefault1 != null)
        {
            Filter.IsInternal = (bool?)firstOrDefault1.Value;
        }

        var firstOrDefault2 =
            ProductMudDataGrid.FilterDefinitions.FirstOrDefault(x => x.Column is
                { PropertyName: nameof(ProductDto.Code) });
        if (firstOrDefault2 != null)
        {
            Filter.Code = (string)firstOrDefault2.Value!;
        }

        var firstOrDefault3 =
            ProductMudDataGrid.FilterDefinitions.FirstOrDefault(x => x.Column is
                { PropertyName: nameof(ProductDto.Name) });
        if (firstOrDefault3 != null)
        {
            Filter.Name = (string)firstOrDefault3.Value!;
        }

        var result = await ProductsAppService.GetListAsync(Filter);
        ProductList = result.Items;
        GridData<ProductDto> data = new()
        {
            Items = ProductList,
            TotalItems = (int)result.TotalCount
        };
        return data;
    }

    private async Task DownloadAsExcelAsync()
    {
        var token = (await ProductsAppService.GetDownloadTokenAsync()).Token;
        var remoteService =
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("IBLTermocasa") ??
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
        if (!culture.IsNullOrEmpty())
        {
            culture = "&culture=" + culture;
        }

        await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
        NavigationManager.NavigateTo(
            $"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/products/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Code={HttpUtility.UrlEncode(Filter.Code)}&Name={HttpUtility.UrlEncode(Filter.Name)}&Description={HttpUtility.UrlEncode(Filter.Description)}&IsAssembled={Filter.IsAssembled}&IsInternal={Filter.IsInternal}",
            forceLoad: true);
    }

    private async Task OpenCreateProductPageAsync()
    {
        NavigationManager.NavigateTo($"/product/new-product");
    }

    private async Task OpenEditProductAsync(ProductDto input)
    {
        NavigationManager.NavigateTo($"/product/{input.Id}");
    }

    private async Task DeleteProductAsync(ProductDto input)
    {
        await ProductsAppService.DeleteAsync(input.Id);
        await GetProductsAsync();
        await ProductMudDataGrid.ReloadServerData();
    }

    private async Task GetComponentLookupAsync(string? newValue = null)
    {
        Components = (await ProductsAppService.GetComponentLookupAsync(new LookupRequestDto { Filter = newValue }))
            .Items;
    }

    private async Task GetQuestionTemplateLookupAsync(string? newValue = null)
    {
        QuestionTemplates =
            (await ProductsAppService.GetQuestionTemplateLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private Task ClearSelection()
    {
        AllProductsSelected = false;
        SelectedProducts.Clear();

        return Task.CompletedTask;
    }

    private async void SearchAsync(string filterText)
    {
        _searchString = filterText;
        if ((_searchString.IsNullOrEmpty() || _searchString.Length < 3) &&
            (ProductMudDataGrid.Items != null && ProductMudDataGrid.Items.Any()))
        {
            return;
        }

        await LoadGridData(new GridState<ProductDto>
        {
            Page = 0,
            PageSize = PageSize,
            SortDefinitions = ProductMudDataGrid.SortDefinitions.Values.ToList()
        });
        await ProductMudDataGrid.ReloadServerData();
        StateHasChanged();
    }
}