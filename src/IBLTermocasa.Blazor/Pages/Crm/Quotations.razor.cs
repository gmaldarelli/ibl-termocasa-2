using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IBLTermocasa.BillOfMaterials;
using IBLTermocasa.Blazor.Components;
using IBLTermocasa.Permissions;
using IBLTermocasa.Quotations;
using IBLTermocasa.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using NUglify.Helpers;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;

namespace IBLTermocasa.Blazor.Pages.Crm
{
    public partial class Quotations
    {
        protected List<BreadcrumbItem> BreadcrumbItems = new();
        protected PageToolbar Toolbar { get; } = new();
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
        private QuotationUpdateDto EditingQuotation { get; set; }

        private GetQuotationsInput Filter { get; set; }
        private QuotationDto? SelectedQuotation;
        private string _searchString;

        [Inject] private SecureConfirmationService _SecureConfirmationService { get; set; }
        [Inject] private IBillOfMaterialsAppService BillOfMaterialsAppService { get; set; }


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
            BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:Quotations"]));
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
                var billOfMaterial = await BillOfMaterialsAppService.GetAsync(input.IdBOM);
                var billOfMaterialUpdate = ObjectMapper.Map<BillOfMaterialDto, BillOfMaterialUpdateDto>(billOfMaterial);
                billOfMaterialUpdate.Status = BomStatusType.QUOTATION_REJECTED;
                await BillOfMaterialsAppService.UpdateAsync(billOfMaterial.Id, billOfMaterialUpdate);
                await QuotationsAppService.DeleteAsync(input.Id);
                await GetQuotationsAsync();
                await QuotationMudDataGrid.ReloadServerData();
            }
            else
            {
                // Cancellazione annullata
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>Cancellazione annullata...");
            }

            StateHasChanged();
        }

        private async void SearchAsync(string filterText)
        {
            _searchString = filterText;
            if ((_searchString.IsNullOrEmpty() || _searchString.Length < 3) &&
                QuotationMudDataGrid.Items != null && QuotationMudDataGrid.Items.Any())
            {
                return;
            }

            await LoadGridData(new GridState<QuotationDto>
            {
                Page = 0,
                PageSize = PageSize,
                SortDefinitions = QuotationMudDataGrid.SortDefinitions.Values.ToList()
            });
            await QuotationMudDataGrid.ReloadServerData();
            StateHasChanged();
        }

        private async Task<GridData<QuotationDto>> LoadGridData(GridState<QuotationDto> state)
        {
            state.SortDefinitions.ForEach(sortDef =>
            {
                CurrentSorting = sortDef.Descending ? $" {sortDef.SortBy} DESC" : $" {sortDef.SortBy} ";
            });
            Filter.SkipCount = state.Page * state.PageSize;
            Filter.Sorting = CurrentSorting;
            Filter.MaxResultCount = state.PageSize;
            Filter.FilterText = _searchString;
            var firstOrDefault = QuotationMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(QuotationDto.Code) });
            if (firstOrDefault != null)
            {
                Filter.Code = (string?)firstOrDefault.Value;
            }

            var firstOrDefault1 = QuotationMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(QuotationDto.Name) });
            if (firstOrDefault1 != null)
            {
                Filter.Name = (string?)firstOrDefault1.Value;
            }

            var firstOrDefault2 = QuotationMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(QuotationDto.Name) });
            if (firstOrDefault2 != null)
            {
                Filter.Name = (string)firstOrDefault2.Value!;
            }

            var firstOrDefault3 = QuotationMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(QuotationDto.SentDate) });
            if (firstOrDefault3 != null)
            {
                Filter.SentDateMin = (DateTime)firstOrDefault3.Value!;
            }

            var firstOrDefault4 = QuotationMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(QuotationDto.QuotationValidDate) });
            if (firstOrDefault4 != null)
            {
                Filter.QuotationValidDateMin = (DateTime)firstOrDefault4.Value!;
            }

            var firstOrDefault5 = QuotationMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(QuotationDto.ConfirmedDate) });
            if (firstOrDefault5 != null)
            {
                Filter.ConfirmedDateMin = (DateTime?)firstOrDefault5.Value!;
            }

            var firstOrDefault6 = QuotationMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(QuotationDto.Status) });
            if (firstOrDefault6 != null)
            {
                Filter.Status = (QuotationStatus)firstOrDefault6.Value!;
            }

            var result = await QuotationsAppService.GetListAsync(Filter);
            QuotationList = result.Items;
            GridData<QuotationDto> data = new()
            {
                Items = QuotationList,
                TotalItems = (int)result.TotalCount
            };
            return data;
        }
    }
}