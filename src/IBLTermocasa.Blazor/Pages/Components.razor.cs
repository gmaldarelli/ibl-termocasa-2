using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Web;
using AngleSharp.Diffing.Extensions;
using Blazorise;
using Blazorise.DataGrid;
using Blazorise.Extensions;
using IBLTermocasa.Blazor.Components.Component;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using IBLTermocasa.Components;
using IBLTermocasa.Materials;
using IBLTermocasa.Permissions;
using IBLTermocasa.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using NUglify.Helpers;
using Volo.Abp.AspNetCore.Components.Messages;
using ComponentItemDto = IBLTermocasa.Components.ComponentItemDto;
using SortDirection = Blazorise.SortDirection;


namespace IBLTermocasa.Blazor.Pages
{
    public partial class Components
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems =
            new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();

        [Inject] public IDialogService DialogService { get; set; }
        protected PageToolbar Toolbar { get; } = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private List<ComponentDto> ComponentList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateComponent { get; set; }
        private bool CanEditComponent { get; set; }
        private bool CanDeleteComponent { get; set; }
        private ComponentCreateDto NewComponent { get; set; }
        private Validations NewComponentValidations { get; set; } = new();
        private ComponentUpdateDto EditingComponent { get; set; }
        private Validations EditingComponentValidations { get; set; } = new();
        private Guid EditingComponentId { get; set; }
        private Modal CreateComponentModal { get; set; } = new();
        private Modal EditComponentModal { get; set; } = new();
        private GetComponentsInput Filter { get; set; }
        private ComponentDto? _selectedComponent;

        private ObservableCollection<ComponentItemDto> SelectedComponentItems { get; set; } =
            new ObservableCollection<ComponentItemDto>();
        

        private bool CanListComponentItem { get; set; }
        private bool CanCreateComponentItem { get; set; }
        private bool CanEditComponentItem { get; set; }
        private bool CanDeleteComponentItem { get; set; }
        private ComponentItemDto NewComponentItem { get; set; }
        private DataGrid<ComponentItemDto> ComponentItemDataGrid { get; set; } = new();
        private DataGrid<ComponentDto> ComponentDtoDataGrid { get; set; } = new();
        private int ComponentItemPageSize { get; } = 5;
        private Validations NewComponentItemValidations { get; set; } = new();
        private Modal CreateComponentItemModal { get; set; } = new();
        private Guid EditingComponentItemId { get; set; }
        private ComponentItemDto EditingComponentItem { get; set; }
        private Validations EditingComponentItemValidations { get; set; } = new();
        private Modal EditComponentItemModal { get; set; } = new();
        private IReadOnlyList<LookupDto<Guid>> MaterialsCollection { get; set; } = new List<LookupDto<Guid>>();
        private MudDataGrid<ComponentDto> ComponentMudDataGrid { get; set; }
        private MudDataGrid<ComponentItemDto> ComponentItemMudDataGrid { get; set; }
        private IEnumerable<ComponentItemDto> ComponentItemList { get; set; } = new List<ComponentItemDto>();
        private bool ComponentSelected { get; set; } = true;


        private bool _isComponentRendered = false;
        private bool _componentItemDataGridLoaded = false;

        public Components()
        {
            NewComponent = new ComponentCreateDto();
            EditingComponent = new ComponentUpdateDto();
            Filter = new GetComponentsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            ComponentList = new List<ComponentDto>();

            NewComponentItem = new ComponentItemDto();
            EditingComponentItem = new ComponentItemDto();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetMaterialLookupAsync();
            await GetComponentsAsync();
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Components"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () => { await DownloadAsExcelAsync(); }, IconName.Download);

            Toolbar.AddButton(L["NewComponent"], async () => { await OpenCreateComponentModalAsync(); }, IconName.Add,
                requiredPolicyName: IBLTermocasaPermissions.Components.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateComponent = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.Components.Create);
            CanEditComponent = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.Components.Edit);
            CanDeleteComponent = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.Components.Delete);

            CanListComponentItem = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.ComponentItems.Default);
            CanCreateComponentItem = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.ComponentItems.Create);
            CanEditComponentItem = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.ComponentItems.Edit);
            CanDeleteComponentItem = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.ComponentItems.Delete);

        }

        private async Task GetComponentsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await ComponentsAppService.GetListAsync(Filter);
            ComponentList = result.Items.ToList();
            TotalCount = (int)result.TotalCount;
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetComponentsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await ComponentsAppService.GetDownloadTokenAsync()).Token;
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
                $"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/components/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Name={HttpUtility.UrlEncode(Filter.Name)}",
                forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<ComponentDto> e)
        {
            
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetComponentsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateComponentModalAsync()
        {
            NewComponent = new ComponentCreateDto
            {


            };
            await NewComponentValidations.ClearAll();
            await CreateComponentModal.Show();
        }

        private async Task CloseCreateComponentModalAsync()
        {
            NewComponent = new ComponentCreateDto
            {


            };
            await CreateComponentModal.Hide();
        }

        private async Task DeleteComponentAsync(ComponentDto input)
        {
            var message = L["DeleteSelectedRecords", input.Name];
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            await ComponentsAppService.DeleteAsync(input.Id);
            await GetComponentsAsync();
            StateHasChanged();
        }

        private async Task CreateComponentAsync()
        {
            try
            {
                if (await NewComponentValidations.ValidateAll() == false)
                {
                    return;
                }

                await ComponentsAppService.CreateAsync(NewComponent);
                await GetComponentsAsync();
                await CloseCreateComponentModalAsync();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditComponentModalAsync()
        {
            await EditComponentModal.Hide();
        }

        private async Task UpdateComponentAsync()
        {
            try
            {
                if (await EditingComponentValidations.ValidateAll() == false)
                {
                    return;
                }

                await ComponentsAppService.UpdateAsync(EditingComponentId, EditingComponent);
                await GetComponentsAsync();
                await EditComponentModal.Hide();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditComponentItemModalAsync()
        {
            await EditComponentItemModal.Hide();
        }

        private async Task UpdateComponentItemAsync()
        {
            try
            {
                if (await EditingComponentItemValidations.ValidateAll() == false)
                {
                    return;
                }

                var updated =
                    await ComponentsAppService.UpdateComponentItemAsync(_selectedComponent.Id, new List<ComponentItemDto>
                    {
                        EditingComponentItem
                    });
                await ReLoadComponentItemsComponent(updated);
                await EditComponentItemModal.Hide();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        
        private async Task OpenCreateComponentItemModalAsync()
        {
            var exclusionIds = ComponentItemList.Select(x => x.Id).ToList();
            
            var dialog = await DialogService.ShowAsync<AddMaterialsInput>(L["ConfirmGenerationMudDialogTitle"] ,new DialogParameters
            {
                { "ExclusionIds", exclusionIds }
            }, new DialogOptions
            {
                Position = DialogPosition.Custom,
                FullWidth = true,
                MaxWidth = MaxWidth.Medium
            });
            
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                var selectedItems = (List<MaterialDto>)result.Data;
                if (selectedItems.Count == 0)
                {
                    return;
                }
                selectedItems.ForEach(x =>
                {
                    ComponentMudDataGrid.SelectedItem.ComponentItems.Add(new ComponentItemDto
                    {
                        Id = Guid.NewGuid(),
                        MaterialId = x.Id,
                        MaterialCode = x.Code,
                        MaterialName = x.Name,
                        IsDefault = false
                    });
                });

                var updated = await ComponentsAppService.UpdateAsync(
                    ComponentMudDataGrid.SelectedItem.Id,
                    ObjectMapper.Map<ComponentDto, ComponentUpdateDto>(ComponentMudDataGrid.SelectedItem)
                );
                ComponentList.ToList().ForEach(x => {
                    if (x.Id == ComponentMudDataGrid.SelectedItem.Id) {
                        x = updated;
                    }
                });
                await ComponentMudDataGrid.ReloadServerData();
                StateHasChanged();
            }
            
        }

        private async Task CloseCreateComponentItemModalAsync()
        {
            NewComponentItem = new ComponentItemDto();
            await CreateComponentItemModal.Hide();
        }

        private async Task CreateComponentItemAsync()
        {
            try
            {
                if (await NewComponentItemValidations.ValidateAll() == false)
                {
                    return;
                }

                var updated =
                    await ComponentsAppService.CreateComponentItemAsync(_selectedComponent.Id, new List<ComponentItemDto>
                    {
                        NewComponentItem
                    });
                await ReLoadComponentItemsComponent(updated);
                await CloseCreateComponentItemModalAsync();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task GetMaterialLookupAsync(string? filter = null)
        {
            MaterialsCollection = (await ComponentsAppService.GetMaterialLookupAsync(new LookupRequestDto
            {
                Filter = filter
            })).Items;
        }

        
        private async Task DeleteComponentItemAsync(ComponentItemDto input)
        {
            var message = L["DeleteSelectedRecords", input.MaterialName!];
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            var updated = await ComponentsAppService.DeleteComponentItemAsync(_selectedComponent!.Id, input.Id);
            
            await ReLoadComponentItemsComponent(updated);
            StateHasChanged();
        }

        private async Task ReLoadComponentItemsComponent(ComponentDto updatedComponentDto)
        {
            _selectedComponent = updatedComponentDto;
            var index = ComponentList.FindIndex(x => x.Id == updatedComponentDto.Id);
            ComponentList[index] = updatedComponentDto;
            ComponentMudDataGrid.Items = ComponentList;
            
            SelectedComponentItems = new ObservableCollection<ComponentItemDto>(updatedComponentDto.ComponentItems);
            
            ComponentItemMudDataGrid.Items = SelectedComponentItems;
            ComponentItemMudDataGrid.CurrentPage = 1;
            await ComponentItemMudDataGrid.ReloadServerData();
            await ComponentItemMudDataGrid.ReloadServerData();
            StateHasChanged();
        }
        
        
        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                _isComponentRendered = true;
            }
        }

        private void OnSelectedItemChanged(ComponentDto obj)
        {
            ComponentItemList = new List<ComponentItemDto>();
            if(obj.ComponentItems.Count > 0){
                ComponentItemList = obj.ComponentItems;
            }
            _selectedComponent = obj;
            ComponentSelected = false;
            ComponentItemMudDataGrid.ReloadServerData();
            StateHasChanged();
        }

        private async Task OnCommittedItemChanges(ComponentDto obj)
        {
            EditingComponent = ObjectMapper.Map<ComponentDto, ComponentUpdateDto>(obj);
            var result =  await ComponentsAppService.UpdateAsync(obj.Id, EditingComponent);
            await GetComponentsAsync();
            StateHasChanged();
        }
    }
}
