using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using IBLTermocasa.Blazor.Components;
using IBLTermocasa.Contacts;
using IBLTermocasa.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using NUglify.Helpers;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;

namespace IBLTermocasa.Blazor.Pages.Crm
{
    public partial class Contacts
    {
        protected List<BreadcrumbItem> BreadcrumbItems = new();
        protected PageToolbar Toolbar { get; } = new();
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
        private ContactDto? SelectedContact;
        private MudDataGrid<ContactDto> ContactMudDataGrid { get; set; } = new();
        private string _searchString;

        [Inject] private SecureConfirmationService _SecureConfirmationService { get; set; }


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
                StateHasChanged();
            }
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:Contacts"]));
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

        private async Task DownloadAsExcelAsync()
        {
            var token = (await ContactsAppService.GetDownloadTokenAsync()).Token;
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
                $"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/contacts/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}{culture}&Title={Filter.Title}&Name={Filter.Name}&Surname={Filter.Surname}&Phone={Filter.PhoneInfo}&Mail={Filter.MailInfo}&ConfidentialName={Filter.ConfidentialName}&JobRole={Filter.JobRole}&Tag={Filter.Tag}",
                forceLoad: true);
        }

        private Task OpenCreateContactModalAsync()
        {
            ContactInput = new ContactDto()
            {
            };

            SelectedContact = ContactInput;
            NavigationManager.NavigateTo($"/contact/{Guid.Empty}");
            return Task.CompletedTask;
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

            StateHasChanged();
        }


        private async void SearchAsync(string filterText)
        {
            _searchString = filterText;
            if ((_searchString.IsNullOrEmpty() || _searchString.Length < 3) &&
                ContactMudDataGrid.Items != null && ContactMudDataGrid.Items.Any())
            {
                return;
            }

            await LoadGridData(new GridState<ContactDto>
            {
                Page = 0,
                PageSize = PageSize,
                SortDefinitions = ContactMudDataGrid.SortDefinitions.Values.ToList()
            });
            await ContactMudDataGrid.ReloadServerData();
            StateHasChanged();
        }

        private async Task<GridData<ContactDto>> LoadGridData(GridState<ContactDto> state)
        {
            state.SortDefinitions.ForEach(sortDef =>
            {
                CurrentSorting = sortDef.Descending ? $" {sortDef.SortBy} DESC" : $" {sortDef.SortBy} ";
            });
            Filter.SkipCount = state.Page * state.PageSize;
            Filter.Sorting = CurrentSorting;
            Filter.MaxResultCount = state.PageSize;
            Filter.FilterText = _searchString;
            var firstOrDefault = ContactMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(ContactDto.Title) });
            if (firstOrDefault != null)
            {
                Filter.Title = (string?)firstOrDefault.Value;
            }

            var firstOrDefault1 = ContactMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(ContactDto.Name) });
            if (firstOrDefault1 != null)
            {
                Filter.Name = (string?)firstOrDefault1.Value;
            }

            var firstOrDefault2 = ContactMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(ContactDto.Surname) });
            if (firstOrDefault2 != null)
            {
                Filter.Surname = (string)firstOrDefault2.Value!;
            }

            var firstOrDefault3 = ContactMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(ContactDto.ConfidentialName) });
            if (firstOrDefault3 != null)
            {
                Filter.ConfidentialName = (string)firstOrDefault3.Value!;
            }

            var firstOrDefault4 = ContactMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(ContactDto.JobRole) });
            if (firstOrDefault4 != null)
            {
                Filter.JobRole = (string)firstOrDefault4.Value!;
            }

            var firstOrDefault5 = ContactMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(ContactDto.Phones) });
            if (firstOrDefault5 != null)
            {
                Filter.PhoneInfo = (string)firstOrDefault5.Value!;
            }

            var firstOrDefault6 = ContactMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(ContactDto.Emails) });
            if (firstOrDefault6 != null)
            {
                Filter.MailInfo = (string)firstOrDefault6.Value!;
            }

            var result = await ContactsAppService.GetListAsync(Filter);
            ContactList = result.Items;
            GridData<ContactDto> data = new()
            {
                Items = ContactList,
                TotalItems = (int)result.TotalCount
            };
            return data;
        }
    }
}