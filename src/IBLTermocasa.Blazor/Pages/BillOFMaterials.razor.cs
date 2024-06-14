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
using IBLTermocasa.BillOFMaterials;
using IBLTermocasa.Common;
using IBLTermocasa.Permissions;
using IBLTermocasa.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Volo.Abp.AspNetCore.Components.Web.Theming.Toolbars;
using Color = Blazorise.Color;
using SortDirection = Blazorise.SortDirection;


namespace IBLTermocasa.Blazor.Pages
{
    public partial class BillOFMaterials
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<BillOFMaterialDto> BillOFMaterialList { get; set; } = new List<BillOFMaterialDto>();
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateBillOFMaterial { get; set; }
        private bool CanEditBillOFMaterial { get; set; }
        private bool CanDeleteBillOFMaterial { get; set; }
        private GetBillOFMaterialsInput Filter { get; set; } = new();
        private DataGridEntityActionsColumn<BillOFMaterialDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "billOFMaterial-create-tab";
        protected string SelectedEditTab = "billOFMaterial-edit-tab";
        private BillOFMaterialDto? SelectedBillOFMaterial;
        
        
        
        
        
        private List<BillOFMaterialDto> SelectedBillOFMaterials { get; set; } = new();
        private bool AllBillOFMaterialsSelected { get; set; }
        
        public BillOFMaterials()
        {
            Filter = new GetBillOFMaterialsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            BillOFMaterialList = new List<BillOFMaterialDto>();
            
            
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:BillOFMaterials"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            

                
            ToolbarButton buttonNewRfq = new ToolbarButton()
            {
                Color = Color.Primary,
                Text = "Nuovi Preventivi",
                Disabled = false,
                Icon = IconName.Search,
                Clicked = new Func<Task>(SearchAsync),
            };
            
            RenderFragment renderFragment = (builder) =>
            {
                builder.OpenComponent<ToolbarButton>(0);
                builder.AddAttribute(1, "Color", buttonNewRfq.Color);
                builder.AddAttribute(2, "Text", buttonNewRfq.Text);
                builder.AddAttribute(2, "Disabled", buttonNewRfq.Disabled);
                builder.AddAttribute(2, "Icon", buttonNewRfq.Icon);
                builder.AddAttribute(2, "Clicked", buttonNewRfq.Clicked);
                builder.CloseComponent();
            };
           
            MudBadge badge = new MudBadge()
            {
                Color = MudBlazor.Color.Primary,
                Content = BadgeContent,
                Max = 100,
                Overlap = false,
                Dot = false,
                ChildContent = (builder) =>
                {
                    builder.AddContent(0, renderFragment);
                },
            };
            SimplePageToolbarContributor badgeContributor = new SimplePageToolbarContributor(
                badge.GetType(),
                new Dictionary<string, object?>
                {
                    { "Icon", badge.Icon},
                    { "Color", badge.Color},
                    { "Content", BadgeContent},
                    { "Max", badge.Max},
                    { "Overlap", badge.Overlap},
                    { "Class", badge.Class},
                    { "ChildContent", badge.ChildContent},
                },
                5
                );
            
            Toolbar.Contributors.Add(badgeContributor);
            
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewBillOFMaterial"], async () =>
            {
                await OpenCreateBillOFMaterialModalAsync();
            }, IconName.Add, requiredPolicyName: IBLTermocasaPermissions.BillOFMaterials.Create);
            Toolbar.AddButton("Aumenta", async () =>
            {
                BadgeContent = new Random().Next(0, 100).ToString();
                Toolbar.Contributors.Where(x => x.GetType() == typeof(SimplePageToolbarContributor)).ToList().ForEach(
                    x =>
                    {
                        
                    });
                
                await UiMessageService.Success("Badge content updated with random number: " + BadgeContent);
                
                StateHasChanged();
            }, IconName.PlusCircle, requiredPolicyName: IBLTermocasaPermissions.BillOFMaterials.Create);
            return ValueTask.CompletedTask;
        }

        public string? BadgeContent { get; set; } = "25";

        private async Task SetPermissionsAsync()
        {
            CanCreateBillOFMaterial = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.BillOFMaterials.Create);
            CanEditBillOFMaterial = await AuthorizationService
                            .IsGrantedAsync(IBLTermocasaPermissions.BillOFMaterials.Edit);
            CanDeleteBillOFMaterial = await AuthorizationService
                            .IsGrantedAsync(IBLTermocasaPermissions.BillOFMaterials.Delete);
                            
                            
        }

        private async Task GetBillOFMaterialsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await BillOFMaterialsAppService.GetListAsync(Filter);
            BillOFMaterialList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetBillOFMaterialsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await BillOFMaterialsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("IBLTermocasa") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/bill-of-materials/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Name={HttpUtility.UrlEncode(Filter.Name)}&RequestForQuotationId={HttpUtility.UrlEncode(Filter.RequestForQuotationProperty.Name)}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<BillOFMaterialDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetBillOFMaterialsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateBillOFMaterialModalAsync()
        {
            //TODO: Add your business logic here.
        }


        private async Task OpenEditBillOFMaterialModalAsync(BillOFMaterialDto input)
        {
            //TODO: Add your business logic here.
        }

        private async Task DeleteBillOFMaterialAsync(BillOFMaterialDto input)
        {
            await BillOFMaterialsAppService.DeleteAsync(input.Id);
            await GetBillOFMaterialsAsync();
        }

        protected virtual async Task OnNameChangedAsync(string? name)
        {
            Filter.Name = name;
            await SearchAsync();
        }
        protected virtual async Task OnRequestForQuotationIdChangedAsync(string? requestForQuotationProperty)
        {
            Filter.RequestForQuotationProperty = new RequestForQuotationProperty(Guid.Empty, requestForQuotationProperty);
            await SearchAsync();
        }

        /*protected virtual async Task OnListItemsChangedAsync(string? listItems)
        {
            Filter.ListItems = listItems;
            await SearchAsync();
        }*/
        
        private Task SelectAllItems()
        {
            AllBillOFMaterialsSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllBillOFMaterialsSelected = false;
            SelectedBillOFMaterials.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedBillOFMaterialRowsChanged()
        {
            if (SelectedBillOFMaterials.Count != PageSize)
            {
                AllBillOFMaterialsSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedBillOFMaterialsAsync()
        {
            var message = AllBillOFMaterialsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedBillOFMaterials.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllBillOFMaterialsSelected)
            {
                await BillOFMaterialsAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await BillOFMaterialsAppService.DeleteByIdsAsync(SelectedBillOFMaterials.Select(x => x.Id).ToList());
            }

            SelectedBillOFMaterials.Clear();
            AllBillOFMaterialsSelected = false;

            await GetBillOFMaterialsAsync();
        }


    }
}
