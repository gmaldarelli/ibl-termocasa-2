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
using IBLTermocasa.Industries;
using IBLTermocasa.Permissions;
using IBLTermocasa.Shared;



namespace IBLTermocasa.Blazor.Pages
{
    public partial class Industries
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
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
        private DataGridEntityActionsColumn<IndustryDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "industry-create-tab";
        protected string SelectedEditTab = "industry-edit-tab";
        private IndustryDto? SelectedIndustry;
        
        
        
        
        
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Industries"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            
            
            Toolbar.AddButton(L["NewIndustry"], async () =>
            {
                await OpenCreateIndustryModalAsync();
            }, IconName.Add, requiredPolicyName: IBLTermocasaPermissions.Industries.Create);

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

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<IndustryDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetIndustriesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateIndustryModalAsync()
        {
            NewIndustry = new IndustryCreateDto{
                
                
            };
            await NewIndustryValidations.ClearAll();
            await CreateIndustryModal.Show();
        }

        private async Task CloseCreateIndustryModalAsync()
        {
            NewIndustry = new IndustryCreateDto{
                
                
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
        protected virtual async Task OnDescriptionChangedAsync(string? description)
        {
            Filter.Description = description;
            await SearchAsync();
        }
        





        private Task SelectAllItems()
        {
            AllIndustriesSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllIndustriesSelected = false;
            SelectedIndustries.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedIndustryRowsChanged()
        {
            if (SelectedIndustries.Count != PageSize)
            {
                AllIndustriesSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedIndustriesAsync()
        {
            var message = AllIndustriesSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedIndustries.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllIndustriesSelected)
            {
                await IndustriesAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await IndustriesAppService.DeleteByIdsAsync(SelectedIndustries.Select(x => x.Id).ToList());
            }

            SelectedIndustries.Clear();
            AllIndustriesSelected = false;

            await GetIndustriesAsync();
        }


    }
}
