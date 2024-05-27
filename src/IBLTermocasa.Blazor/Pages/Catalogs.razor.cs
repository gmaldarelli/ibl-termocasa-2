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
using IBLTermocasa.Catalogs;
using IBLTermocasa.Permissions;
using IBLTermocasa.Shared;



namespace IBLTermocasa.Blazor.Pages
{
    public partial class Catalogs
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<CatalogWithNavigationPropertiesDto> CatalogList { get; set; }
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
        private DataGridEntityActionsColumn<CatalogWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "catalog-create-tab";
        protected string SelectedEditTab = "catalog-edit-tab";
        private CatalogWithNavigationPropertiesDto? SelectedCatalog;
        private IReadOnlyList<LookupDto<Guid>> Products { get; set; } = new List<LookupDto<Guid>>();
        
        private string SelectedProductId { get; set; }
        
        private string SelectedProductText { get; set; }

        private List<LookupDto<Guid>> SelectedProducts { get; set; } = new List<LookupDto<Guid>>();
        
        
        
        
        
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
            CatalogList = new List<CatalogWithNavigationPropertiesDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetProductLookupAsync();


            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Catalogs"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewCatalog"], async () =>
            {
                await OpenCreateCatalogModalAsync();
            }, IconName.Add, requiredPolicyName: IBLTermocasaPermissions.Catalogs.Create);

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

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetCatalogsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await CatalogsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("IBLTermocasa") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/catalogs/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Name={HttpUtility.UrlEncode(Filter.Name)}&FromMin={Filter.FromMin?.ToString("O")}&FromMax={Filter.FromMax?.ToString("O")}&ToMin={Filter.ToMin?.ToString("O")}&ToMax={Filter.ToMax?.ToString("O")}&Description={HttpUtility.UrlEncode(Filter.Description)}&ProductId={Filter.ProductId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<CatalogWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetCatalogsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateCatalogModalAsync()
        {
            SelectedProducts = new List<LookupDto<Guid>>();
            

            NewCatalog = new CatalogCreateDto{
                From = DateTime.Now,
To = DateTime.Now,

                
            };
            await NewCatalogValidations.ClearAll();
            await CreateCatalogModal.Show();
        }

        private async Task CloseCreateCatalogModalAsync()
        {
            NewCatalog = new CatalogCreateDto{
                From = DateTime.Now,
To = DateTime.Now,

                
            };
            await CreateCatalogModal.Hide();
        }

        private async Task OpenEditCatalogModalAsync(CatalogWithNavigationPropertiesDto input)
        {
            var catalog = await CatalogsAppService.GetWithNavigationPropertiesAsync(input.Catalog.Id);
            
            EditingCatalogId = catalog.Catalog.Id;
            EditingCatalog = ObjectMapper.Map<CatalogDto, CatalogUpdateDto>(catalog.Catalog);
            SelectedProducts = catalog.Products.Select(a => new LookupDto<Guid>{ Id = a.Id, DisplayName = a.Name}).ToList();

            await EditingCatalogValidations.ClearAll();
            await EditCatalogModal.Show();
        }

        private async Task DeleteCatalogAsync(CatalogWithNavigationPropertiesDto input)
        {
            await CatalogsAppService.DeleteAsync(input.Catalog.Id);
            await GetCatalogsAsync();
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
                await CloseCreateCatalogModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditCatalogModalAsync()
        {
            await EditCatalogModal.Hide();
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
                await EditCatalogModal.Hide();                
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private void OnSelectedCreateTabChanged(string name)
        {
            SelectedCreateTab = name;
        }

        private void OnSelectedEditTabChanged(string name)
        {
            SelectedEditTab = name;
        }

        protected virtual async Task OnNameChangedAsync(string? name)
        {
            Filter.Name = name;
            await SearchAsync();
        }
        protected virtual async Task OnFromMinChangedAsync(DateTime? fromMin)
        {
            Filter.FromMin = fromMin.HasValue ? fromMin.Value.Date : fromMin;
            await SearchAsync();
        }
        protected virtual async Task OnFromMaxChangedAsync(DateTime? fromMax)
        {
            Filter.FromMax = fromMax.HasValue ? fromMax.Value.Date.AddDays(1).AddSeconds(-1) : fromMax;
            await SearchAsync();
        }
        protected virtual async Task OnToMinChangedAsync(DateTime? toMin)
        {
            Filter.ToMin = toMin.HasValue ? toMin.Value.Date : toMin;
            await SearchAsync();
        }
        protected virtual async Task OnToMaxChangedAsync(DateTime? toMax)
        {
            Filter.ToMax = toMax.HasValue ? toMax.Value.Date.AddDays(1).AddSeconds(-1) : toMax;
            await SearchAsync();
        }
        protected virtual async Task OnDescriptionChangedAsync(string? description)
        {
            Filter.Description = description;
            await SearchAsync();
        }
        protected virtual async Task OnProductIdChangedAsync(Guid? productId)
        {
            Filter.ProductId = productId;
            await SearchAsync();
        }
        

        private async Task GetProductLookupAsync(string? newValue = null)
        {
            Products = (await CatalogsAppService.GetProductLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
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







    }
}
