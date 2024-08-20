using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Blazorise;
using IBLTermocasa.Blazor.Components.Catalog;
using IBLTermocasa.Catalogs;
using IBLTermocasa.Permissions;
using IBLTermocasa.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using NUglify.Helpers;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;

namespace IBLTermocasa.Blazor.Pages.Crm
{
    public partial class Catalogs
    {
        protected List<BreadcrumbItem> BreadcrumbItems = new();
        protected PageToolbar Toolbar { get; } = new();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<CatalogDto> CatalogList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateCatalog { get; set; }
        private bool CanEditCatalog { get; set; }
        private bool CanDeleteCatalog { get; set; }
        private CatalogCreateDto NewCatalog { get; set; }
        private Validations NewCatalogValidations { get; set; } = new();
        private CatalogUpdateDto EditingCatalog { get; set; }
        private Validations EditingCatalogValidations { get; set; } = new();
        private Guid EditingCatalogId { get; set; }
        private Modal CreateCatalogModal { get; set; } = new();
        private Modal EditCatalogModal { get; set; } = new();
        private GetCatalogsInput Filter { get; set; }

        protected string SelectedCreateTab = "catalog-create-tab";
        protected string SelectedEditTab = "catalog-edit-tab";
        private CatalogWithNavigationPropertiesDto? SelectedCatalog;
        private IReadOnlyList<LookupDto<Guid>> Products { get; set; } = new List<LookupDto<Guid>>();

        private string SelectedProductId { get; set; }

        private string SelectedProductText { get; set; }

        private List<LookupDto<Guid>> SelectedProducts { get; set; } = new();
        private MudDataGrid<CatalogDto> CatalogMudDataGrid { get; set; } = new();
        private string _searchString;
        [Inject] public IDialogService DialogService { get; set; }

        public Catalogs()
        {
            NewCatalog = new CatalogCreateDto();
            EditingCatalog = new CatalogUpdateDto();
            Filter = new GetCatalogsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            CatalogList = new List<CatalogDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetCatalogsAsync();
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
            BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:Catalogs"]));
            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateCatalog = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.Catalogs.Create);
            CanEditCatalog = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.Catalogs.Edit);
            CanDeleteCatalog = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.Catalogs.Delete);
        }

        private async Task GetCatalogsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await CatalogsAppService.GetListAsync(Filter);
            CatalogList = result.Items;
            TotalCount = (int)result.TotalCount;
        }


        private async Task DownloadAsExcelAsync()
        {
            var token = (await CatalogsAppService.GetDownloadTokenAsync()).Token;
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
                $"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/catalogs/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Name={HttpUtility.UrlEncode(Filter.Name)}&FromMin={Filter.FromMin?.ToString("O")}&FromMax={Filter.FromMax?.ToString("O")}&ToMin={Filter.ToMin?.ToString("O")}&ToMax={Filter.ToMax?.ToString("O")}&Description={HttpUtility.UrlEncode(Filter.Description)}",
                forceLoad: true);
        }

        private async Task OpenCreateCatalogModalAsync()
        {
            SelectedProducts = new List<LookupDto<Guid>>();


            NewCatalog = new CatalogCreateDto
            {
                From = DateTime.Now,
                To = DateTime.Now,
            };
            await NewCatalogValidations.ClearAll();
            await CreateCatalogModal.Show();
        }

        private async Task CloseCreateCatalogModalAsync()
        {
            NewCatalog = new CatalogCreateDto
            {
                From = DateTime.Now,
                To = DateTime.Now,
            };
            await CreateCatalogModal.Hide();
        }

        private async Task OpenEditCatalogModalAsync(CatalogDto input)
        {
            var catalog = await CatalogsAppService.GetWithNavigationPropertiesAsync(input.Id);

            EditingCatalogId = catalog.Catalog.Id;
            EditingCatalog = ObjectMapper.Map<CatalogDto, CatalogUpdateDto>(catalog.Catalog);
            SelectedProducts = catalog.Products.Select(a => new LookupDto<Guid> { Id = a.Id, DisplayName = a.Name })
                .ToList();

            await EditingCatalogValidations.ClearAll();
            await EditCatalogModal.Show();
        }

        private async Task DeleteCatalogAsync(CatalogDto input)
        {
            await CatalogsAppService.DeleteAsync(input.Id);
            await GetCatalogsAsync();
            await CatalogMudDataGrid.ReloadServerData();
        }

        private async Task CreateCatalogAsync()
        {
            try
            {
                if (await NewCatalogValidations.ValidateAll() == false)
                {
                    return;
                }

                NewCatalog.ProductIds = SelectedProducts.Select(x => x.Id).ToList();


                await CatalogsAppService.CreateAsync(NewCatalog);
                await GetCatalogsAsync();
                await CatalogMudDataGrid.ReloadServerData();
                await CloseCreateCatalogModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        
        private async Task UpdateCatalogAsync()
        {
            try
            {
                if (await EditingCatalogValidations.ValidateAll() == false)
                {
                    return;
                }

                EditingCatalog.ProductIds = SelectedProducts.Select(x => x.Id).ToList();


                await CatalogsAppService.UpdateAsync(EditingCatalogId, EditingCatalog);
                await GetCatalogsAsync();
                await CatalogMudDataGrid.ReloadServerData();
                await EditCatalogModal.Hide();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task GetProductLookupAsync(string? newValue = null)
        {
            Products = (await CatalogsAppService.GetProductLookupAsync(new LookupRequestDto { Filter = newValue }))
                .Items;
        }

        private void AddProduct()
        {
            if (SelectedProductId.IsNullOrEmpty())
            {
                return;
            }

            if (SelectedProducts.Any(p => p.Id.ToString() == SelectedProductId))
            {
                UiMessageService.Warn(L["ItemAlreadyAdded"]);
                return;
            }

            SelectedProducts.Add(new LookupDto<Guid>
            {
                Id = Guid.Parse(SelectedProductId),
                DisplayName = SelectedProductText
            });
        }

        private async void SearchAsync(string filterText)
        {
            _searchString = filterText;
            if ((_searchString.IsNullOrEmpty() || _searchString.Length < 3) &&
                CatalogMudDataGrid.Items != null && CatalogMudDataGrid.Items.Any())
            {
                return;
            }

            await LoadGridData(new GridState<CatalogDto>
            {
                Page = 0,
                PageSize = PageSize,
                SortDefinitions = CatalogMudDataGrid.SortDefinitions.Values.ToList()
            });
            await CatalogMudDataGrid.ReloadServerData();
            StateHasChanged();
        }

        private async Task<GridData<CatalogDto>> LoadGridData(
            GridState<CatalogDto> state)
        {
            state.SortDefinitions.ForEach(sortDef =>
            {
                CurrentSorting = sortDef.Descending ? $" {sortDef.SortBy} DESC" : $" {sortDef.SortBy} ";
            });
            Filter.SkipCount = state.Page * state.PageSize;
            Filter.Sorting = CurrentSorting;
            Filter.MaxResultCount = state.PageSize;
            Filter.FilterText = _searchString;
            var firstOrDefault = CatalogMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(CatalogDto.Name) });
            if (firstOrDefault != null)
            {
                Filter.Name = (string?)firstOrDefault.Value;
            }


            var result = await CatalogsAppService.GetListAsync(Filter);
            CatalogList = result.Items;
            GridData<CatalogDto> data = new()
            {
                Items = CatalogList,
                TotalItems = (int)result.TotalCount
            };
            return data;
        }
        
        private async Task OpenNewOrReadOrEditCatalog(CatalogDto input, bool isReadOnly, bool isNew)
        {
            var parameters = new DialogParameters
            {
                { "Catalog", input },
                { "DisplayReadOnly", isReadOnly },
                { "IsNew", isNew }
            };

            var dialog = await DialogService.ShowAsync<CatalogInput>(L["Catalog"], parameters,
                new DialogOptions
                {
                    Position = DialogPosition.Custom,
                    FullWidth = true,
                    MaxWidth = MaxWidth.Small
                });

            var result = await dialog.Result;
            if (!result.Canceled)
            {
                List<CatalogDto> _tempList = new List<CatalogDto>();
                CatalogList.ForEach(catalogDto =>
                {
                    if (catalogDto.Id == input.Id)
                    {
                        _tempList.Add((CatalogDto)result.Data);
                    }
                    else
                    {
                        _tempList.Add(catalogDto);
                    }
                });
                CatalogList = _tempList;
                await CatalogMudDataGrid.ReloadServerData();
                StateHasChanged();
            }
        }
    }
}