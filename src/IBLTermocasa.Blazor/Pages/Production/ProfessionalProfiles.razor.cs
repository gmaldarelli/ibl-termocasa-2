using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using IBLTermocasa.Blazor.Components.ProfessionalProfile;
using IBLTermocasa.Permissions;
using IBLTermocasa.ProfessionalProfiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using NUglify.Helpers;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;


namespace IBLTermocasa.Blazor.Pages.Production
{
    public partial class ProfessionalProfiles
    {
        protected List<BreadcrumbItem> BreadcrumbItems = new();
        protected PageToolbar Toolbar { get; } = new();
        private IReadOnlyList<ProfessionalProfileDto> ProfessionalProfileList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateProfessionalProfile { get; set; }
        private bool CanEditProfessionalProfile { get; set; }
        private bool CanDeleteProfessionalProfile { get; set; }
        private ProfessionalProfileCreateDto NewProfessionalProfile { get; set; }

        private ProfessionalProfileUpdateDto EditingProfessionalProfile { get; set; }
        private GetProfessionalProfilesInput Filter { get; set; }
        private ProfessionalProfileDto? SelectedProfessionalProfile;
        private bool isAttributeModalOpen;
        private string _searchString;
        private MudDataGrid<ProfessionalProfileDto> ProfessionalProfileMudDataGrid { get; set; } = new();
        [Inject] public IDialogService DialogService { get; set; }
        
        public ProfessionalProfiles()
        {
            NewProfessionalProfile = new ProfessionalProfileCreateDto();
            EditingProfessionalProfile = new ProfessionalProfileUpdateDto();
            Filter = new GetProfessionalProfilesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            ProfessionalProfileList = new List<ProfessionalProfileDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetProfessionalProfilesAsync();
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
            BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:ProfessionalProfiles"]));
            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateProfessionalProfile = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.ProfessionalProfiles.Create);
            CanEditProfessionalProfile = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.ProfessionalProfiles.Edit);
            CanDeleteProfessionalProfile = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.ProfessionalProfiles.Delete);
        }

        private async Task GetProfessionalProfilesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await ProfessionalProfilesAppService.GetListAsync(Filter);
            ProfessionalProfileList = result.Items;
            TotalCount = (int)result.TotalCount;
        }

        private async void SearchAsync(string filterText)
        {
            _searchString = filterText;
            if ((_searchString.IsNullOrEmpty() || _searchString.Length < 3) &&
                ProfessionalProfileMudDataGrid.Items != null && ProfessionalProfileMudDataGrid.Items.Any())
            {
                return;
            }

            await LoadGridData(new GridState<ProfessionalProfileDto>
            {
                Page = 0,
                PageSize = PageSize,
                SortDefinitions = ProfessionalProfileMudDataGrid.SortDefinitions.Values.ToList()
            });
            await ProfessionalProfileMudDataGrid.ReloadServerData();
            StateHasChanged();
        }

        private async Task<GridData<ProfessionalProfileDto>> LoadGridData(GridState<ProfessionalProfileDto> state)
        {
            state.SortDefinitions.ForEach(sortDef =>
            {
                CurrentSorting = sortDef.Descending ? $" {sortDef.SortBy} DESC" : $" {sortDef.SortBy} ";
            });
            Filter.SkipCount = state.Page * state.PageSize;
            Filter.Sorting = CurrentSorting;
            Filter.MaxResultCount = state.PageSize;
            Filter.FilterText = _searchString;
            var firstOrDefault = ProfessionalProfileMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(ProfessionalProfileDto.Name) });
            if (firstOrDefault != null)
            {
                Filter.Name = (string?)firstOrDefault.Value;
            }

            var firstOrDefault1 = ProfessionalProfileMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(ProfessionalProfileDto.StandardPrice) });
            if (firstOrDefault1 != null)
            {
                Filter.StandardPrice = (double?)firstOrDefault1.Value;
            }

            var result = await ProfessionalProfilesAppService.GetListAsync(Filter);
            ProfessionalProfileList = result.Items;
            GridData<ProfessionalProfileDto> data = new()
            {
                Items = ProfessionalProfileList,
                TotalItems = (int)result.TotalCount
            };
            return data;
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await ProfessionalProfilesAppService.GetDownloadTokenAsync()).Token;
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
                $"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/professional-profiles/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Name={HttpUtility.UrlEncode(Filter.Name)}&StandardPriceMin={Filter.StandardPriceMin}&StandardPriceMax={Filter.StandardPriceMax}",
                forceLoad: true);
        }

        private async Task DeleteProfessionalProfileAsync(ProfessionalProfileDto input)
        {
            await ProfessionalProfilesAppService.DeleteAsync(input.Id);
            await GetProfessionalProfilesAsync();
            await ProfessionalProfileMudDataGrid.ReloadServerData();
        }

        private async Task OpenNewOrReadOrEditProfessionalProfile(ProfessionalProfileDto input, bool isReadOnly, bool isNew)
        {
            var parameters = new DialogParameters
            {
                { "ProfessionalProfile", input },
                { "DisplayReadOnly", isReadOnly },
                { "IsNew", isNew }
            };

            var dialog = await DialogService.ShowAsync<ProfessionalProfileInput>(L["ProfessionalProfile"], parameters,
                new DialogOptions
                {
                    Position = DialogPosition.Custom,
                    FullWidth = true,
                    MaxWidth = MaxWidth.Small
                });

            var result = await dialog.Result;
            if (!result.Canceled)
            {
                List<ProfessionalProfileDto> _tempList = new List<ProfessionalProfileDto>();
                ProfessionalProfileList.ForEach(professionalProfileDto =>
                {
                    if (professionalProfileDto.Id == input.Id)
                    {
                        _tempList.Add((ProfessionalProfileDto)result.Data);
                    }
                    else
                    {
                        _tempList.Add(professionalProfileDto);
                    }
                });
                ProfessionalProfileList = _tempList;
                await ProfessionalProfileMudDataGrid.ReloadServerData();
                StateHasChanged();
            }
        }
    }
}