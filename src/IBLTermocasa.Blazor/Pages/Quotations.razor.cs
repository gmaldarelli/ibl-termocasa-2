using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Web;
using Blazorise;
using Blazorise.DataGrid;
using IBLTermocasa.Blazor.Components;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using IBLTermocasa.Quotations;
using IBLTermocasa.Permissions;
using IBLTermocasa.Shared;

using IBLTermocasa.Types;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using SortDirection = Blazorise.SortDirection;


namespace IBLTermocasa.Blazor.Pages
{
    public partial class Quotations
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<QuotationDto> QuotationList { get; set; }
        private MudDataGrid<QuotationDto> QuotationMudDataGrid { get; set; }
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

        private GetQuotationsInput Filter { get; set; }
        private DataGridEntityActionsColumn<QuotationDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "quotation-create-tab";
        protected string SelectedEditTab = "quotation-edit-tab";
        private QuotationDto? SelectedQuotation;
        
        [Inject]
        private SecureConfirmationService _SecureConfirmationService { get; set; }
        
        
        
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
            await GetQuotationsAsync();
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Quotations"]));
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

        /*private async Task DownloadAsExcelAsync()
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
        }*/
        
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
        
        private void OpenEditQuotationPageAsync(QuotationDto input)
        {
            //navigate to the page Quotation
            SelectedQuotation = input;
            NavigationManager.NavigateTo($"/quotation/{input.Id}");
        }
        
        private async Task DeleteQuotationAsync(QuotationDto input)
        {
            
            bool result = await _SecureConfirmationService.ShowConfirmation(
                "Sei sicuro di voler eliminare questo preventivo?",
                "Scrivi il codice del preventivo {0} per confermare l'eliminazione",
                input.Code
            );
            if (result)
            {
                // Procedi con la cancellazione
                Console.WriteLine("Cancellazione in corso preventivo : " + input.Id);
                await QuotationsAppService.DeleteAsync(input.Id);
                await GetQuotationsAsync();
            }
            else
            {
                // Cancellazione annullata
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>Cancellazione annullata...");
            }
            StateHasChanged();
        }
    }
}
