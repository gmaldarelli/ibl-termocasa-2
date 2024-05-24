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
using IBLTermocasa.Organizations;
using IBLTermocasa.Permissions;
using IBLTermocasa.Shared;

using IBLTermocasa.Types;



namespace IBLTermocasa.Blazor.Pages
{
    public partial class Organizations
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<OrganizationWithNavigationPropertiesDto> OrganizationList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateOrganization { get; set; }
        private bool CanEditOrganization { get; set; }
        private bool CanDeleteOrganization { get; set; }
        private OrganizationCreateDto NewOrganization { get; set; }
        private Validations NewOrganizationValidations { get; set; } = new();
        private OrganizationUpdateDto EditingOrganization { get; set; }
        private Validations EditingOrganizationValidations { get; set; } = new();
        private Guid EditingOrganizationId { get; set; }
        private Modal CreateOrganizationModal { get; set; } = new();
        private Modal EditOrganizationModal { get; set; } = new();
        private GetOrganizationsInput Filter { get; set; }
        private DataGridEntityActionsColumn<OrganizationWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "organization-create-tab";
        protected string SelectedEditTab = "organization-edit-tab";
        private OrganizationWithNavigationPropertiesDto? SelectedOrganization;
        private IReadOnlyList<LookupDto<Guid>> IndustriesCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        
        public Organizations()
        {
            NewOrganization = new OrganizationCreateDto();
            EditingOrganization = new OrganizationUpdateDto();
            Filter = new GetOrganizationsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            OrganizationList = new List<OrganizationWithNavigationPropertiesDto>();
            
            
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
            OrganizationList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            
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
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/organizations/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Code={HttpUtility.UrlEncode(Filter.Code)}&Name={HttpUtility.UrlEncode(Filter.Name)}&OrganizationType={Filter.OrganizationType}&MailInfo={HttpUtility.UrlEncode(Filter.MailInfo)}&PhoneInfo={HttpUtility.UrlEncode(Filter.PhoneInfo)}&Tags={HttpUtility.UrlEncode(Filter.Tags)}&IndustryId={Filter.IndustryId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<OrganizationWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetOrganizationsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateOrganizationModalAsync()
        {
            NewOrganization = new OrganizationCreateDto{
                
                IndustryId = IndustriesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await NewOrganizationValidations.ClearAll();
            await CreateOrganizationModal.Show();
        }

        private async Task CloseCreateOrganizationModalAsync()
        {
            NewOrganization = new OrganizationCreateDto{
                
                IndustryId = IndustriesCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateOrganizationModal.Hide();
        }

        private async Task OpenEditOrganizationModalAsync(OrganizationWithNavigationPropertiesDto input)
        {
            var organization = await OrganizationsAppService.GetWithNavigationPropertiesAsync(input.Organization.Id);
            
            EditingOrganizationId = organization.Organization.Id;
            EditingOrganization = ObjectMapper.Map<OrganizationDto, OrganizationUpdateDto>(organization.Organization);
            await EditingOrganizationValidations.ClearAll();
            await EditOrganizationModal.Show();
        }

        private async Task DeleteOrganizationAsync(OrganizationWithNavigationPropertiesDto input)
        {
            await OrganizationsAppService.DeleteAsync(input.Organization.Id);
            await GetOrganizationsAsync();
        }

        private async Task CreateOrganizationAsync()
        {
            try
            {
                if (await NewOrganizationValidations.ValidateAll() == false)
                {
                    return;
                }

                await OrganizationsAppService.CreateAsync(NewOrganization);
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
                if (await EditingOrganizationValidations.ValidateAll() == false)
                {
                    return;
                }

                await OrganizationsAppService.UpdateAsync(EditingOrganizationId, EditingOrganization);
                await GetOrganizationsAsync();
                await EditOrganizationModal.Hide();                
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
