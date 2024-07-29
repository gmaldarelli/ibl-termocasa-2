using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using IBLTermocasa.Industries;
using IBLTermocasa.Permissions;
using Microsoft.AspNetCore.Authorization;
using MudBlazor;
using NUglify.Helpers;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.BlazoriseUI.Components;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;

namespace IBLTermocasa.Blazor.Pages.Crm
{
    public partial class Industries
    {
        protected List<BreadcrumbItem> BreadcrumbItems = new();
        protected PageToolbar Toolbar { get; } = new();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<IndustryDto> IndustryList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateIndustry { get; set; }
        private bool CanEditIndustry { get; set; }
        private bool CanDeleteIndustry { get; set; }
        private IndustryCreateDto NewIndustry { get; set; }
        private Validations NewIndustryValidations { get; set; } = new();
        private IndustryUpdateDto EditingIndustry { get; set; }
        private Validations EditingIndustryValidations { get; set; } = new();
        private Guid EditingIndustryId { get; set; }
        private Modal CreateIndustryModal { get; set; } = new();
        private Modal EditIndustryModal { get; set; } = new();
        private GetIndustriesInput Filter { get; set; }
        private IndustryDto? SelectedIndustry;
        private MudDataGrid<IndustryDto> IndustryMudDataGrid { get; set; }
        private string _searchString;
        private List<IndustryDto> SelectedIndustries { get; set; } = new();
        private bool AllIndustriesSelected { get; set; }

        public Industries()
        {
            NewIndustry = new IndustryCreateDto();
            EditingIndustry = new IndustryUpdateDto();
            Filter = new GetIndustriesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            IndustryList = new List<IndustryDto>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetIndustriesAsync();
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
            BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:Industries"]));
            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateIndustry = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.Industries.Create);
            CanEditIndustry = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.Industries.Edit);
            CanDeleteIndustry = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.Industries.Delete);
        }

        private async Task GetIndustriesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await IndustriesAppService.GetListAsync(Filter);
            IndustryList = result.Items;
            TotalCount = (int)result.TotalCount;

            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetIndustriesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateIndustryModalAsync()
        {
            NewIndustry = new IndustryCreateDto
            {
            };
            await NewIndustryValidations.ClearAll();
            await CreateIndustryModal.Show();
        }

        private async Task CloseCreateIndustryModalAsync()
        {
            NewIndustry = new IndustryCreateDto
            {
            };
            await CreateIndustryModal.Hide();
        }

        private async Task OpenEditIndustryModalAsync(IndustryDto input)
        {
            var industry = await IndustriesAppService.GetAsync(input.Id);

            EditingIndustryId = industry.Id;
            EditingIndustry = ObjectMapper.Map<IndustryDto, IndustryUpdateDto>(industry);
            await EditingIndustryValidations.ClearAll();
            await EditIndustryModal.Show();
        }

        private async Task DeleteIndustryAsync(IndustryDto input)
        {
            await IndustriesAppService.DeleteAsync(input.Id);
            await GetIndustriesAsync();
        }

        private async Task CreateIndustryAsync()
        {
            try
            {
                if (await NewIndustryValidations.ValidateAll() == false)
                {
                    return;
                }

                await IndustriesAppService.CreateAsync(NewIndustry);
                await GetIndustriesAsync();
                await CloseCreateIndustryModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditIndustryModalAsync()
        {
            await EditIndustryModal.Hide();
        }

        private async Task UpdateIndustryAsync()
        {
            try
            {
                if (await EditingIndustryValidations.ValidateAll() == false)
                {
                    return;
                }

                await IndustriesAppService.UpdateAsync(EditingIndustryId, EditingIndustry);
                await GetIndustriesAsync();
                await EditIndustryModal.Hide();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private Task ClearSelection()
        {
            AllIndustriesSelected = false;
            SelectedIndustries.Clear();

            return Task.CompletedTask;
        }

        private async void SearchAsync(string filterText)
        {
            _searchString = filterText;
            if ((_searchString.IsNullOrEmpty() || _searchString.Length < 3) &&
                IndustryMudDataGrid.Items != null && IndustryMudDataGrid.Items.Any())
            {
                return;
            }

            await LoadGridData(new GridState<IndustryDto>
            {
                Page = 0,
                PageSize = PageSize,
                SortDefinitions = IndustryMudDataGrid.SortDefinitions.Values.ToList()
            });
            await IndustryMudDataGrid.ReloadServerData();
            StateHasChanged();
        }

        private async Task<GridData<IndustryDto>> LoadGridData(GridState<IndustryDto> state)
        {
            state.SortDefinitions.ForEach(sortDef =>
            {
                CurrentSorting = sortDef.Descending ? $" {sortDef.SortBy} DESC" : $" {sortDef.SortBy} ";
            });
            Filter.SkipCount = state.Page * state.PageSize;
            Filter.Sorting = CurrentSorting;
            Filter.MaxResultCount = state.PageSize;
            Filter.FilterText = _searchString;
            var firstOrDefault = IndustryMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(IndustryDto.Code) });
            if (firstOrDefault != null)
            {
                Filter.Code = (string?)firstOrDefault.Value;
            }

            var firstOrDefault1 = IndustryMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(IndustryDto.Code) });
            if (firstOrDefault1 != null)
            {
                Filter.Code = (string?)firstOrDefault1.Value;
            }

            var firstOrDefault2 = IndustryMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(IndustryDto.Description) });
            if (firstOrDefault2 != null)
            {
                Filter.Description = (string)firstOrDefault2.Value!;
            }

            var result = await IndustriesAppService.GetListAsync(Filter);
            IndustryList = result.Items;
            GridData<IndustryDto> data = new()
            {
                Items = IndustryList,
                TotalItems = (int)result.TotalCount
            };
            return data;
        }
    }
}