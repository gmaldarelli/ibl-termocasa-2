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
using IBLTermocasa.Quotations;
using IBLTermocasa.Permissions;
using IBLTermocasa.Shared;

using IBLTermocasa.Types;



namespace IBLTermocasa.Blazor.Pages
{
    public partial class Quotations
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<QuotationDto> QuotationList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateQuotation { get; set; }
        private bool CanEditQuotation { get; set; }
        private bool CanDeleteQuotation { get; set; }
        private QuotationCreateDto NewQuotation { get; set; }
        private Validations NewQuotationValidations { get; set; } = new();
        private QuotationUpdateDto EditingQuotation { get; set; }
        private Validations EditingQuotationValidations { get; set; } = new();
        private Guid EditingQuotationId { get; set; }
        private Modal CreateQuotationModal { get; set; } = new();
        private Modal EditQuotationModal { get; set; } = new();
        private GetQuotationsInput Filter { get; set; }
        private DataGridEntityActionsColumn<QuotationDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "quotation-create-tab";
        protected string SelectedEditTab = "quotation-edit-tab";
        private QuotationDto? SelectedQuotation;
        
        
        public Quotations()
        {
            NewQuotation = new QuotationCreateDto();
            EditingQuotation = new QuotationUpdateDto();
            Filter = new GetQuotationsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            QuotationList = new List<QuotationDto>();
            
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Quotations"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewQuotation"], async () =>
            {
                await OpenCreateQuotationModalAsync();
            }, IconName.Add, requiredPolicyName: IBLTermocasaPermissions.Quotations.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateQuotation = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.Quotations.Create);
            CanEditQuotation = await AuthorizationService
                            .IsGrantedAsync(IBLTermocasaPermissions.Quotations.Edit);
            CanDeleteQuotation = await AuthorizationService
                            .IsGrantedAsync(IBLTermocasaPermissions.Quotations.Delete);
                            
                            
        }

        private async Task GetQuotationsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await QuotationsAppService.GetListAsync(Filter);
            QuotationList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetQuotationsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await QuotationsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("IBLTermocasa") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/quotations/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Code={HttpUtility.UrlEncode(Filter.Code)}&Name={HttpUtility.UrlEncode(Filter.Name)}&SentDateMin={Filter.SentDateMin?.ToString("O")}&SentDateMax={Filter.SentDateMax?.ToString("O")}&QuotationValidDateMin={Filter.QuotationValidDateMin?.ToString("O")}&QuotationValidDateMax={Filter.QuotationValidDateMax?.ToString("O")}&ConfirmedDateMin={Filter.ConfirmedDateMin?.ToString("O")}&ConfirmedDateMax={Filter.ConfirmedDateMax?.ToString("O")}&Status={Filter.Status}&DepositRequired={Filter.DepositRequired}&DepositRequiredValueMin={Filter.DepositRequiredValueMin}&DepositRequiredValueMax={Filter.DepositRequiredValueMax}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<QuotationDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetQuotationsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateQuotationModalAsync()
        {
            NewQuotation = new QuotationCreateDto{
                SentDate = DateTime.Now,
QuotationValidDate = DateTime.Now,
ConfirmedDate = DateTime.Now,

                
            };
            await NewQuotationValidations.ClearAll();
            await CreateQuotationModal.Show();
        }

        private async Task CloseCreateQuotationModalAsync()
        {
            NewQuotation = new QuotationCreateDto{
                SentDate = DateTime.Now,
QuotationValidDate = DateTime.Now,
ConfirmedDate = DateTime.Now,

                
            };
            await CreateQuotationModal.Hide();
        }

        private async Task OpenEditQuotationModalAsync(QuotationDto input)
        {
            var quotation = await QuotationsAppService.GetAsync(input.Id);
            NavigationManager.NavigateTo($"/quotation/{quotation.Id}"); 
        }

        private async Task DeleteQuotationAsync(QuotationDto input)
        {
            await QuotationsAppService.DeleteAsync(input.Id);
            await GetQuotationsAsync();
        }

        private async Task CreateQuotationAsync()
        {
            try
            {
                if (await NewQuotationValidations.ValidateAll() == false)
                {
                    return;
                }

                await QuotationsAppService.CreateAsync(NewQuotation);
                await GetQuotationsAsync();
                await CloseCreateQuotationModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditQuotationModalAsync()
        {
            await EditQuotationModal.Hide();
        }

        private async Task UpdateQuotationAsync()
        {
            try
            {
                if (await EditingQuotationValidations.ValidateAll() == false)
                {
                    return;
                }

                await QuotationsAppService.UpdateAsync(EditingQuotationId, EditingQuotation);
                await GetQuotationsAsync();
                await EditQuotationModal.Hide();                
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
        protected virtual async Task OnSentDateMinChangedAsync(DateTime? sentDateMin)
        {
            Filter.SentDateMin = sentDateMin.HasValue ? sentDateMin.Value.Date : sentDateMin;
            await SearchAsync();
        }
        protected virtual async Task OnSentDateMaxChangedAsync(DateTime? sentDateMax)
        {
            Filter.SentDateMax = sentDateMax.HasValue ? sentDateMax.Value.Date.AddDays(1).AddSeconds(-1) : sentDateMax;
            await SearchAsync();
        }
        protected virtual async Task OnQuotationValidDateMinChangedAsync(DateTime? quotationValidDateMin)
        {
            Filter.QuotationValidDateMin = quotationValidDateMin.HasValue ? quotationValidDateMin.Value.Date : quotationValidDateMin;
            await SearchAsync();
        }
        protected virtual async Task OnQuotationValidDateMaxChangedAsync(DateTime? quotationValidDateMax)
        {
            Filter.QuotationValidDateMax = quotationValidDateMax.HasValue ? quotationValidDateMax.Value.Date.AddDays(1).AddSeconds(-1) : quotationValidDateMax;
            await SearchAsync();
        }
        protected virtual async Task OnConfirmedDateMinChangedAsync(DateTime? confirmedDateMin)
        {
            Filter.ConfirmedDateMin = confirmedDateMin.HasValue ? confirmedDateMin.Value.Date : confirmedDateMin;
            await SearchAsync();
        }
        protected virtual async Task OnConfirmedDateMaxChangedAsync(DateTime? confirmedDateMax)
        {
            Filter.ConfirmedDateMax = confirmedDateMax.HasValue ? confirmedDateMax.Value.Date.AddDays(1).AddSeconds(-1) : confirmedDateMax;
            await SearchAsync();
        }
        protected virtual async Task OnStatusChangedAsync(QuotationStatus? status)
        {
            Filter.Status = status;
            await SearchAsync();
        }
        protected virtual async Task OnDepositRequiredChangedAsync(bool? depositRequired)
        {
            Filter.DepositRequired = depositRequired;
            await SearchAsync();
        }
        protected virtual async Task OnDepositRequiredValueMinChangedAsync(double? depositRequiredValueMin)
        {
            Filter.DepositRequiredValueMin = depositRequiredValueMin;
            await SearchAsync();
        }
        protected virtual async Task OnDepositRequiredValueMaxChangedAsync(double? depositRequiredValueMax)
        {
            Filter.DepositRequiredValueMax = depositRequiredValueMax;
            await SearchAsync();
        }
    }
}
