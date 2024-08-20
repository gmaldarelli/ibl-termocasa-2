using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Blazorise;
using IBLTermocasa.Interactions;
using IBLTermocasa.Permissions;
using IBLTermocasa.Shared;
using IBLTermocasa.Types;
using Microsoft.AspNetCore.Authorization;
using MudBlazor;
using NUglify.Helpers;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;

namespace IBLTermocasa.Blazor.Pages.Crm
{
    public partial class Interactions
    {
        protected List<BreadcrumbItem> BreadcrumbItems = new();
        protected PageToolbar Toolbar { get; } = new();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<InteractionDto> InteractionList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateInteraction { get; set; }
        private bool CanEditInteraction { get; set; }
        private bool CanDeleteInteraction { get; set; }
        private InteractionCreateDto NewInteraction { get; set; }
        private Validations NewInteractionValidations { get; set; } = new();
        private InteractionUpdateDto EditingInteraction { get; set; }
        private Validations EditingInteractionValidations { get; set; } = new();
        private Guid EditingInteractionId { get; set; }
        private Modal CreateInteractionModal { get; set; } = new();
        private Modal EditInteractionModal { get; set; } = new();
        private GetInteractionsInput Filter { get; set; }
        private InteractionWithNavigationPropertiesDto? SelectedInteraction;
        private IReadOnlyList<LookupDto<Guid>> IdentityUsersCollection { get; set; } = new List<LookupDto<Guid>>();
        private IReadOnlyList<LookupDto<Guid>> OrganizationUnitsCollection { get; set; } = new List<LookupDto<Guid>>();
        private MudDataGrid<InteractionDto> InteractionMudDataGrid { get; set; } = new();
        private string _searchString;


        public Interactions()
        {
            NewInteraction = new InteractionCreateDto();
            EditingInteraction = new InteractionUpdateDto();
            Filter = new GetInteractionsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            InteractionList = new List<InteractionDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetIdentityUserCollectionLookupAsync();
            await GetOrganizationUnitCollectionLookupAsync();
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
            BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:Interactions"]));
            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateInteraction = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.Interactions.Create);
            CanEditInteraction = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.Interactions.Edit);
            CanDeleteInteraction = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.Interactions.Delete);
        }

        private async Task GetInteractionsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await InteractionsAppService.GetListAsync(Filter);

            var temp = new List<InteractionDto>();
            result.Items.ForEach(item => { temp.Add(item.Interaction); });
            InteractionList = temp;
            TotalCount = (int)result.TotalCount;
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await InteractionsAppService.GetDownloadTokenAsync()).Token;
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
                $"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/interactions/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&InteractionType={Filter.InteractionType}&InteractionDateMin={Filter.InteractionDateMin?.ToString("O")}&InteractionDateMax={Filter.InteractionDateMax?.ToString("O")}&Content={HttpUtility.UrlEncode(Filter.Content)}&ReferenceObject={HttpUtility.UrlEncode(Filter.ReferenceObject)}&WriterNotes={HttpUtility.UrlEncode(Filter.WriterNotes)}&WriterUserId={Filter.WriterUserId}&IdentityUserId={Filter.IdentityUserId}",
                forceLoad: true);
        }

        private async Task OpenCreateInteractionModalAsync()
        {
            NewInteraction = new InteractionCreateDto
            {
                InteractionDate = DateTime.Now,

                WriterUserId = IdentityUsersCollection.Select(i => i.Id).FirstOrDefault(),
            };
            await NewInteractionValidations.ClearAll();
            await CreateInteractionModal.Show();
        }

        private async Task CloseCreateInteractionModalAsync()
        {
            NewInteraction = new InteractionCreateDto
            {
                InteractionDate = DateTime.Now,

                WriterUserId = IdentityUsersCollection.Select(i => i.Id).FirstOrDefault(),
            };
            await CreateInteractionModal.Hide();
        }

        private async Task OpenEditInteractionModalAsync(InteractionDto input)
        {
            var interaction = await InteractionsAppService.GetWithNavigationPropertiesAsync(input.Id);

            EditingInteractionId = interaction.Interaction.Id;
            EditingInteraction = ObjectMapper.Map<InteractionDto, InteractionUpdateDto>(interaction.Interaction);
            await EditingInteractionValidations.ClearAll();
            await EditInteractionModal.Show();
        }

        private async Task DeleteInteractionAsync(InteractionDto input)
        {
            await InteractionsAppService.DeleteAsync(input.Id);
            await InteractionMudDataGrid.ReloadServerData();
            await GetInteractionsAsync();
        }

        private async Task CreateInteractionAsync()
        {
            try
            {
                if (await NewInteractionValidations.ValidateAll() == false)
                {
                    return;
                }

                await InteractionsAppService.CreateAsync(NewInteraction);
                await GetInteractionsAsync();
                await InteractionMudDataGrid.ReloadServerData();
                await CloseCreateInteractionModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditInteractionModalAsync()
        {
            await EditInteractionModal.Hide();
        }

        private async Task UpdateInteractionAsync()
        {
            try
            {
                if (await EditingInteractionValidations.ValidateAll() == false)
                {
                    return;
                }

                await InteractionsAppService.UpdateAsync(EditingInteractionId, EditingInteraction);
                await GetInteractionsAsync();
                await InteractionMudDataGrid.ReloadServerData();
                await EditInteractionModal.Hide();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task GetIdentityUserCollectionLookupAsync(string? newValue = null)
        {
            IdentityUsersCollection =
                (await InteractionsAppService.GetIdentityUserLookupAsync(new LookupRequestDto { Filter = newValue }))
                .Items;
        }

        private async Task GetOrganizationUnitCollectionLookupAsync(string? newValue = null)
        {
            OrganizationUnitsCollection =
                (await InteractionsAppService.GetOrganizationUnitLookupAsync(new LookupRequestDto
                    { Filter = newValue })).Items;
        }


        private async void SearchAsync(string filterText)
        {
            _searchString = filterText;
            if ((_searchString.IsNullOrEmpty() || _searchString.Length < 3) &&
                InteractionMudDataGrid.Items != null && InteractionMudDataGrid.Items.Any())
            {
                return;
            }

            await LoadGridData(new GridState<InteractionDto>
            {
                Page = 0,
                PageSize = PageSize,
                SortDefinitions = InteractionMudDataGrid.SortDefinitions.Values.ToList()
            });
            await InteractionMudDataGrid.ReloadServerData();
            StateHasChanged();
        }

        private async Task<GridData<InteractionDto>> LoadGridData(GridState<InteractionDto> state)
        {
            state.SortDefinitions.ForEach(sortDef =>
            {
                CurrentSorting = sortDef.Descending ? $" {sortDef.SortBy} DESC" : $" {sortDef.SortBy} ";
            });
            Filter.SkipCount = state.Page * state.PageSize;
            Filter.Sorting = CurrentSorting;
            Filter.MaxResultCount = state.PageSize;
            Filter.FilterText = _searchString;
            var firstOrDefault = InteractionMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(InteractionDto.InteractionType) });
            if (firstOrDefault != null)
            {
                Filter.InteractionType = (InteractionType?)firstOrDefault.Value;
            }

            var firstOrDefault1 = InteractionMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(InteractionDto.InteractionDate) });
            if (firstOrDefault1 != null)
            {
                Filter.InteractionDateMin = (DateTime?)firstOrDefault1.Value;
            }

            var firstOrDefault2 = InteractionMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(InteractionDto.ReferenceObject) });
            if (firstOrDefault2 != null)
            {
                Filter.ReferenceObject = (string)firstOrDefault2.Value!;
            }

            var firstOrDefault3 = InteractionMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(InteractionDto.WriterUserId) });
            if (firstOrDefault3 != null)
            {
                Filter.WriterUserId = (Guid)firstOrDefault3.Value!;
            }

            var firstOrDefault4 = InteractionMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(InteractionDto.IdentityUserId) });
            if (firstOrDefault4 != null)
            {
                Filter.IdentityUserId = (Guid)firstOrDefault4.Value!;
            }

            var result = await InteractionsAppService.GetListAsync(Filter);
            var temp = new List<InteractionDto>();
            result.Items.ForEach(item => { temp.Add(item.Interaction); });
            InteractionList = temp;
            GridData<InteractionDto> data = new()
            {
                Items = InteractionList,
                TotalItems = (int)result.TotalCount
            };
            return data;
        }
    }
}