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



namespace IBLTermocasa.Blazor.Pages
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
        private DataGridEntityActionsColumn<QuestionTemplateDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "questionTemplate-create-tab";
        protected string SelectedEditTab = "questionTemplate-edit-tab";
        private QuestionTemplateDto? SelectedQuestionTemplate;
        
        
        
        
        
        
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:QuestionTemplates"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewQuestionTemplate"], async () =>
            {
                await OpenCreateQuestionTemplateModalAsync();
            }, IconName.Add, requiredPolicyName: IBLTermocasaPermissions.QuestionTemplates.Create);

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

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetQuestionTemplatesAsync();
            await InvokeAsync(StateHasChanged);
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

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<QuestionTemplateDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetQuestionTemplatesAsync();
            await InvokeAsync(StateHasChanged);
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
        protected virtual async Task OnQuestionTextChangedAsync(string? questionText)
        {
            Filter.QuestionText = questionText;
            await SearchAsync();
        }
        protected virtual async Task OnAnswerTypeChangedAsync(AnswerType? answerType)
        {
            Filter.AnswerType = answerType;
            await SearchAsync();
        }
        protected virtual async Task OnChoiceValueChangedAsync(string? choiceValue)
        {
            Filter.ChoiceValue = choiceValue;
            await SearchAsync();
        }
        







    }
}
