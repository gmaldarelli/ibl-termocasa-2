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
using IBLTermocasa.ConsumptionEstimations;
using IBLTermocasa.Permissions;
using IBLTermocasa.Shared;



namespace IBLTermocasa.Blazor.Pages
{
    public partial class ConsumptionEstimations
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<ConsumptionEstimationDto> ConsumptionEstimationList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateConsumptionEstimation { get; set; }
        private bool CanEditConsumptionEstimation { get; set; }
        private bool CanDeleteConsumptionEstimation { get; set; }
        private ConsumptionEstimationCreateDto NewConsumptionEstimation { get; set; }
        private Validations NewConsumptionEstimationValidations { get; set; } = new();
        private ConsumptionEstimationUpdateDto EditingConsumptionEstimation { get; set; }
        private Validations EditingConsumptionEstimationValidations { get; set; } = new();
        private Guid EditingConsumptionEstimationId { get; set; }
        private Modal CreateConsumptionEstimationModal { get; set; } = new();
        private Modal EditConsumptionEstimationModal { get; set; } = new();
        private GetConsumptionEstimationsInput Filter { get; set; }
        private DataGridEntityActionsColumn<ConsumptionEstimationDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "consumptionEstimation-create-tab";
        protected string SelectedEditTab = "consumptionEstimation-edit-tab";
        private ConsumptionEstimationDto? SelectedConsumptionEstimation;
        
        
        
        
        
        private List<ConsumptionEstimationDto> SelectedConsumptionEstimations { get; set; } = new();
        private bool AllConsumptionEstimationsSelected { get; set; }
        
        public ConsumptionEstimations()
        {
            NewConsumptionEstimation = new ConsumptionEstimationCreateDto();
            EditingConsumptionEstimation = new ConsumptionEstimationUpdateDto();
            Filter = new GetConsumptionEstimationsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            ConsumptionEstimationList = new List<ConsumptionEstimationDto>();
            
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:ConsumptionEstimations"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewConsumptionEstimation"], async () =>
            {
                await OpenCreateConsumptionEstimationModalAsync();
            }, IconName.Add, requiredPolicyName: IBLTermocasaPermissions.ConsumptionEstimations.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateConsumptionEstimation = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.ConsumptionEstimations.Create);
            CanEditConsumptionEstimation = await AuthorizationService
                            .IsGrantedAsync(IBLTermocasaPermissions.ConsumptionEstimations.Edit);
            CanDeleteConsumptionEstimation = await AuthorizationService
                            .IsGrantedAsync(IBLTermocasaPermissions.ConsumptionEstimations.Delete);
                            
                            
        }

        private async Task GetConsumptionEstimationsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await ConsumptionEstimationsAppService.GetListAsync(Filter);
            ConsumptionEstimationList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetConsumptionEstimationsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await ConsumptionEstimationsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("IBLTermocasa") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/consumption-estimations/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&ConsumptionProduct={HttpUtility.UrlEncode(Filter.ConsumptionProduct)}&ConsumptionWork={HttpUtility.UrlEncode(Filter.ConsumptionWork)}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<ConsumptionEstimationDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetConsumptionEstimationsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateConsumptionEstimationModalAsync()
        {
            NewConsumptionEstimation = new ConsumptionEstimationCreateDto{
                
                
            };
            await NewConsumptionEstimationValidations.ClearAll();
            await CreateConsumptionEstimationModal.Show();
        }

        private async Task CloseCreateConsumptionEstimationModalAsync()
        {
            NewConsumptionEstimation = new ConsumptionEstimationCreateDto{
                
                
            };
            await CreateConsumptionEstimationModal.Hide();
        }

        private async Task OpenEditConsumptionEstimationModalAsync(ConsumptionEstimationDto input)
        {
            var consumptionEstimation = await ConsumptionEstimationsAppService.GetAsync(input.Id);
            
            EditingConsumptionEstimationId = consumptionEstimation.Id;
            EditingConsumptionEstimation = ObjectMapper.Map<ConsumptionEstimationDto, ConsumptionEstimationUpdateDto>(consumptionEstimation);
            await EditingConsumptionEstimationValidations.ClearAll();
            await EditConsumptionEstimationModal.Show();
        }

        private async Task DeleteConsumptionEstimationAsync(ConsumptionEstimationDto input)
        {
            await ConsumptionEstimationsAppService.DeleteAsync(input.Id);
            await GetConsumptionEstimationsAsync();
        }

        private async Task CreateConsumptionEstimationAsync()
        {
            try
            {
                if (await NewConsumptionEstimationValidations.ValidateAll() == false)
                {
                    return;
                }

                await ConsumptionEstimationsAppService.CreateAsync(NewConsumptionEstimation);
                await GetConsumptionEstimationsAsync();
                await CloseCreateConsumptionEstimationModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditConsumptionEstimationModalAsync()
        {
            await EditConsumptionEstimationModal.Hide();
        }

        private async Task UpdateConsumptionEstimationAsync()
        {
            try
            {
                if (await EditingConsumptionEstimationValidations.ValidateAll() == false)
                {
                    return;
                }

                await ConsumptionEstimationsAppService.UpdateAsync(EditingConsumptionEstimationId, EditingConsumptionEstimation);
                await GetConsumptionEstimationsAsync();
                await EditConsumptionEstimationModal.Hide();                
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

        protected virtual async Task OnConsumptionProductChangedAsync(string? consumptionProduct)
        {
            Filter.ConsumptionProduct = consumptionProduct;
            await SearchAsync();
        }
        protected virtual async Task OnConsumptionWorkChangedAsync(string? consumptionWork)
        {
            Filter.ConsumptionWork = consumptionWork;
            await SearchAsync();
        }
        





        private Task SelectAllItems()
        {
            AllConsumptionEstimationsSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllConsumptionEstimationsSelected = false;
            SelectedConsumptionEstimations.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedConsumptionEstimationRowsChanged()
        {
            if (SelectedConsumptionEstimations.Count != PageSize)
            {
                AllConsumptionEstimationsSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedConsumptionEstimationsAsync()
        {
            var message = AllConsumptionEstimationsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedConsumptionEstimations.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllConsumptionEstimationsSelected)
            {
                await ConsumptionEstimationsAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await ConsumptionEstimationsAppService.DeleteByIdsAsync(SelectedConsumptionEstimations.Select(x => x.Id).ToList());
            }

            SelectedConsumptionEstimations.Clear();
            AllConsumptionEstimationsSelected = false;

            await GetConsumptionEstimationsAsync();
        }


    }
}
