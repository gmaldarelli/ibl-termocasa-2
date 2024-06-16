using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Web;
using Blazorise;
using Blazorise.DataGrid;
using DocumentFormat.OpenXml.Spreadsheet;
using IBLTermocasa.Common;
using IBLTermocasa.Contacts;
using IBLTermocasa.Organizations;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using IBLTermocasa.RequestForQuotations;
using IBLTermocasa.Permissions;
using IBLTermocasa.Shared;

using IBLTermocasa.RequestForQuotations;
using IBLTermocasa.Types;
using Microsoft.AspNetCore.Components;
using Volo.Abp.Identity;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;


namespace IBLTermocasa.Blazor.Pages
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

        [Inject] public IIdentityUserAppService UserAppService { get; set; }

        protected List<OrganizationDto> Organizations { get; set; } = new();
        protected List<ContactDto> Contacts { get; set; } = new();
        protected List<IdentityUserDto> Agents { get; set; } = new();
        protected Progress progressRef;
        protected int progress;
        private List<LookupDto<Guid>> OrganizationsList { get; set; } = new();
        private List<LookupDto<Guid>> ContactsList { get; set; } = new();
        private List<LookupDto<Guid>> AgentsList { get; set; } = new();
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

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetRequestForQuotationsAsync();
            await InvokeAsync(StateHasChanged);
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

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<RequestForQuotationDto> e)
        {
            progress = 25;
            await InvokeAsync(StateHasChanged);
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            progress = 50;
            await InvokeAsync(StateHasChanged);
            CurrentPage = e.Page;
            progress = 75;
            await InvokeAsync(StateHasChanged);
            await GetRequestForQuotationsAsync();
            progress = 100;
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenEditRequestForQuotationModalAsync(RequestForQuotationDto input)
        {
            RequestForQuotationInput = input;
            await EditRequestForQuotationModal.Show();
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

        private void OpenEditRequestForQuotationPageAsync(RequestForQuotationDto input)
        {
            //navigate to the page RequestForQuotationDetails
            if (input.Status == RfqStatus.DRAFT)
            {
                NavigationManager.NavigateTo($"/rfq-draft/{input.Id}");
            }
            else
            {
                NavigationManager.NavigateTo($"/request-for-quotation/{input.Id}");
            }
        }

        private void OpenCreateRequestForQuotationPageAsync()
        {
            //navigate to the page RequestForQuotationCreate
            NavigationManager.NavigateTo($"/rfq-create");
        }
        
// Metodi per la gestione dei filtri

        protected virtual async Task OnQuoteNumberChangedAsync(string? quoteNumber)
        {
            Filter.QuoteNumber = quoteNumber;
            await SearchAsync();
        }
        protected virtual async Task OnWorkSiteChangedAsync(string? workSite)
        {
            Filter.WorkSite = workSite;
            await SearchAsync();
        }
        protected virtual async Task OnCityChangedAsync(string? city)
        {
            Filter.City = city;
            await SearchAsync();
        }
        protected virtual async Task OnAgentPropertyChangedAsync(string? agentProperty)
        {
            Filter.AgentProperty = new AgentProperty(Guid.Empty, agentProperty);
            await SearchAsync();
        }
        protected virtual async Task OnOrganizationPropertyChangedAsync(string? organizationProperty)
        {
            Filter.OrganizationProperty = new OrganizationProperty(Guid.Empty, organizationProperty);
            await SearchAsync();
        }
        protected virtual async Task OnContactPropertyChangedAsync(string? contactProperty)
        {
            Filter.ContactProperty = new ContactProperty(Guid.Empty, contactProperty);
            await SearchAsync();
        }
        protected virtual async Task OnPhoneInfoChangedAsync(string? phoneInfo)
        {
            PhoneInfo temp = new PhoneInfo();
            temp.PhoneItems.Add(new PhoneItem());
            temp.PhoneItems[0].Number = phoneInfo;
            Filter.PhoneInfo = temp;
            await SearchAsync();
        }
        protected virtual async Task OnMailInfoChangedAsync(string? mailInfo)
        {
            MailInfo temp = new MailInfo();
            temp.MailItems.Add(new MailItem());
            temp.MailItems[0].Email = mailInfo;
            Filter.MailInfo = temp;
            await SearchAsync();
        }
        protected virtual async Task OnDiscountMinChangedAsync(decimal? discountMin)
        {
            Filter.DiscountMin = discountMin;
            await SearchAsync();
        }
        protected virtual async Task OnDiscountMaxChangedAsync(decimal? discountMax)
        {
            Filter.DiscountMax = discountMax;
            await SearchAsync();
        }
        protected virtual async Task OnDescriptionChangedAsync(string? description)
        {
            Filter.Description = description;
            await SearchAsync();
        }
        protected virtual async Task OnStatusChangedAsync(RfqStatus? status)
        {
            Filter.Status = status;
            await SearchAsync();
        }
        
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
