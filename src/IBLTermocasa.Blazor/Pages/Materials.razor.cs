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
using IBLTermocasa.Materials;
using IBLTermocasa.Permissions;
using IBLTermocasa.Shared;

using IBLTermocasa.Types;



namespace IBLTermocasa.Blazor.Pages
{
    public partial class Materials
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
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
        protected string SelectedCreateTab = "material-create-tab";
        protected string SelectedEditTab = "material-edit-tab";
        private MaterialDto? SelectedMaterial;
        
        
        
        
        
        private List<MaterialDto> SelectedMaterials { get; set; } = new();
        private bool AllMaterialsSelected { get; set; }
        
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Materials"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewMaterial"], async () =>
            {
                await OpenCreateMaterialModalAsync();
            }, IconName.Add, requiredPolicyName: IBLTermocasaPermissions.Materials.Create);

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
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetMaterialsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await MaterialsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("IBLTermocasa") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/materials/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Code={HttpUtility.UrlEncode(Filter.Code)}&Name={HttpUtility.UrlEncode(Filter.Name)}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<MaterialDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetMaterialsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateMaterialModalAsync()
        {
            NewMaterial = new MaterialCreateDto{
                
                
            };
            await NewMaterialValidations.ClearAll();
            await CreateMaterialModal.Show();
        }

        private async Task CloseCreateMaterialModalAsync()
        {
            NewMaterial = new MaterialCreateDto{
                
                
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

        private void OnSelectedCreateTabChanged(string name)
        {
            SelectedCreateTab = name;
        }

        private void OnSelectedEditTabChanged(string name)
        {
            SelectedEditTab = name;
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
        





        private Task SelectAllItems()
        {
            AllMaterialsSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllMaterialsSelected = false;
            SelectedMaterials.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedMaterialRowsChanged()
        {
            if (SelectedMaterials.Count != PageSize)
            {
                AllMaterialsSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedMaterialsAsync()
        {
            var message = AllMaterialsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedMaterials.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllMaterialsSelected)
            {
                await MaterialsAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await MaterialsAppService.DeleteByIdsAsync(SelectedMaterials.Select(x => x.Id).ToList());
            }

            SelectedMaterials.Clear();
            AllMaterialsSelected = false;

            await GetMaterialsAsync();
        }


    }
}
