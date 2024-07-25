using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Blazorise;
using Blazorise.DataGrid;
using IBLTermocasa.Materials;
using IBLTermocasa.Permissions;
using IBLTermocasa.Types;
using Microsoft.AspNetCore.Authorization;
using MudBlazor;
using NUglify.Helpers;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;
using SortDirection = Blazorise.SortDirection;


namespace IBLTermocasa.Blazor.Pages.Inventory
{
    public partial class Materials
    {
        protected List<BreadcrumbItem> BreadcrumbItems = new List<BreadcrumbItem>();
        protected PageToolbar Toolbar { get; } = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<MaterialDto> MaterialList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateMaterial { get; set; }
        private bool CanEditMaterial { get; set; }
        private bool CanDeleteMaterial { get; set; }
        private MaterialCreateDto NewMaterial { get; set; }
        private Validations NewMaterialValidations { get; set; } = new();
        private MaterialUpdateDto EditingMaterial { get; set; }
        private Validations EditingMaterialValidations { get; set; } = new();
        private Guid EditingMaterialId { get; set; }
        private Modal CreateMaterialModal { get; set; } = new();
        private Modal EditMaterialModal { get; set; } = new();
        private GetMaterialsInput Filter { get; set; }
        private DataGridEntityActionsColumn<MaterialDto> EntityActionsColumn { get; set; } = new();
        private MaterialDto? SelectedMaterial;
        private MudDataGrid<MaterialDto> MaterialMudDataGrid { get; set; }
        private string _searchString;


        public Materials()
        {
            NewMaterial = new MaterialCreateDto();
            EditingMaterial = new MaterialUpdateDto();
            Filter = new GetMaterialsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            MaterialList = new List<MaterialDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetMaterialsAsync();
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
            BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:Materials"]));
            return ValueTask.CompletedTask;
        }
        private async Task SetPermissionsAsync()
        {
            CanCreateMaterial = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.Materials.Create);
            CanEditMaterial = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.Materials.Edit);
            CanDeleteMaterial = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.Materials.Delete);
        }

        private async Task GetMaterialsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await MaterialsAppService.GetListAsync(Filter);
            MaterialList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await MaterialsAppService.GetDownloadTokenAsync()).Token;
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
                $"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/materials/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Code={HttpUtility.UrlEncode(Filter.Code)}&Name={HttpUtility.UrlEncode(Filter.Name)}&SourceType={Filter.SourceType}",
                forceLoad: true);
        }

        private async Task OpenCreateMaterialModalAsync()
        {
            NewMaterial = new MaterialCreateDto
            {
            };
            await NewMaterialValidations.ClearAll();
            await CreateMaterialModal.Show();
        }

        private async Task CloseCreateMaterialModalAsync()
        {
            NewMaterial = new MaterialCreateDto
            {
            };
            await CreateMaterialModal.Hide();
        }

        private async Task OpenEditMaterialModalAsync(MaterialDto input)
        {
            var material = await MaterialsAppService.GetAsync(input.Id);

            EditingMaterialId = material.Id;
            EditingMaterial = ObjectMapper.Map<MaterialDto, MaterialUpdateDto>(material);
            await EditingMaterialValidations.ClearAll();
            await EditMaterialModal.Show();
        }

        private async Task DeleteMaterialAsync(MaterialDto input)
        {
            await MaterialsAppService.DeleteAsync(input.Id);
            await GetMaterialsAsync();
        }

        private async Task CreateMaterialAsync()
        {
            try
            {
                if (await NewMaterialValidations.ValidateAll() == false)
                {
                    return;
                }

                await MaterialsAppService.CreateAsync(NewMaterial);
                await GetMaterialsAsync();
                await CloseCreateMaterialModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditMaterialModalAsync()
        {
            await EditMaterialModal.Hide();
        }

        private async Task UpdateMaterialAsync()
        {
            try
            {
                if (await EditingMaterialValidations.ValidateAll() == false)
                {
                    return;
                }

                await MaterialsAppService.UpdateAsync(EditingMaterialId, EditingMaterial);
                await GetMaterialsAsync();
                await EditMaterialModal.Hide();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async void SearchAsync(string filterText)
        {
            _searchString = filterText;
            if ((_searchString.IsNullOrEmpty() || _searchString.Length < 3) &&
                MaterialMudDataGrid.Items != null && MaterialMudDataGrid.Items.Any())
            {
                return; 
            }

            await LoadGridData(new GridState<MaterialDto>
            {
                Page = 0,
                PageSize = PageSize,
                SortDefinitions = MaterialMudDataGrid.SortDefinitions.Values.ToList()
            });
            await MaterialMudDataGrid.ReloadServerData();
            StateHasChanged();
        }

        private async Task<GridData<MaterialDto>> LoadGridData(GridState<MaterialDto> state)
        {
            state.SortDefinitions.ForEach(sortDef =>
            {
                CurrentSorting = sortDef.Descending ? $" {sortDef.SortBy} DESC" : $" {sortDef.SortBy} ";
            });
            Filter.SkipCount = state.Page * state.PageSize;
            Filter.Sorting = CurrentSorting;
            Filter.MaxResultCount = state.PageSize;
            Filter.FilterText = _searchString;
            var firstOrDefault = MaterialMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(MaterialDto.Code) });
            if (firstOrDefault != null)
            {
                Filter.Code = (string?)firstOrDefault.Value;
            }

            var firstOrDefault1 = MaterialMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(MaterialDto.Name) });
            if (firstOrDefault1 != null)
            {
                Filter.Name = (string?)firstOrDefault1.Value;
            }

            var firstOrDefault2 = MaterialMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(MaterialDto.SourceType) });
            if (firstOrDefault2 != null)
            {
                Filter.SourceType = (SourceType)firstOrDefault2.Value!;
            }

            var result = await MaterialsAppService.GetListAsync(Filter);
            MaterialList = result.Items;
            GridData<MaterialDto> data = new()
            {
                Items = MaterialList,
                TotalItems = (int)result.TotalCount
            };
            return data;
        }
    }
}