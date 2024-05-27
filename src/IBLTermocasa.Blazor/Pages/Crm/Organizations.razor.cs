using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Linq.Dynamic.Core;
using Blazorise;
using Blazorise.DataGrid;
using IBLTermocasa.Blazor.Components;
using IBLTermocasa.Industries;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using IBLTermocasa.Organizations;
using IBLTermocasa.Permissions;
using IBLTermocasa.Shared;
using IBLTermocasa.Types;
using Microsoft.AspNetCore.Components;
using NUglify.Helpers;


namespace IBLTermocasa.Blazor.Pages.Crm
{
    public partial class Organizations
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<OrganizationDto> OrganizationList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateOrganization { get; set; }
        private bool CanEditOrganization { get; set; }
        private bool CanDeleteOrganization { get; set; }
        private OrganizationDto OrganizationInput { get; set; }
        private Modal CreateOrganizationModal { get; set; } = new();
        private Modal EditOrganizationModal { get; set; } = new();
        private GetOrganizationsInput Filter { get; set; }
        private DataGridEntityActionsColumn<OrganizationDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "organization-create-tab";
        protected string SelectedEditTab = "organization-edit-tab";
        private OrganizationDto? SelectedOrganization;
        protected Progress progressRef;
        protected int progress;
        private IReadOnlyList<LookupDto<Guid>> IndustriesCollection { get; set; } = new List<LookupDto<Guid>>();

        [Inject]
        private IIndustriesAppService _industriesAppService { get; set; }
        [Inject]
        private SecureConfirmationService _SecureConfirmationService { get; set; }

        protected List<IndustryDto> Industries { get; set; } = new List<IndustryDto>();
        protected List<OrganizationType> OrganizationTypes {get; set;} = new List<OrganizationType>();
        
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
            await GetIndustryCollectionLookupAsync();
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Organizations"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            Toolbar.AddButton(L["NewOrganization"], async () =>
            {
                await OpenCreateOrganizationModalAsync();
            }, IconName.Add, requiredPolicyName: IBLTermocasaPermissions.Organizations.Create);

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
            result.Items.ForEach(item =>
            {
                temp.Add(item.Organization);
            });
            OrganizationList = temp;
            TotalCount = (int)result.TotalCount;
            await IndustriesAppService.GetListAsync(new GetIndustriesInput
            {
                MaxResultCount = 100,
                SkipCount = (1 - 1) * 100,
                Sorting = "Code"
            }).ContinueWith(task =>
            {
                Industries = task.Result.Items.ToList();
            });
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetOrganizationsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await OrganizationsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("IBLTermocasa") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/organizations/as-excel-file?DownloadToken={token}&FilterText={Filter.FilterText}{culture}&Name={Filter.Name}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<OrganizationDto> e)
        {
            progress = 25;
            await InvokeAsync( StateHasChanged );
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            progress = 50;
            await InvokeAsync( StateHasChanged );
            await GetOrganizationsAsync();
            progress = 75;
            await InvokeAsync( StateHasChanged );
            await InvokeAsync(StateHasChanged);
            progress = 100;
            await InvokeAsync( StateHasChanged );
            progress = 0;
        }

        private async Task OpenCreateOrganizationModalAsync()
        {
            OrganizationInput = new OrganizationDto(){};
            SelectedOrganization = OrganizationInput;
            NavigationManager.NavigateTo($"/organization/{Guid.Empty}");
            //await CreateOrganizationModal.Show();
        }

        private async Task CloseCreateOrganizationModalAsync()
        {
            
            
            OrganizationInput = new OrganizationDto();
            await CreateOrganizationModal.Hide();
        }
        private void OpenEditContactPageAsync(OrganizationDto input)
        {
            //navigate to the page ContactDetail
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
            }
            else
            {
                // Cancellazione annullata
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>Cancellazione annullata...");
            }
        }

        private async Task CreateOrganizationAsync()
        {
            try
            {
                await GetOrganizationsAsync();
                await CloseCreateOrganizationModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditOrganizationModalAsync()
        {
            await EditOrganizationModal.Hide();
        }

        private async Task UpdateOrganizationAsync()
        {
            try
            {
                await GetOrganizationsAsync();
                await EditOrganizationModal.Hide();                
            } catch (Exception ex)
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
        protected virtual async Task OnOrganizationTypeChangedAsync(OrganizationType? organizationType)
        {
            Filter.OrganizationType = organizationType;
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
        protected virtual async Task OnTagsChangedAsync(string? tags)
        {
            Filter.Tags = tags;
            await SearchAsync();
        }
        protected virtual async Task OnIndustryIdChangedAsync(Guid? industryId)
        {
            Filter.IndustryId = industryId;
            await SearchAsync();
        }
        

        private async Task GetIndustryCollectionLookupAsync(string? newValue = null)
        {
            IndustriesCollection = (await OrganizationsAppService.GetIndustryLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }
        
    }
}
