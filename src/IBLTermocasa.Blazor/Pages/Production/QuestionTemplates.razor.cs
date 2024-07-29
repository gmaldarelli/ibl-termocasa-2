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
using IBLTermocasa.QuestionTemplates;
using IBLTermocasa.Permissions;
using IBLTermocasa.Shared;

using IBLTermocasa.Types;
using MudBlazor;
using NUglify.Helpers;
using SortDirection = Blazorise.SortDirection;


namespace IBLTermocasa.Blazor.Pages.Production
{
    public partial class QuestionTemplates
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<QuestionTemplateDto> QuestionTemplateList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateQuestionTemplate { get; set; }
        private bool CanEditQuestionTemplate { get; set; }
        private bool CanDeleteQuestionTemplate { get; set; }
        private QuestionTemplateCreateDto NewQuestionTemplate { get; set; }
        private Validations NewQuestionTemplateValidations { get; set; } = new();
        private QuestionTemplateUpdateDto EditingQuestionTemplate { get; set; }
        private Validations EditingQuestionTemplateValidations { get; set; } = new();
        private Guid EditingQuestionTemplateId { get; set; }
        private Modal CreateQuestionTemplateModal { get; set; } = new();
        private Modal EditQuestionTemplateModal { get; set; } = new();
        private GetQuestionTemplatesInput Filter { get; set; }
        private QuestionTemplateDto? SelectedQuestionTemplate;
        private MudDataGrid<QuestionTemplateDto> QuestionTemplateMudDataGrid { get; set; }
        private string _searchString = string.Empty;
        
        
        
        
        
        public QuestionTemplates()
        {
            NewQuestionTemplate = new QuestionTemplateCreateDto();
            EditingQuestionTemplate = new QuestionTemplateUpdateDto();
            Filter = new GetQuestionTemplatesInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            QuestionTemplateList = new List<QuestionTemplateDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetQuestionTemplatesAsync();
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:QuestionTemplates"]));
            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateQuestionTemplate = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.QuestionTemplates.Create);
            CanEditQuestionTemplate = await AuthorizationService
                            .IsGrantedAsync(IBLTermocasaPermissions.QuestionTemplates.Edit);
            CanDeleteQuestionTemplate = await AuthorizationService
                            .IsGrantedAsync(IBLTermocasaPermissions.QuestionTemplates.Delete);
                            
                            
        }

        private async Task GetQuestionTemplatesAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await QuestionTemplatesAppService.GetListAsync(Filter);
            QuestionTemplateList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await QuestionTemplatesAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("IBLTermocasa") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/question-templates/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Code={HttpUtility.UrlEncode(Filter.Code)}&QuestionText={HttpUtility.UrlEncode(Filter.QuestionText)}&AnswerType={Filter.AnswerType}&ChoiceValue={HttpUtility.UrlEncode(Filter.ChoiceValue)}", forceLoad: true);
        }
        private async Task OpenCreateQuestionTemplateModalAsync()
        {
            NewQuestionTemplate = new QuestionTemplateCreateDto{
                
                
            };
            await NewQuestionTemplateValidations.ClearAll();
            await CreateQuestionTemplateModal.Show();
        }

        private async Task CloseCreateQuestionTemplateModalAsync()
        {
            NewQuestionTemplate = new QuestionTemplateCreateDto{
                
                
            };
            await CreateQuestionTemplateModal.Hide();
        }

        private async Task OpenEditQuestionTemplateModalAsync(QuestionTemplateDto input)
        {
            var questionTemplate = await QuestionTemplatesAppService.GetAsync(input.Id);
            
            EditingQuestionTemplateId = questionTemplate.Id;
            EditingQuestionTemplate = ObjectMapper.Map<QuestionTemplateDto, QuestionTemplateUpdateDto>(questionTemplate);
            await EditingQuestionTemplateValidations.ClearAll();
            await EditQuestionTemplateModal.Show();
        }

        private async Task DeleteQuestionTemplateAsync(QuestionTemplateDto input)
        {
            await QuestionTemplatesAppService.DeleteAsync(input.Id);
            await GetQuestionTemplatesAsync();
        }

        private async Task CreateQuestionTemplateAsync()
        {
            try
            {
                if (await NewQuestionTemplateValidations.ValidateAll() == false)
                {
                    return;
                }

                await QuestionTemplatesAppService.CreateAsync(NewQuestionTemplate);
                await GetQuestionTemplatesAsync();
                await CloseCreateQuestionTemplateModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditQuestionTemplateModalAsync()
        {
            await EditQuestionTemplateModal.Hide();
        }

        private async Task UpdateQuestionTemplateAsync()
        {
            try
            {
                if (await EditingQuestionTemplateValidations.ValidateAll() == false)
                {
                    return;
                }

                await QuestionTemplatesAppService.UpdateAsync(EditingQuestionTemplateId, EditingQuestionTemplate);
                await GetQuestionTemplatesAsync();
                await EditQuestionTemplateModal.Hide();                
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        
        private async void SearchAsync(string filterText)
        {
            _searchString = filterText;
            if ((_searchString.IsNullOrEmpty() || _searchString.Length < 3) &&
                QuestionTemplateMudDataGrid.Items != null && QuestionTemplateMudDataGrid.Items.Any())
            {
                return;
            }

            await LoadGridData(new GridState<QuestionTemplateDto>
            {
                Page = 0,
                PageSize = PageSize,
                SortDefinitions = QuestionTemplateMudDataGrid.SortDefinitions.Values.ToList()
            });
            await QuestionTemplateMudDataGrid.ReloadServerData();
            StateHasChanged();
        }

        private async Task<GridData<QuestionTemplateDto>> LoadGridData(GridState<QuestionTemplateDto> state)
        {
            state.SortDefinitions.ForEach(sortDef =>
            {
                CurrentSorting = sortDef.Descending ? $" {sortDef.SortBy} DESC" : $" {sortDef.SortBy} ";
            });
            Filter.SkipCount = state.Page * state.PageSize;
            Filter.Sorting = CurrentSorting;
            Filter.MaxResultCount = state.PageSize;
            Filter.FilterText = _searchString;
            var firstOrDefault = QuestionTemplateMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(QuestionTemplateDto.Code) });
            if (firstOrDefault != null)
            {
                Filter.Code = (string?)firstOrDefault.Value;
            }

            var firstOrDefault1 = QuestionTemplateMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(QuestionTemplateDto.QuestionText) });
            if (firstOrDefault1 != null)
            {
                Filter.QuestionText = (string?)firstOrDefault1.Value;
            }

            var firstOrDefault2 = QuestionTemplateMudDataGrid.FilterDefinitions.FirstOrDefault(x =>
                x.Column is { PropertyName: nameof(QuestionTemplateDto.AnswerType) });
            if (firstOrDefault2 != null)
            {
                Filter.AnswerType = (AnswerType?)firstOrDefault2.Value!;
            }

            var result = await QuestionTemplatesAppService.GetListAsync(Filter);
            QuestionTemplateList = result.Items;
            GridData<QuestionTemplateDto> data = new()
            {
                Items = QuestionTemplateList,
                TotalItems = (int)result.TotalCount
            };
            return data;
        }
    }
}
