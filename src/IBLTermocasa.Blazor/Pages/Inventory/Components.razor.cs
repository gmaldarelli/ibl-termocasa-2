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


namespace IBLTermocasa.Blazor.Pages.Inventory
{
    public partial class Components
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems =
            new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();

        [Inject] public IDialogService DialogService { get; set; }
        protected PageToolbar Toolbar { get; } = new PageToolbar();
        private List<ComponentDto> ComponentList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateComponent { get; set; }
        private bool CanEditComponent { get; set; }
        private bool CanDeleteComponent { get; set; }
        private ComponentCreateDto NewComponent { get; set; }
        private ComponentUpdateDto EditingComponent { get; set; }
        private Modal CreateComponentModal { get; set; } = new();
        private GetComponentsInput Filter { get; set; }
        private ComponentDto? _selectedComponent;
        

        private bool CanListComponentItem { get; set; }
        private bool CanCreateComponentItem { get; set; }
        private bool CanEditComponentItem { get; set; }
        private bool CanDeleteComponentItem { get; set; }
        private ComponentItemDto NewComponentItem { get; set; }
        private ComponentItemDto EditingComponentItem { get; set; }
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
                StateHasChanged();
            }
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Components"]));
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
            ComponentItemMudDataGrid.Items = new List<ComponentItemDto>();
            ComponentMudDataGrid.SelectedItem = null;
            ComponentSelected = true;
            await GetComponentsAsync();
            await ComponentMudDataGrid.ReloadServerData();
            StateHasChanged();
        }
        
        private async Task OpenCreateComponentDialog() {
            var exclusionCodes = ComponentList?.Select(x => x.Code).ToList() ?? new List<string>();

            var dialog = DialogService.Show<AddComponentsInput>(L["NewComponent"], new DialogParameters {
                { "ExclusionCodes", exclusionCodes }
            }, new DialogOptions
            {
                Position = DialogPosition.Custom,
                FullWidth = true,
                MaxWidth = MaxWidth.Small
            });

            var result = await dialog.Result;

            if (!result.Canceled) {
                var selectedItem = (ComponentDto)result.Data;

                var create = await ComponentsAppService.CreateAsync(
                    ObjectMapper.Map<ComponentDto, ComponentCreateDto>(selectedItem));
            
                ComponentList.Add(create);
                ComponentMudDataGrid.Items = ComponentList;
                ComponentMudDataGrid.SelectedItem = create;
            
                await ComponentMudDataGrid.ReloadServerData();
                StateHasChanged();
            }
        }

        
        private async Task OpenAssociateComponentItemDialogAsync()
        {
            var exclusionIds = ComponentItemList.Select(x => x.Id).ToList();
            var dialog = await DialogService.ShowAsync<AddMaterialsInput>(L["NewComponent"] ,new DialogParameters
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
                if (exclusionIds.Count == 0)
                {
                    ComponentMudDataGrid.SelectedItem.ComponentItems.First().IsDefault = true;
                }
                await UpdateComponent(ComponentMudDataGrid.SelectedItem);
                StateHasChanged();
            }
            
        }

        private async Task UpdateComponent(ComponentDto ComponentDto)
        {
            var updated = await ComponentsAppService.UpdateAsync(ComponentDto.Id,
                ObjectMapper.Map<ComponentDto, ComponentUpdateDto>(ComponentDto)
            );
            var indexComponent = ComponentList.FindIndex(x => x.Id == updated.Id);
            ComponentList[indexComponent] = updated;
            ComponentMudDataGrid.SelectedItem = ComponentList[indexComponent];
            ComponentMudDataGrid.Items = ComponentList;
            ComponentItemMudDataGrid.Items = ComponentMudDataGrid.SelectedItem.ComponentItems;
            await ComponentMudDataGrid.ReloadServerData();
            await ComponentItemMudDataGrid.ReloadServerData();
            StateHasChanged();
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
            ComponentMudDataGrid.SelectedItem = updatedComponentDto;
            StateHasChanged();
        }
        
        
        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                _isComponentRendered = true;
            }
        }

        private async Task OnSelectedItemChanged(ComponentDto obj)
        {
            ComponentItemList = new List<ComponentItemDto>();
            if(obj.ComponentItems.Count > 0){
                ComponentItemList = obj.ComponentItems;
            }
            _selectedComponent = obj;
            ComponentSelected = false;
            await ComponentItemMudDataGrid.ReloadServerData();
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
