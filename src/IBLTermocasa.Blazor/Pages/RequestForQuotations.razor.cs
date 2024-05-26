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
using IBLTermocasa.RequestForQuotations;
using IBLTermocasa.Permissions;
using IBLTermocasa.Shared;

using IBLTermocasa.RequestForQuotations;



namespace IBLTermocasa.Blazor.Pages
{
    public partial class RequestForQuotations
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<RequestForQuotationWithNavigationPropertiesDto> RequestForQuotationList { get; set; }
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
        private GetRequestForQuotationsInput Filter { get; set; }
        private DataGridEntityActionsColumn<RequestForQuotationWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "requestForQuotation-create-tab";
        protected string SelectedEditTab = "requestForQuotation-edit-tab";
        private RequestForQuotationWithNavigationPropertiesDto? SelectedRequestForQuotation;
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
            RequestForQuotationList = new List<RequestForQuotationWithNavigationPropertiesDto>();
            
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:RequestForQuotations"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewRequestForQuotation"], async () =>
            {
                await OpenCreateRequestForQuotationModalAsync();
            }, IconName.Add, requiredPolicyName: IBLTermocasaPermissions.RequestForQuotations.Create);

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

            var result = await RequestForQuotationsAppService.GetListAsync(Filter);
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
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/request-for-quotations/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&QuoteNumber={HttpUtility.UrlEncode(Filter.QuoteNumber)}&WorkSite={HttpUtility.UrlEncode(Filter.WorkSite)}&City={HttpUtility.UrlEncode(Filter.City)}&OrganizationProperty={HttpUtility.UrlEncode(Filter.OrganizationProperty)}&ContactProperty={HttpUtility.UrlEncode(Filter.ContactProperty)}&PhoneInfo={HttpUtility.UrlEncode(Filter.PhoneInfo)}&MailInfo={HttpUtility.UrlEncode(Filter.MailInfo)}&DiscountMin={Filter.DiscountMin}&DiscountMax={Filter.DiscountMax}&Description={HttpUtility.UrlEncode(Filter.Description)}&Status={Filter.Status}&AgentId={Filter.AgentId}&ContactId={Filter.ContactId}&OrganizationId={Filter.OrganizationId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<RequestForQuotationWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetRequestForQuotationsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateRequestForQuotationModalAsync()
        {
            NewRequestForQuotation = new RequestForQuotationCreateDto{
                
                
            };
            await NewRequestForQuotationValidations.ClearAll();
            await CreateRequestForQuotationModal.Show();
        }

        private async Task CloseCreateRequestForQuotationModalAsync()
        {
            NewRequestForQuotation = new RequestForQuotationCreateDto{
                
                
            };
            await CreateRequestForQuotationModal.Hide();
        }

        private async Task OpenEditRequestForQuotationModalAsync(RequestForQuotationWithNavigationPropertiesDto input)
        {
            var requestForQuotation = await RequestForQuotationsAppService.GetWithNavigationPropertiesAsync(input.RequestForQuotation.Id);
            
            EditingRequestForQuotationId = requestForQuotation.RequestForQuotation.Id;
            EditingRequestForQuotation = ObjectMapper.Map<RequestForQuotationDto, RequestForQuotationUpdateDto>(requestForQuotation.RequestForQuotation);
            await EditingRequestForQuotationValidations.ClearAll();
            await EditRequestForQuotationModal.Show();
        }

        private async Task DeleteRequestForQuotationAsync(RequestForQuotationWithNavigationPropertiesDto input)
        {
            await RequestForQuotationsAppService.DeleteAsync(input.RequestForQuotation.Id);
            await GetRequestForQuotationsAsync();
        }

        private async Task CreateRequestForQuotationAsync()
        {
            try
            {
                if (await NewRequestForQuotationValidations.ValidateAll() == false)
                {
                    return;
                }

                await RequestForQuotationsAppService.CreateAsync(NewRequestForQuotation);
                await GetRequestForQuotationsAsync();
                await CloseCreateRequestForQuotationModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
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

                await RequestForQuotationsAppService.UpdateAsync(EditingRequestForQuotationId, EditingRequestForQuotation);
                await GetRequestForQuotationsAsync();
                await EditRequestForQuotationModal.Hide();                
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
        protected virtual async Task OnOrganizationPropertyChangedAsync(string? organizationProperty)
        {
            Filter.OrganizationProperty = organizationProperty;
            await SearchAsync();
        }
        protected virtual async Task OnContactPropertyChangedAsync(string? contactProperty)
        {
            Filter.ContactProperty = contactProperty;
            await SearchAsync();
        }
        protected virtual async Task OnPhoneInfoChangedAsync(string? phoneInfo)
        {
            Filter.PhoneInfo = phoneInfo;
            await SearchAsync();
        }
        protected virtual async Task OnMailInfoChangedAsync(string? mailInfo)
        {
            Filter.MailInfo = mailInfo;
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
        protected virtual async Task OnStatusChangedAsync(Status? status)
        {
            Filter.Status = status;
            await SearchAsync();
        }
        protected virtual async Task OnAgentIdChangedAsync(Guid? agentId)
        {
            Filter.AgentId = agentId;
            await SearchAsync();
        }
        protected virtual async Task OnContactIdChangedAsync(Guid? contactId)
        {
            Filter.ContactId = contactId;
            await SearchAsync();
        }
        protected virtual async Task OnOrganizationIdChangedAsync(Guid? organizationId)
        {
            Filter.OrganizationId = organizationId;
            await SearchAsync();
        }
        

        private async Task GetIdentityUserCollectionLookupAsync(string? newValue = null)
        {
            IdentityUsersCollection = (await RequestForQuotationsAppService.GetIdentityUserLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task GetContactCollectionLookupAsync(string? newValue = null)
        {
            ContactsCollection = (await RequestForQuotationsAppService.GetContactLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task GetOrganizationCollectionLookupAsync(string? newValue = null)
        {
            OrganizationsCollection = (await RequestForQuotationsAppService.GetOrganizationLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }







    }
}
