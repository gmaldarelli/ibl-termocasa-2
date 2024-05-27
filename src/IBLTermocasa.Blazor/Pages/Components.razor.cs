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
using IBLTermocasa.Components;
using IBLTermocasa.Permissions;
using IBLTermocasa.Shared;
using IBLTermocasa.ComponentItems; 



namespace IBLTermocasa.Blazor.Pages
{
    public partial class Components
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<ComponentDto> ComponentList { get; set; }
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
        private DataGridEntityActionsColumn<ComponentDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "component-create-tab";
        protected string SelectedEditTab = "component-edit-tab";
        private ComponentDto? SelectedComponent;
        
        
                #region Child Entities
        
                #region ComponentItems

                private bool CanListComponentItem { get; set; }
                private bool CanCreateComponentItem { get; set; }
                private bool CanEditComponentItem { get; set; }
                private bool CanDeleteComponentItem { get; set; }
                private ComponentItemCreateDto NewComponentItem { get; set; }
                private Dictionary<Guid, DataGrid<ComponentItemWithNavigationPropertiesDto>> ComponentItemDataGrids { get; set; } = new();
                private int ComponentItemPageSize { get; } = 5;
                private DataGridEntityActionsColumn<ComponentItemWithNavigationPropertiesDto> ComponentItemEntityActionsColumns { get; set; } = new();
                private Validations NewComponentItemValidations { get; set; } = new();
                private Modal CreateComponentItemModal { get; set; } = new();
                private Guid EditingComponentItemId { get; set; }
                private ComponentItemUpdateDto EditingComponentItem { get; set; }
                private Validations EditingComponentItemValidations { get; set; } = new();
                private Modal EditComponentItemModal { get; set; } = new();
                private IReadOnlyList<LookupDto<Guid>> MaterialsCollection { get; set; } = new List<LookupDto<Guid>>();

            
                #endregion
        
        #endregion
        
        
        
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
            
            NewComponentItem = new ComponentItemCreateDto();
EditingComponentItem = new ComponentItemUpdateDto();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetMaterialLookupAsync();
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
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewComponent"], async () =>
            {
                await OpenCreateComponentModalAsync();
            }, IconName.Add, requiredPolicyName: IBLTermocasaPermissions.Components.Create);

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
                            
            
            #region ComponentItems
            CanListComponentItem = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.ComponentItems.Default);
            CanCreateComponentItem = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.ComponentItems.Create);
            CanEditComponentItem = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.ComponentItems.Edit);
            CanDeleteComponentItem = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.ComponentItems.Delete);
            #endregion                
        }

        private async Task GetComponentsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await ComponentsAppService.GetListAsync(Filter);
            ComponentList = result.Items;
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
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("IBLTermocasa") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/components/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Name={HttpUtility.UrlEncode(Filter.Name)}", forceLoad: true);
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
            NewComponent = new ComponentCreateDto{
                
                
            };
            await NewComponentValidations.ClearAll();
            await CreateComponentModal.Show();
        }

        private async Task CloseCreateComponentModalAsync()
        {
            NewComponent = new ComponentCreateDto{
                
                
            };
            await CreateComponentModal.Hide();
        }

        private async Task OpenEditComponentModalAsync(ComponentDto input)
        {
            var component = await ComponentsAppService.GetAsync(input.Id);
            
            EditingComponentId = component.Id;
            EditingComponent = ObjectMapper.Map<ComponentDto, ComponentUpdateDto>(component);
            await EditingComponentValidations.ClearAll();
            await EditComponentModal.Show();
        }

        private async Task DeleteComponentAsync(ComponentDto input)
        {
            await ComponentsAppService.DeleteAsync(input.Id);
            await GetComponentsAsync();
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
        


    private bool ShouldShowDetailRow()
    {
        return CanListComponentItem;
    }
    
    public string SelectedChildTab { get; set; } = "componentitem-tab";
        
    private Task OnSelectedChildTabChanged(string name)
    {
        SelectedChildTab = name;
    
        return Task.CompletedTask;
    }




        #region ComponentItems
        
        private async Task OnComponentItemDataGridReadAsync(DataGridReadDataEventArgs<ComponentItemWithNavigationPropertiesDto> e, Guid componentId)
        {
            var sorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");

            var currentPage = e.Page;
            await SetComponentItemsAsync(componentId, currentPage, sorting: sorting);
            await InvokeAsync(StateHasChanged);
        }
        
        private async Task SetComponentItemsAsync(Guid componentId, int currentPage = 1, string? sorting = null)
        {
            var component = ComponentList.FirstOrDefault(x => x.Id == componentId);
            if(component == null)
            {
                return;
            }

            var componentItems = await ComponentItemsAppService.GetListWithNavigationPropertiesByComponentIdAsync(new GetComponentItemListInput 
            {
                ComponentId = componentId,
                MaxResultCount = ComponentItemPageSize,
                SkipCount = (currentPage - 1) * ComponentItemPageSize,
                Sorting = sorting
            });

            component.ComponentItems = componentItems.Items.ToList();

            var componentItemDataGrid = ComponentItemDataGrids[componentId];
            
            componentItemDataGrid.CurrentPage = currentPage;
            componentItemDataGrid.TotalItems = (int)componentItems.TotalCount;
        }
        
        private async Task OpenEditComponentItemModalAsync(ComponentItemWithNavigationPropertiesDto input)
        {
            var componentItem = await ComponentItemsAppService.GetAsync(input.ComponentItem.Id);

            EditingComponentItemId = componentItem.Id;
            EditingComponentItem = ObjectMapper.Map<ComponentItemDto, ComponentItemUpdateDto>(componentItem);
            await EditingComponentItemValidations.ClearAll();
            await EditComponentItemModal.Show();
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

                await ComponentItemsAppService.UpdateAsync(EditingComponentItemId, EditingComponentItem);
                await SetComponentItemsAsync(EditingComponentItem.ComponentId);
                await EditComponentItemModal.Hide();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        
        private async Task DeleteComponentItemAsync(ComponentItemWithNavigationPropertiesDto input)
        {
            await ComponentItemsAppService.DeleteAsync(input.ComponentItem.Id);
            await SetComponentItemsAsync(input.ComponentItem.ComponentId);
        }
        
        private async Task OpenCreateComponentItemModalAsync(Guid componentId)
        {
            NewComponentItem = new ComponentItemCreateDto
            {
                ComponentId = componentId
            };

            await NewComponentItemValidations.ClearAll();
            await CreateComponentItemModal.Show();
        }
        
        private async Task CloseCreateComponentItemModalAsync()
        {
            NewComponentItem = new ComponentItemCreateDto();

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

                await ComponentItemsAppService.CreateAsync(NewComponentItem);
                await SetComponentItemsAsync(NewComponentItem.ComponentId);
                await CloseCreateComponentItemModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        
                private async Task GetMaterialLookupAsync(string? filter = null)
        {
            MaterialsCollection = (await ComponentItemsAppService.GetMaterialLookupAsync(new LookupRequestDto
            {
                Filter = filter
            })).Items;
        }
        
        #endregion
    }
}
