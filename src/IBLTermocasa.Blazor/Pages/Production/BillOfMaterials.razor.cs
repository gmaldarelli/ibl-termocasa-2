using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using IBLTermocasa.BillOfMaterials;
using IBLTermocasa.Blazor.Components;
using IBLTermocasa.Blazor.Components.BillOfMaterial;
using IBLTermocasa.Permissions;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using NUglify.Helpers;


namespace IBLTermocasa.Blazor.Pages.Production
{
    public partial class BillOfMaterials
    {
        [Inject]
        public IDialogService DialogService { get; set; }
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();   
        [Inject]
        private SecureConfirmationService _SecureConfirmationService { get; set; }
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<BillOfMaterialDto> BillOfMaterialList { get; set; } = new List<BillOfMaterialDto>();
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateBillOfMaterial { get; set; }
        private bool CanEditBillOfMaterial { get; set; }
        private bool CanDeleteBillOfMaterial { get; set; }
        private GetBillOfMaterialsInput Filter { get; set; } = new();
        private BillOfMaterialDto? SelectedBillOfMaterial;
        private ModalBillOfMaterialInput AddBillOfMaterialModal { get; set; }
        
        private List<BillOfMaterialDto> SelectedBillOfMaterials { get; set; } = new();
        private bool AllBillOfMaterialsSelected { get; set; }
        private string _searchString;
        private MudDataGrid<BillOfMaterialDto> BillOfMaterialMudDataGrid { get; set; } = new();
        
        public BillOfMaterials()
        {
            Filter = new GetBillOfMaterialsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            BillOfMaterialList = new List<BillOfMaterialDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await NewRefqBadgeContributor();
            await GetBillOfMaterialsAsync();
        }



        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await SetBreadcrumbItemsAsync();
                await NewRefqBadgeContributor();
                StateHasChanged();
            }
        }  

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:BillOfMaterials"]));
            return ValueTask.CompletedTask;
        }
        

        private async Task OpenDialogRfqChoiceAsync()
        {
            var parameters = new DialogParameters<BillOfMaterialFromRrq>();
            var dialog = await DialogService.ShowAsync<BillOfMaterialFromRrq>(@L["SelectQuote"], parameters, new DialogOptions
            {

                FullWidth= true,
                MaxWidth=MaxWidth.Large,
                CloseButton= true,
                BackdropClick= true,
                NoHeader=false,
                Position = DialogPosition.Custom,
                CloseOnEscapeKey=false
            });
            var result = await dialog.Result;
            await NewRefqBadgeContributor();
            await GetBillOfMaterialsAsync();
        }

        private string? BadgeContent { get; set; } = "0";
        
        private async Task NewRefqBadgeContributor()
        {
            var result = await RequestForQuotationsAppService.GetNewRequestForQuotationCountAsync();
            BadgeContent = result.Value.ToString();
            Toolbar.Contributors.ToList().ForEach(
                x =>
                {
                    SimplePageToolbarContributor y = (SimplePageToolbarContributor)x;
                    if (y.Arguments != null && y!.Arguments!.ContainsKey("Content"))
                    {
                        y.Arguments["Content"] = BadgeContent;
                    }
                });
            StateHasChanged();
        }
        private async Task SetPermissionsAsync()
        {
            CanCreateBillOfMaterial = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.BillOfMaterials.Create);
            CanEditBillOfMaterial = await AuthorizationService
                            .IsGrantedAsync(IBLTermocasaPermissions.BillOfMaterials.Edit);
            CanDeleteBillOfMaterial = await AuthorizationService
                            .IsGrantedAsync(IBLTermocasaPermissions.BillOfMaterials.Delete);
                            
                            
        }

        private async Task GetBillOfMaterialsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await BillOfMaterialsAppService.GetListAsync(Filter);
            BillOfMaterialList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await BillOfMaterialsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("IBLTermocasa") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/bill-of-materials/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&BomNumber={HttpUtility.UrlEncode(Filter.BomNumber)}", forceLoad: true);
        }
        
        private Task ClearSelection()
        {
            AllBillOfMaterialsSelected = false;
            SelectedBillOfMaterials.Clear();
            
            return Task.CompletedTask;
        }

        private void CloseModalBillOfMaterialAsync()
        {
            AddBillOfMaterialModal.Hide();
            InvokeAsync(StateHasChanged);
        }
        
        private async void SaveModalBillOfMaterialInputAsync(BillOfMaterialDto obj)
        {
            AddBillOfMaterialModal.Hide();
            await InvokeAsync(StateHasChanged);
        }

        private void OpenBillOfMaterialDetailAsync(BillOfMaterialDto contextItem)
        {
            NavigationManager.NavigateTo($"/bill-of-materials-detail/{contextItem.Id}");
        }

        private async void RemoveBillOfMaterialDetailAsync(BillOfMaterialDto contextItem)
        {
             bool result = await _SecureConfirmationService.ShowConfirmation(
                "Sei sicuro di voler eliminare questa Distinta? La richiesta di preventivo associata tornerà in stato di Nuova",
                "Scrivi il numero di distinta {0} per confermare l'eliminazione",
                contextItem.BomNumber
            );
            if (result)
            {
                await BillOfMaterialsAppService.DeleteAsync(contextItem.Id);
                await GetBillOfMaterialsAsync();
                await BillOfMaterialMudDataGrid.ReloadServerData();
            }
            else
            {
                // Cancellazione annullata
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>Cancellazione annullata...");
            }
            await NewRefqBadgeContributor();
            StateHasChanged();
        }
        
        private async void SearchAsync(string filterText)
        {
            _searchString = filterText;
            if ((_searchString.IsNullOrEmpty() || _searchString.Length < 3) &&
                BillOfMaterialMudDataGrid.Items != null && BillOfMaterialMudDataGrid.Items.Any())
            {
                return;
            }

            await LoadGridData(new GridState<BillOfMaterialDto>
            {
                Page = 0,
                PageSize = PageSize,
                SortDefinitions = BillOfMaterialMudDataGrid.SortDefinitions.Values.ToList()
            });
            await BillOfMaterialMudDataGrid.ReloadServerData();
            StateHasChanged();
        }

        private async Task<GridData<BillOfMaterialDto>> LoadGridData(GridState<BillOfMaterialDto> state)
        {
            state.SortDefinitions.ForEach(sortDef =>
            {
                CurrentSorting = sortDef.Descending ? $" {sortDef.SortBy} DESC" : $" {sortDef.SortBy} ";
            });
            Filter.SkipCount = state.Page * state.PageSize;
            Filter.Sorting = CurrentSorting;
            Filter.MaxResultCount = state.PageSize;
            Filter.FilterText = _searchString;
            var firstOrDefault = BillOfMaterialMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(BillOfMaterialDto.BomNumber) });
            if (firstOrDefault != null)
            {
                Filter.BomNumber = (string?)firstOrDefault.Value!;
            }

            var firstOrDefault1 = BillOfMaterialMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(BillOfMaterialDto.RequestForQuotationProperty) });
            if (firstOrDefault1 != null)
            {
                Filter.RequestForQuotationProperty.Name = (string?)firstOrDefault1.Value;
            }
            
            var firstOrDefault2 = BillOfMaterialMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(BillOfMaterialDto.RequestForQuotationProperty) });
            if (firstOrDefault2 != null)
            {
                Filter.RequestForQuotationProperty.OrganizationName = (string?)firstOrDefault2.Value;
            }

            var result = await BillOfMaterialsAppService.GetListAsync(Filter);
            BillOfMaterialList = result.Items;
            GridData<BillOfMaterialDto> data = new()
            {
                Items = BillOfMaterialList,
                TotalItems = (int)result.TotalCount
            };
            return data;
        }
    }
}
