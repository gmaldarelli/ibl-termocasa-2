using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Blazorise.DataGrid;
using IBLTermocasa.Blazor.Components;
using IBLTermocasa.Contacts;
using IBLTermocasa.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;

namespace IBLTermocasa.Blazor.Pages.Crm
{
    public partial class Contacts
    {
        protected List<BreadcrumbItem> BreadcrumbItems = new List<BreadcrumbItem>();
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
        private ContactDto ContactInput { get; set; }
        private GetContactsInput Filter { get; set; }
        private DataGridEntityActionsColumn<ContactDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "contact-create-tab";
        protected string SelectedEditTab = "contact-edit-tab";
        private ContactDto? SelectedContact;
        [Inject]
        private SecureConfirmationService _SecureConfirmationService { get; set; }
        
        public Contacts()
        {
            ContactInput = new ContactDto();
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
            await GetContactsAsync();
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
            BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:Contacts"]));
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
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/contacts/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}{culture}&Title={Filter.Title}&Name={Filter.Name}&Surname={Filter.Surname}&Phone={Filter.PhoneInfo}&Mail={Filter.MailInfo}&ConfidentialName={Filter.ConfidentialName}&JobRole={Filter.JobRole}&Tag={Filter.Tag}", forceLoad: true);
        }

        private async Task OpenCreateContactModalAsync()
        {
            ContactInput = new ContactDto(){
            };
            
            SelectedContact = ContactInput;
            NavigationManager.NavigateTo($"/contact/{Guid.Empty}");
            //await CreateContactModal.Show();
        }
        private void OpenEditContactPageAsync(ContactDto input)
        {
            //navigate to the page ContactDetail
            SelectedContact = input;
            NavigationManager.NavigateTo($"/contact/{input.Id}");
        }
        private async Task DeleteContactAsync(ContactDto input)
        {
            
            bool result = await _SecureConfirmationService.ShowConfirmation(
                "Sei sicuro di voler eliminare questo contatto?",
                "Scrivi il nome dell contatto {0} per confermare l'eliminazione",
                input.Name
            );
            if (result)
            {
                // Procedi con la cancellazione
                Console.WriteLine("Cancellazione in corso organization id: " + input.Id);
                await ContactsAppService.DeleteAsync(input.Id);
                await GetContactsAsync();
            }
            else
            {
                // Cancellazione annullata
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>Cancellazione annullata...");
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
        protected virtual async Task OnPhoneChangedAsync(string? phone)
        {
            Filter.PhoneInfo = phone;
            await SearchAsync();
        }
        protected virtual async Task OnMailChangedAsync(string? mail)
        {
            Filter.MailInfo = mail;
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
        protected virtual async Task OnTagChangedAsync(string? tag)
        {
            Filter.Tag = tag;
            await SearchAsync();
        }
    }
}
