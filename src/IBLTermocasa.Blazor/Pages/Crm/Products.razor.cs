using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Web;
using Blazorise;
using Blazorise.DataGrid;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using IBLTermocasa.Products;
using IBLTermocasa.Permissions;
using IBLTermocasa.Shared;
using MudBlazor;
using NUglify.Helpers;
using SortDirection = Blazorise.SortDirection;


namespace IBLTermocasa.Blazor.Pages.Crm;

public partial class Products
{
    protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems =
        new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();

    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected bool ShowAdvancedFilters { get; set; }
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
    private Modal CreateProductModal { get; set; } = new();
    private GetProductsInput Filter { get; set; }
    private DataGridEntityActionsColumn<ProductDto> EntityActionsColumn { get; set; } = new();
    protected string SelectedCreateTab = "product-create-tab";
    protected string SelectedEditTab = "product-edit-tab";
    private ProductDto? SelectedProduct;
    private string _searchString;
    private IReadOnlyList<LookupDto<Guid>> Components { get; set; } = new List<LookupDto<Guid>>();

    private string SelectedComponentId { get; set; }

    private string SelectedComponentText { get; set; }

    private List<LookupDto<Guid>> SelectedComponents { get; set; } = new List<LookupDto<Guid>>();
    private IReadOnlyList<LookupDto<Guid>> QuestionTemplates { get; set; } = new List<LookupDto<Guid>>();

    private string SelectedQuestionTemplateId { get; set; }

    private string SelectedQuestionTemplateText { get; set; }

    private List<LookupDto<Guid>> SelectedQuestionTemplates { get; set; } = new List<LookupDto<Guid>>();

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
            await SetToolbarItemsAsync();
            StateHasChanged();
        }
    }

    protected virtual ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Products"]));
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["ExportToExcel"], async () => { await DownloadAsExcelAsync(); }, IconName.Download);

        Toolbar.AddButton(L["NewProduct"], async () => { await OpenCreateProductPageAsync(); }, IconName.Add,
            requiredPolicyName: IBLTermocasaPermissions.Products.Create);

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
        var firstOrDefault = ProductMudDataGrid.FilterDefinitions.FirstOrDefault(x => x.Column is { PropertyName: nameof(ProductDto.IsAssembled) });
        if (firstOrDefault != null)
        {
            Filter.IsAssembled = (bool?)firstOrDefault.Value;
        }
        var firstOrDefault1 = ProductMudDataGrid.FilterDefinitions.FirstOrDefault(x => x.Column is { PropertyName: nameof(ProductDto.IsInternal) });
        if (firstOrDefault1 != null)
        {
            Filter.IsInternal = (bool?)firstOrDefault1.Value;
        }
        var firstOrDefault2 = ProductMudDataGrid.FilterDefinitions.FirstOrDefault(x => x.Column is { PropertyName: nameof(ProductDto.Code) });
        if (firstOrDefault2 != null)
        {
            Filter.Code = (string)firstOrDefault2.Value!;
        }
        var firstOrDefault3 = ProductMudDataGrid.FilterDefinitions.FirstOrDefault(x => x.Column is { PropertyName: nameof(ProductDto.Name) });
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





protected virtual async Task SearchAsync()
    {
        CurrentPage = 1;
        await GetProductsAsync();
        await InvokeAsync(StateHasChanged);
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

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<ProductDto> e)
    {
        CurrentSorting = e.Columns
            .Where(c => c.SortDirection != SortDirection.Default)
            .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
            .JoinAsString(",");
        CurrentPage = e.Page;
        await GetProductsAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task OpenCreateProductPageAsync()
    {
        NavigationManager.NavigateTo($"/product/new-product");
    }

    private async Task CloseCreateProductModalAsync()
    {
        NewProduct = new ProductCreateDto
        {
        };
        await CreateProductModal.Hide();
    }

    private async Task OpenEditProductAsync(ProductDto input)
    {
        NavigationManager.NavigateTo($"/product/{input.Id}");
    }

    private async Task DeleteProductAsync(ProductDto input)
    {
        await ProductsAppService.DeleteAsync(input.Id);
        await GetProductsAsync();
    }

    protected virtual async Task OnCodeChangedAsync(string? code)
    {
        Filter.Code = code;
        await SearchAsync();
    }

    protected virtual async Task OnNameChangedAsync(string? name)
    {
        Filter.Name = name;
        await SearchAsync();
    }

    protected virtual async Task OnDescriptionChangedAsync(string? description)
    {
        Filter.Description = description;
        await SearchAsync();
    }

    protected virtual async Task OnIsAssembledChangedAsync(bool? isAssembled)
    {
        Filter.IsAssembled = isAssembled;
        await SearchAsync();
    }

    protected virtual async Task OnIsInternalChangedAsync(bool? isInternal)
    {
        Filter.IsInternal = isInternal;
        await SearchAsync();
    }


    private async Task GetComponentLookupAsync(string? newValue = null)
    {
        Components = (await ProductsAppService.GetComponentLookupAsync(new LookupRequestDto { Filter = newValue }))
            .Items;
    }

    private void AddComponent()
    {
        if (SelectedComponentId.IsNullOrEmpty())
        {
            return;
        }

        if (SelectedComponents.Any(p => p.Id.ToString() == SelectedComponentId))
        {
            UiMessageService.Warn(L["ItemAlreadyAdded"]);
            return;
        }

        SelectedComponents.Add(new LookupDto<Guid>
        {
            Id = Guid.Parse(SelectedComponentId),
            DisplayName = SelectedComponentText
        });
    }

    private async Task GetQuestionTemplateLookupAsync(string? newValue = null)
    {
        QuestionTemplates =
            (await ProductsAppService.GetQuestionTemplateLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
    }

    private void AddQuestionTemplate()
    {
        if (SelectedQuestionTemplateId.IsNullOrEmpty())
        {
            return;
        }

        if (SelectedQuestionTemplates.Any(p => p.Id.ToString() == SelectedQuestionTemplateId))
        {
            UiMessageService.Warn(L["ItemAlreadyAdded"]);
            return;
        }

        SelectedQuestionTemplates.Add(new LookupDto<Guid>
        {
            Id = Guid.Parse(SelectedQuestionTemplateId),
            DisplayName = SelectedQuestionTemplateText
        });
    }

    public string SelectedChildTab { get; set; } = "subproduct-tab";
    public MudDataGrid<ProductDto> ProductMudDataGrid { get; set; }

    private Task OnSelectedChildTabChanged(string name)
    {
        SelectedChildTab = name;

        return Task.CompletedTask;
    }


    private Task SelectAllItems()
    {
        AllProductsSelected = true;

        return Task.CompletedTask;
    }

    private Task ClearSelection()
    {
        AllProductsSelected = false;
        SelectedProducts.Clear();

        return Task.CompletedTask;
    }

    private Task SelectedProductRowsChanged()
    {
        if (SelectedProducts.Count != PageSize)
        {
            AllProductsSelected = false;
        }

        return Task.CompletedTask;
    }

    private async Task DeleteSelectedProductsAsync()
    {
        var message = AllProductsSelected
            ? L["DeleteAllRecords"].Value
            : L["DeleteSelectedRecords", SelectedProducts.Count].Value;

        if (!await UiMessageService.Confirm(message))
        {
            return;
        }

        if (AllProductsSelected)
        {
            await ProductsAppService.DeleteAllAsync(Filter);
        }
        else
        {
            await ProductsAppService.DeleteByIdsAsync(SelectedProducts.Select(x => x.Id).ToList());
        }

        SelectedProducts.Clear();
        AllProductsSelected = false;

        await GetProductsAsync();
    }

    private Func<ProductDto, bool> _quickFilter => x =>
    {   
        
        if (string.IsNullOrWhiteSpace(_searchString))
            return true;

        if (x.Code.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        if (x.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        
        return false;
    };

    private async void SearchAsync1(string filterText)
    {
        _searchString = filterText;
        if((_searchString.IsNullOrEmpty() || _searchString.Length < 3) &&  (ProductMudDataGrid.Items != null && ProductMudDataGrid.Items.Any()))
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