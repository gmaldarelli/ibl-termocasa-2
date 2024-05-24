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
using IBLTermocasa.Interactions;
using IBLTermocasa.Permissions;
using IBLTermocasa.Shared;

using IBLTermocasa.Types;



namespace IBLTermocasa.Blazor.Pages
{
    public partial class Interactions
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<InteractionWithNavigationPropertiesDto> InteractionList { get; set; }
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
        private DataGridEntityActionsColumn<InteractionWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "interaction-create-tab";
        protected string SelectedEditTab = "interaction-edit-tab";
        private InteractionWithNavigationPropertiesDto? SelectedInteraction;
        private IReadOnlyList<LookupDto<Guid>> IdentityUsersCollection { get; set; } = new List<LookupDto<Guid>>();
private IReadOnlyList<LookupDto<Guid>> OrganizationUnitsCollection { get; set; } = new List<LookupDto<Guid>>();

        
        
        
        
        
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
            InteractionList = new List<InteractionWithNavigationPropertiesDto>();
            
            
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
                await SetToolbarItemsAsync();
                StateHasChanged();
            }
        }  

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Interactions"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewInteraction"], async () =>
            {
                await OpenCreateInteractionModalAsync();
            }, IconName.Add, requiredPolicyName: IBLTermocasaPermissions.Interactions.Create);

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
            InteractionList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetInteractionsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await InteractionsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("IBLTermocasa") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/interactions/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&InteractionType={Filter.InteractionType}&InteractionDateMin={Filter.InteractionDateMin?.ToString("O")}&InteractionDateMax={Filter.InteractionDateMax?.ToString("O")}&Content={HttpUtility.UrlEncode(Filter.Content)}&ReferenceObject={HttpUtility.UrlEncode(Filter.ReferenceObject)}&WriterNotes={HttpUtility.UrlEncode(Filter.WriterNotes)}&WriterUserId={Filter.WriterUserId}&IdentityUserId={Filter.IdentityUserId}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<InteractionWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetInteractionsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateInteractionModalAsync()
        {
            NewInteraction = new InteractionCreateDto{
                InteractionDate = DateTime.Now,

                WriterUserId = IdentityUsersCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await NewInteractionValidations.ClearAll();
            await CreateInteractionModal.Show();
        }

        private async Task CloseCreateInteractionModalAsync()
        {
            NewInteraction = new InteractionCreateDto{
                InteractionDate = DateTime.Now,

                WriterUserId = IdentityUsersCollection.Select(i=>i.Id).FirstOrDefault(),

            };
            await CreateInteractionModal.Hide();
        }

        private async Task OpenEditInteractionModalAsync(InteractionWithNavigationPropertiesDto input)
        {
            var interaction = await InteractionsAppService.GetWithNavigationPropertiesAsync(input.Interaction.Id);
            
            EditingInteractionId = interaction.Interaction.Id;
            EditingInteraction = ObjectMapper.Map<InteractionDto, InteractionUpdateDto>(interaction.Interaction);
            await EditingInteractionValidations.ClearAll();
            await EditInteractionModal.Show();
        }

        private async Task DeleteInteractionAsync(InteractionWithNavigationPropertiesDto input)
        {
            await InteractionsAppService.DeleteAsync(input.Interaction.Id);
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
                await EditInteractionModal.Hide();                
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

        protected virtual async Task OnInteractionTypeChangedAsync(InteractionType? interactionType)
        {
            Filter.InteractionType = interactionType;
            await SearchAsync();
        }
        protected virtual async Task OnInteractionDateMinChangedAsync(DateTime? interactionDateMin)
        {
            Filter.InteractionDateMin = interactionDateMin.HasValue ? interactionDateMin.Value.Date : interactionDateMin;
            await SearchAsync();
        }
        protected virtual async Task OnInteractionDateMaxChangedAsync(DateTime? interactionDateMax)
        {
            Filter.InteractionDateMax = interactionDateMax.HasValue ? interactionDateMax.Value.Date.AddDays(1).AddSeconds(-1) : interactionDateMax;
            await SearchAsync();
        }
        protected virtual async Task OnContentChangedAsync(string? content)
        {
            Filter.Content = content;
            await SearchAsync();
        }
        protected virtual async Task OnReferenceObjectChangedAsync(string? referenceObject)
        {
            Filter.ReferenceObject = referenceObject;
            await SearchAsync();
        }
        protected virtual async Task OnWriterNotesChangedAsync(string? writerNotes)
        {
            Filter.WriterNotes = writerNotes;
            await SearchAsync();
        }
        protected virtual async Task OnWriterUserIdChangedAsync(Guid? writerUserId)
        {
            Filter.WriterUserId = writerUserId;
            await SearchAsync();
        }
        protected virtual async Task OnIdentityUserIdChangedAsync(Guid? identityUserId)
        {
            Filter.IdentityUserId = identityUserId;
            await SearchAsync();
        }
        

        private async Task GetIdentityUserCollectionLookupAsync(string? newValue = null)
        {
            IdentityUsersCollection = (await InteractionsAppService.GetIdentityUserLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private async Task GetOrganizationUnitCollectionLookupAsync(string? newValue = null)
        {
            OrganizationUnitsCollection = (await InteractionsAppService.GetOrganizationUnitLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }







    }
}
