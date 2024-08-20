using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Blazorise;
using IBLTermocasa.Blazor.Components;
using IBLTermocasa.Industries;
using IBLTermocasa.Organizations;
using IBLTermocasa.Permissions;
using IBLTermocasa.Shared;
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
    public partial class Organizations
    {
        protected List<BreadcrumbItem> BreadcrumbItems = new();
        protected PageToolbar Toolbar { get; } = new();
        private IReadOnlyList<OrganizationDto> OrganizationList { get; set; }
        private MudDataGrid<OrganizationDto> OrganizationMudDataGrid { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.MaxMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateOrganization { get; set; }
        private bool CanEditOrganization { get; set; }
        private bool CanDeleteOrganization { get; set; }
        private OrganizationDto OrganizationInput { get; set; }
        private Modal CreateOrganizationModal { get; set; } = new();
        private GetOrganizationsInput Filter { get; set; }
        private OrganizationDto? SelectedOrganization;
        private IReadOnlyList<LookupDto<Guid>> IndustriesCollection { get; set; } = new List<LookupDto<Guid>>();

        [Inject] private IIndustriesAppService _industriesAppService { get; set; }
        [Inject] private SecureConfirmationService _SecureConfirmationService { get; set; }

        private List<IndustryDto> Industries { get; set; } = [];
        protected List<OrganizationType> OrganizationTypes { get; set; }
        private string _searchString;


        public Organizations()
        {
            OrganizationInput = new OrganizationDto();
            Filter = new GetOrganizationsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            OrganizationList = new List<OrganizationDto>();
            OrganizationTypes = Enum.GetValuesAsUnderlyingType<OrganizationType>().ToDynamicList<OrganizationType>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetOrganizationsAsync();
            await GetIndustryCollectionLookupAsync();
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
            BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:Organizations"]));
            return ValueTask.CompletedTask;
        }


        private async Task SetPermissionsAsync()
        {
            CanCreateOrganization = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.Organizations.Create);
            CanEditOrganization = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.Organizations.Edit);
            CanDeleteOrganization = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.Organizations.Delete);
        }

        private async Task GetOrganizationsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await OrganizationsAppService.GetListAsync(Filter);

            var temp = new List<OrganizationDto>();
            result.Items.ForEach(item => { temp.Add(item.Organization); });
            OrganizationList = temp;
            TotalCount = (int)result.TotalCount;
            await IndustriesAppService.GetListAsync(new GetIndustriesInput
            {
                MaxResultCount = 100,
                SkipCount = (1 - 1) * 100,
                Sorting = "Code"
            }).ContinueWith(task => { Industries = task.Result.Items.ToList(); });
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await OrganizationsAppService.GetDownloadTokenAsync()).Token;
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
                $"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/organizations/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}{culture}&Name={Filter.Name}",
                forceLoad: true);
        }

        private async Task OpenCreateOrganizationModalAsync()
        {
            OrganizationInput = new OrganizationDto() { };
            SelectedOrganization = OrganizationInput;
            NavigationManager.NavigateTo($"/organization/{Guid.Empty}");
            //await CreateOrganizationModal.Show();
        }

        private void OpenEditOrganizationPageAsync(OrganizationDto input)
        {
            //navigate to the page OrganizationDetails
            SelectedOrganization = input;
            NavigationManager.NavigateTo($"/organization/{input.Id}");
        }

        private async Task DeleteOrganizationAsync(OrganizationDto input)
        {
            bool result = await _SecureConfirmationService.ShowConfirmation(
                "Sei sicuro di voler eliminare questa organizzazione?",
                "Scrivi il nome dell'organizzazione {0} per confermare l'eliminazione",
                input.Name
            );
            if (result)
            {
                // Procedi con la cancellazione
                Console.WriteLine("Cancellazione in corso organization id: " + input.Id);
                await OrganizationsAppService.DeleteAsync(input.Id);
                await GetOrganizationsAsync();
                await OrganizationMudDataGrid.ReloadServerData();
            }
            else
            {
                // Cancellazione annullata
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>Cancellazione annullata...");
            }

            StateHasChanged();
        }

        private async Task GetIndustryCollectionLookupAsync(string? newValue = null)
        {
            IndustriesCollection =
                (await OrganizationsAppService.GetIndustryLookupAsync(new LookupRequestDto { Filter = newValue }))
                .Items;
        }

        private async void SearchAsync(string filterText)
        {
            _searchString = filterText;
            if ((_searchString.IsNullOrEmpty() || _searchString.Length < 3) &&
                OrganizationMudDataGrid.Items != null && OrganizationMudDataGrid.Items.Any())
            {
                return;
            }

            await LoadGridData(new GridState<OrganizationDto>
            {
                Page = 0,
                PageSize = PageSize,
                SortDefinitions = OrganizationMudDataGrid.SortDefinitions.Values.ToList()
            });
            await OrganizationMudDataGrid.ReloadServerData();
            StateHasChanged();
        }

        private async Task<GridData<OrganizationDto>> LoadGridData(GridState<OrganizationDto> state)
        {
            state.SortDefinitions.ForEach(sortDef =>
            {
                CurrentSorting = sortDef.Descending ? $" {sortDef.SortBy} DESC" : $" {sortDef.SortBy} ";
            });
            Filter.SkipCount = state.Page * state.PageSize;
            Filter.Sorting = CurrentSorting;
            Filter.MaxResultCount = state.PageSize;
            Filter.FilterText = _searchString;
            var firstOrDefault = OrganizationMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(OrganizationDto.Code) });
            if (firstOrDefault != null)
            {
                Filter.Code = (string?)firstOrDefault.Value;
            }
            
            var firstOrDefault1 = OrganizationMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(OrganizationDto.Name) });
            if (firstOrDefault1 != null)
            {
                Filter.Name = (string?)firstOrDefault1.Value;
            }

            var firstOrDefault2 = OrganizationMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(OrganizationDto.Phones) });
            if (firstOrDefault2 != null)
            {
                Filter.PhoneInfo = (string?)firstOrDefault2.Value;
            }

            var firstOrDefault3 = OrganizationMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(OrganizationDto.Emails) });
            if (firstOrDefault3 != null)
            {
                Filter.MailInfo = (string)firstOrDefault3.Value!;
            }

            var firstOrDefault4 = OrganizationMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(OrganizationDto.OrganizationType) });
            if (firstOrDefault4 != null)
            {
                Filter.OrganizationType = (OrganizationType)firstOrDefault4.Value!;
            }

            var firstOrDefault5 = OrganizationMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(OrganizationDto.SourceType) });
            if (firstOrDefault5 != null)
            {
                Filter.SourceType = (SourceType)firstOrDefault5.Value!;
            }

            var result = await OrganizationsAppService.GetListAsync(Filter);

            var temp = new List<OrganizationDto>();
            result.Items.ForEach(item => { temp.Add(item.Organization); });
            OrganizationList = temp;
            GridData<OrganizationDto> data = new()
            {
                Items = OrganizationList,
                TotalItems = (int)result.TotalCount
            };
            return data;
        }
    }
}