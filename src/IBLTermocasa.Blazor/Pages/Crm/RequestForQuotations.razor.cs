using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Web;
using Blazorise;
using Blazorise.DataGrid;
using DocumentFormat.OpenXml.Spreadsheet;
using IBLTermocasa.Blazor.Components.RequestForQuotation;
using IBLTermocasa.Common;
using IBLTermocasa.Contacts;
using IBLTermocasa.Organizations;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using IBLTermocasa.RequestForQuotations;
using IBLTermocasa.Permissions;
using IBLTermocasa.Products;
using IBLTermocasa.Shared;

using IBLTermocasa.RequestForQuotations;
using IBLTermocasa.Types;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using NUglify.Helpers;
using Volo.Abp.Identity;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;
using SortDirection = Blazorise.SortDirection;



namespace IBLTermocasa.Blazor.Pages.Crm
{
    public partial class RequestForQuotations
    {
        protected List<BreadcrumbItem> BreadcrumbItems = new();
        protected PageToolbar Toolbar { get; } = new();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<RequestForQuotationDto> RequestForQuotationList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateRequestForQuotation { get; set; }
        private bool CanEditRequestForQuotation { get; set; }
        private bool CanDeleteRequestForQuotation { get; set; }
        private RequestForQuotationCreateDto NewRequestForQuotation { get; set; }
        private Validations NewRequestForQuotationValidations { get; set; } = new();
        private RequestForQuotationUpdateDto EditingRequestForQuotation { get; set; }

        private Validations EditingRequestForQuotationValidations { get; set; } = new();
        private Guid EditingRequestForQuotationId { get; set; }
        private Modal CreateRequestForQuotationModal { get; set; } = new();
        private Modal EditRequestForQuotationModal { get; set; } = new();

        private Modal EditAttributeModal { get; set; } = new();
        private GetRequestForQuotationsInput Filter { get; set; }
        private RequestForQuotationDto? SelectedRequestForQuotation;
        private RequestForQuotationDto RequestForQuotationInput { get; set; }
        private Dictionary<string, bool> buttonVisibility = new();
        private bool isAttributeModalOpen;
        
        private ModalSize ModalSize { get; set; } = ModalSize.Large;
        [Inject] private IOrganizationsAppService OrganizationsAppService { get; set; }
        [Inject] private IContactsAppService ContactsAppService { get; set; }
        [Inject] public IDialogService DialogService { get; set; }
        [Inject] public IIdentityUserAppService UserAppService { get; set; }

        protected List<OrganizationDto> Organizations { get; set; } = new();
        protected List<ContactDto> Contacts { get; set; } = new();
        protected List<IdentityUserDto> Agents { get; set; } = new();
        protected Progress progressRef;
        protected int progress;
        private string _searchString;
        private List<LookupDto<Guid>> OrganizationsList { get; set; } = new();
        private List<LookupDto<Guid>> ContactsList { get; set; } = new();
        private List<LookupDto<Guid>> AgentsList { get; set; } = new();
        private MudDataGrid<RequestForQuotationDto> RequestForQuotationMudDataGrid { get; set; } = new();

        
// Variabili per la gestione dei filtri
        private IReadOnlyList<LookupDto<Guid>> IdentityUsersCollection { get; set; } = new List<LookupDto<Guid>>();
        private IReadOnlyList<LookupDto<Guid>> ContactsCollection { get; set; } = new List<LookupDto<Guid>>();
        private IReadOnlyList<LookupDto<Guid>> OrganizationsCollection { get; set; } = new List<LookupDto<Guid>>();

        
        public RequestForQuotations()
        {
            NewRequestForQuotation = new RequestForQuotationCreateDto();
            EditingRequestForQuotation = new RequestForQuotationUpdateDto();
            Filter = new GetRequestForQuotationsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            RequestForQuotationList = new List<RequestForQuotationDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetIdentityUserCollectionLookupAsync();
            await GetContactCollectionLookupAsync();
            await GetOrganizationCollectionLookupAsync();
            await GetRequestForQuotationsAsync();
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
            BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:RequestForQuotations"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () => { await DownloadAsExcelAsync(); }, IconName.Download);

            Toolbar.AddButton(L["NewRequestForQuotation"], () =>
                {
                    OpenCreateRequestForQuotationPageAsync();
                    return Task.CompletedTask;
                }, IconName.Add,
                requiredPolicyName: IBLTermocasaPermissions.RequestForQuotations.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateRequestForQuotation = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.RequestForQuotations.Create);
            CanEditRequestForQuotation = await AuthorizationService
                 .IsGrantedAsync(IBLTermocasaPermissions.RequestForQuotations.Edit);
            CanDeleteRequestForQuotation = await AuthorizationService
                 .IsGrantedAsync(IBLTermocasaPermissions.RequestForQuotations.Delete);
        }

        private async Task GetRequestForQuotationsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await RequestForQuotationsAppService.GetListRFQAsync(Filter);
            RequestForQuotationList = result.Items;
            TotalCount = (int)result.TotalCount;
        }
        
        private async void SearchAsync(string filterText)
        {
            _searchString = filterText;
            if((_searchString.IsNullOrEmpty() || _searchString.Length < 3) &&  (RequestForQuotationMudDataGrid.Items != null && RequestForQuotationMudDataGrid.Items.Any()))
            {
                return;
            }
            await LoadGridData(new GridState<RequestForQuotationDto>
            {
                Page = 0,
                PageSize = PageSize,
                SortDefinitions = RequestForQuotationMudDataGrid.SortDefinitions.Values.ToList()
            });
            await RequestForQuotationMudDataGrid.ReloadServerData();
            StateHasChanged();
        }
        
        private async Task<GridData<RequestForQuotationDto>> LoadGridData(GridState<RequestForQuotationDto> state)
        {
            state.SortDefinitions.ForEach(sortDef =>
            {
                CurrentSorting = sortDef.Descending ? $" {sortDef.SortBy} DESC" : $" {sortDef.SortBy} ";
            });
            Filter.SkipCount = state.Page * state.PageSize;
            Filter.Sorting = CurrentSorting;
            Filter.MaxResultCount = state.PageSize;
            Filter.FilterText = _searchString;
            var firstOrDefault = RequestForQuotationMudDataGrid.FilterDefinitions.FirstOrDefault(x => x.Column is { PropertyName: nameof(RequestForQuotationDto.QuoteNumber) });
            if (firstOrDefault != null)
            {
                Filter.QuoteNumber = (string?)firstOrDefault.Value;
            }
            var firstOrDefault1 = RequestForQuotationMudDataGrid.FilterDefinitions.FirstOrDefault(x => x.Column is { PropertyName: nameof(RequestForQuotationDto.OrganizationProperty) });
            if (firstOrDefault1 != null)
            {
                Filter.OrganizationProperty.Name = (string?)firstOrDefault1.Value;
            }
            var firstOrDefault2 = RequestForQuotationMudDataGrid.FilterDefinitions.FirstOrDefault(x => x.Column is { PropertyName: nameof(RequestForQuotationDto.ContactProperty) });
            if (firstOrDefault2 != null)
            {
                Filter.ContactProperty.Name = (string)firstOrDefault2.Value!;
            }
            var firstOrDefault3 = RequestForQuotationMudDataGrid.FilterDefinitions.FirstOrDefault(x => x.Column is { PropertyName: nameof(RequestForQuotationDto.AgentProperty) });
            if (firstOrDefault3 != null)
            {
                Filter.AgentProperty.Name = (string)firstOrDefault3.Value!;
            }
            var firstOrDefault4 = RequestForQuotationMudDataGrid.FilterDefinitions.FirstOrDefault(x => x.Column is { PropertyName: nameof(RequestForQuotationDto.WorkSite) });
            if (firstOrDefault4 != null)
            {
                Filter.WorkSite = (string)firstOrDefault4.Value!;
            }
            var firstOrDefault5 = RequestForQuotationMudDataGrid.FilterDefinitions.FirstOrDefault(x => x.Column is { PropertyName: nameof(RequestForQuotationDto.City) });
            if (firstOrDefault5 != null)
            {
                Filter.City = (string)firstOrDefault5.Value!;
            }
            var firstOrDefault6 = RequestForQuotationMudDataGrid.FilterDefinitions.FirstOrDefault(x => x.Column is { PropertyName: nameof(RequestForQuotationDto.Status) });
            if (firstOrDefault6 != null)
            {
                Filter.Status = (RfqStatus)firstOrDefault6.Value!;
            }
            var result = await RequestForQuotationsAppService.GetListRFQAsync(Filter);
            RequestForQuotationList = result.Items;
            GridData<RequestForQuotationDto> data = new()
            {
                Items = RequestForQuotationList,
                TotalItems = (int)result.TotalCount
            };
            return data;
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await RequestForQuotationsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("IBLTermocasa") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/request-for-quotations/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&QuoteNumber={HttpUtility.UrlEncode(Filter.QuoteNumber)}&WorkSite={HttpUtility.UrlEncode(Filter.WorkSite)}&City={HttpUtility.UrlEncode(Filter.City)}&OrganizationProperty={HttpUtility.UrlEncode(Filter.OrganizationProperty.ToString())}&ContactProperty={HttpUtility.UrlEncode(Filter.ContactProperty.ToString())}&PhoneInfo={HttpUtility.UrlEncode(Filter.PhoneInfo.ToString())}&MailInfo={HttpUtility.UrlEncode(Filter.MailInfo.ToString())}&DiscountMin={Filter.DiscountMin}&DiscountMax={Filter.DiscountMax}&Description={HttpUtility.UrlEncode(Filter.Description)}&Status={Filter.Status}", forceLoad: true);
        }

        private async Task DeleteRequestForQuotationAsync(RequestForQuotationDto input)
        {
            await RequestForQuotationsAppService.DeleteAsync(input.Id);
            await GetRequestForQuotationsAsync();
        }
        
        private async Task CloseEditRequestForQuotationModalAsync()
        {
            await EditRequestForQuotationModal.Hide();
        }

        private async Task UpdateRequestForQuotationAsync()
        {
            try
            {
                if (await EditingRequestForQuotationValidations.ValidateAll() == false)
                {
                    return;
                }

                await RequestForQuotationsAppService.UpdateAsync(EditingRequestForQuotationId,
                    EditingRequestForQuotation);
                await GetRequestForQuotationsAsync();
                await EditRequestForQuotationModal.Hide();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        
        private async Task OpenReadOrEditRfq(RequestForQuotationDto input, bool isReadOnly)
        {
            if (input.Status is RfqStatus.DRAFT)
            {
                NavigationManager.NavigateTo($"/rfq-draft/{input.Id}");
            }
            else
            {
                var parameters = new DialogParameters
                {
                    { "RequestForQuotation", input },
                    { "DisplayReadOnly", isReadOnly }
                };

                var dialog = await DialogService.ShowAsync<RequestForQuotationInput>(L["RequestForQuotation"], parameters, new DialogOptions
                {
                    Position = DialogPosition.Custom,
                    FullWidth = true,
                    MaxWidth = MaxWidth.Medium
                });

                var result = await dialog.Result;
                if (!result.Cancelled)
                {

                    List<RequestForQuotationDto> _tempList = new List<RequestForQuotationDto>();
                    RequestForQuotationList.ForEach(rfq =>
                    {
                        if (rfq.Id == input.Id)
                        {
                            _tempList.Add((RequestForQuotationDto)result.Data);
                        }
                        else
                        {
                            _tempList.Add(rfq);
                        }
                    });
                    RequestForQuotationList = _tempList;
                    await RequestForQuotationMudDataGrid.ReloadServerData();
                    StateHasChanged();

                }
                
            }
            StateHasChanged();
        }
        private void OpenCreateRequestForQuotationPageAsync()
        {
            //navigate to the page RequestForQuotationCreate
            NavigationManager.NavigateTo($"/rfq-create");
        }
        
// Metodi per la gestione dei filtri
        
        private async Task GetIdentityUserCollectionLookupAsync()
        {
            IdentityUsersCollection = (await RequestForQuotationsAppService.GetIdentityUserLookupAsync(new LookupRequestDto())).Items;
            AgentsList = IdentityUsersCollection.ToList();
        }
        private async Task GetContactCollectionLookupAsync()
        {
            ContactsCollection = (await RequestForQuotationsAppService.GetContactLookupAsync(new LookupRequestDto())).Items;
            ContactsList = ContactsCollection.ToList();
        }
        private async Task GetOrganizationCollectionLookupAsync()
        {
            OrganizationsCollection = (await RequestForQuotationsAppService.GetOrganizationLookupCustomerAsync(new LookupRequestDto())).Items;
            OrganizationsList = OrganizationsCollection.ToList();
        }

// Fine metodi per la gestione dei filtri

    }
}
