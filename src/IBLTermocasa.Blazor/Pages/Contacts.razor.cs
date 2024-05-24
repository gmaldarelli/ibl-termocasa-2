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
using IBLTermocasa.Contacts;
using IBLTermocasa.Permissions;
using IBLTermocasa.Shared;



namespace IBLTermocasa.Blazor.Pages
{
    public partial class Contacts
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<ContactDto> ContactList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateContact { get; set; }
        private bool CanEditContact { get; set; }
        private bool CanDeleteContact { get; set; }
        private ContactCreateDto NewContact { get; set; }
        private Validations NewContactValidations { get; set; } = new();
        private ContactUpdateDto EditingContact { get; set; }
        private Validations EditingContactValidations { get; set; } = new();
        private Guid EditingContactId { get; set; }
        private Modal CreateContactModal { get; set; } = new();
        private Modal EditContactModal { get; set; } = new();
        private GetContactsInput Filter { get; set; }
        private DataGridEntityActionsColumn<ContactDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "contact-create-tab";
        protected string SelectedEditTab = "contact-edit-tab";
        private ContactDto? SelectedContact;
        
        
        
        
        
        
        public Contacts()
        {
            NewContact = new ContactCreateDto();
            EditingContact = new ContactUpdateDto();
            Filter = new GetContactsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            ContactList = new List<ContactDto>();
            
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Contacts"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewContact"], async () =>
            {
                await OpenCreateContactModalAsync();
            }, IconName.Add, requiredPolicyName: IBLTermocasaPermissions.Contacts.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateContact = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.Contacts.Create);
            CanEditContact = await AuthorizationService
                            .IsGrantedAsync(IBLTermocasaPermissions.Contacts.Edit);
            CanDeleteContact = await AuthorizationService
                            .IsGrantedAsync(IBLTermocasaPermissions.Contacts.Delete);
                            
                            
        }

        private async Task GetContactsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await ContactsAppService.GetListAsync(Filter);
            ContactList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetContactsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await ContactsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("IBLTermocasa") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/contacts/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Title={HttpUtility.UrlEncode(Filter.Title)}&Name={HttpUtility.UrlEncode(Filter.Name)}&Surname={HttpUtility.UrlEncode(Filter.Surname)}&ConfidentialName={HttpUtility.UrlEncode(Filter.ConfidentialName)}&JobRole={HttpUtility.UrlEncode(Filter.JobRole)}&MailInfo={HttpUtility.UrlEncode(Filter.MailInfo)}&PhoneInfo={HttpUtility.UrlEncode(Filter.PhoneInfo)}&AddressInfo={HttpUtility.UrlEncode(Filter.AddressInfo)}&Tag={HttpUtility.UrlEncode(Filter.Tag)}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<ContactDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetContactsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateContactModalAsync()
        {
            NewContact = new ContactCreateDto{
                BirthDate = DateTime.Now,

                
            };
            await NewContactValidations.ClearAll();
            await CreateContactModal.Show();
        }

        private async Task CloseCreateContactModalAsync()
        {
            NewContact = new ContactCreateDto{
                BirthDate = DateTime.Now,

                
            };
            await CreateContactModal.Hide();
        }

        private async Task OpenEditContactModalAsync(ContactDto input)
        {
            var contact = await ContactsAppService.GetAsync(input.Id);
            
            EditingContactId = contact.Id;
            EditingContact = ObjectMapper.Map<ContactDto, ContactUpdateDto>(contact);
            await EditingContactValidations.ClearAll();
            await EditContactModal.Show();
        }

        private async Task DeleteContactAsync(ContactDto input)
        {
            await ContactsAppService.DeleteAsync(input.Id);
            await GetContactsAsync();
        }

        private async Task CreateContactAsync()
        {
            try
            {
                if (await NewContactValidations.ValidateAll() == false)
                {
                    return;
                }

                await ContactsAppService.CreateAsync(NewContact);
                await GetContactsAsync();
                await CloseCreateContactModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditContactModalAsync()
        {
            await EditContactModal.Hide();
        }

        private async Task UpdateContactAsync()
        {
            try
            {
                if (await EditingContactValidations.ValidateAll() == false)
                {
                    return;
                }

                await ContactsAppService.UpdateAsync(EditingContactId, EditingContact);
                await GetContactsAsync();
                await EditContactModal.Hide();                
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

        protected virtual async Task OnTitleChangedAsync(string? title)
        {
            Filter.Title = title;
            await SearchAsync();
        }
        protected virtual async Task OnNameChangedAsync(string? name)
        {
            Filter.Name = name;
            await SearchAsync();
        }
        protected virtual async Task OnSurnameChangedAsync(string? surname)
        {
            Filter.Surname = surname;
            await SearchAsync();
        }
        protected virtual async Task OnConfidentialNameChangedAsync(string? confidentialName)
        {
            Filter.ConfidentialName = confidentialName;
            await SearchAsync();
        }
        protected virtual async Task OnJobRoleChangedAsync(string? jobRole)
        {
            Filter.JobRole = jobRole;
            await SearchAsync();
        }
        protected virtual async Task OnMailInfoChangedAsync(string? mailInfo)
        {
            Filter.MailInfo = mailInfo;
            await SearchAsync();
        }
        protected virtual async Task OnPhoneInfoChangedAsync(string? phoneInfo)
        {
            Filter.PhoneInfo = phoneInfo;
            await SearchAsync();
        }
        protected virtual async Task OnAddressInfoChangedAsync(string? addressInfo)
        {
            Filter.AddressInfo = addressInfo;
            await SearchAsync();
        }
        protected virtual async Task OnTagChangedAsync(string? tag)
        {
            Filter.Tag = tag;
            await SearchAsync();
        }
        







    }
}
