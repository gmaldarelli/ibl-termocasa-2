using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Web;
using Blazorise;
using Blazorise.DataGrid;
using Castle.Components.DictionaryAdapter.Xml;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using IBLTermocasa.BillOfMaterials;
using IBLTermocasa.Blazor.Components.BillOfMaterial;
using IBLTermocasa.Common;
using IBLTermocasa.Permissions;
using IBLTermocasa.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Volo.Abp.AspNetCore.Components.Web.Theming.Toolbars;
using Color = Blazorise.Color;
using SortDirection = Blazorise.SortDirection;


namespace IBLTermocasa.Blazor.Pages
{
    public partial class BillOfMaterials
    {
        [Inject]
        public IDialogService DialogService { get; set; }
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<BillOfMaterialDto> BillOfMaterialList { get; set; } = new List<BillOfMaterialDto>();
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateBillOfMaterial { get; set; }
        private bool CanEditBillOfMaterial { get; set; }
        private bool CanDeleteBillOfMaterial { get; set; }
        private GetBillOfMaterialsInput Filter { get; set; } = new();
        protected string SelectedCreateTab = "billOfMaterial-create-tab";
        protected string SelectedEditTab = "billOfMaterial-edit-tab";
        private BillOfMaterialDto? SelectedBillOfMaterial;
        private ModalBillOfMaterialInput AddBillOfMaterialModal { get; set; }
        
        private List<BillOfMaterialDto> SelectedBillOfMaterials { get; set; } = new();
        private bool AllBillOfMaterialsSelected { get; set; }
        
        public BillOfMaterials()
        {
            Filter = new GetBillOfMaterialsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            BillOfMaterialList = new List<BillOfMaterialDto>();
            
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await NewRefqBadgeContributor();
            await GetBillOfMaterialsAsync();
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:BillOfMaterials"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            var badgeContributor = NewRfqBadgeContributor();

            Toolbar.Contributors.Add(badgeContributor);
            
            Toolbar.AddButton(text: L["ExportToExcel"], clicked: async () =>{ await DownloadAsExcelAsync(); }, icon: IconName.Download, order: 1);
            return ValueTask.CompletedTask;
        }

        private SimplePageToolbarContributor NewRfqBadgeContributor()
        {
            ToolbarButton buttonNewRfq = new ToolbarButton()
            {
                Color = Color.Primary,
                Text = "Nuovi Preventivi",
                Disabled = false,
                Icon = IconName.Search,
                Clicked = new Func<Task>(OpenDialogRfqChoiceAsync),
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
                Bordered = true,
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
                    {"Dot", badge.Dot}
                    ,{"Bordered", badge.Bordered} ,
                    { "Class", badge.Class},
                    { "ChildContent", badge.ChildContent},
                },
                5
            );
            return badgeContributor;
        }

        private async Task OpenDialogRfqChoiceAsync()
        {
            var parameters = new DialogParameters<BillOfMaterialFromRrq>
            {
            };
            var dialog = await DialogService.ShowAsync<BillOfMaterialFromRrq>("Seleziona un Preventivo", parameters, new DialogOptions
            {

                FullWidth= true,
                MaxWidth=MaxWidth.Large,
                CloseButton= true,
                DisableBackdropClick= true,
                NoHeader=false,
                Position=DialogPosition.Center,
                CloseOnEscapeKey=false
            });
            var result = await dialog.Result;
            await NewRefqBadgeContributor();
            await GetBillOfMaterialsAsync();
        }

        public string? BadgeContent { get; set; } = "0";
        
        private async Task NewRefqBadgeContributor()
        {
            var result = await RequestForQuotationsAppService.GetNewRequestForQuotationCountAsync();
            BadgeContent = result.Value.ToString();
            Toolbar.Contributors.ToList().ForEach(
                x =>
                {
                    SimplePageToolbarContributor y = (SimplePageToolbarContributor)x;
                    if (y.Arguments != null && y!.Arguments!.ContainsKey("Content"))
                    {
                        y.Arguments["Content"] = BadgeContent;
                    }
                });
            StateHasChanged();
        }
        private async Task SetPermissionsAsync()
        {
            CanCreateBillOfMaterial = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.BillOfMaterials.Create);
            CanEditBillOfMaterial = await AuthorizationService
                            .IsGrantedAsync(IBLTermocasaPermissions.BillOfMaterials.Edit);
            CanDeleteBillOfMaterial = await AuthorizationService
                            .IsGrantedAsync(IBLTermocasaPermissions.BillOfMaterials.Delete);
                            
                            
        }

        private async Task GetBillOfMaterialsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await BillOfMaterialsAppService.GetListAsync(Filter);
            BillOfMaterialList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetBillOfMaterialsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await BillOfMaterialsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("IBLTermocasa") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/bill-of-materials/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Name={HttpUtility.UrlEncode(Filter.Name)}&RequestForQuotationId={HttpUtility.UrlEncode(Filter.RequestForQuotationProperty.Name)}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<BillOfMaterialDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetBillOfMaterialsAsync();
            await InvokeAsync(StateHasChanged);
        }


        private async Task DeleteBillOfMaterialAsync(BillOfMaterialDto input)
        {
            await BillOfMaterialsAppService.DeleteAsync(input.Id);
            await GetBillOfMaterialsAsync();
        }

        /*protected virtual async Task OnListItemsChangedAsync(string? listItems)
        {
            Filter.ListItems = listItems;
            await SearchAsync();
        }*/
        
        private Task ClearSelection()
        {
            AllBillOfMaterialsSelected = false;
            SelectedBillOfMaterials.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedBillOfMaterialRowsChanged()
        {
            if (SelectedBillOfMaterials.Count != PageSize)
            {
                AllBillOfMaterialsSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedBillOfMaterialsAsync()
        {
            var message = AllBillOfMaterialsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedBillOfMaterials.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllBillOfMaterialsSelected)
            {
                await BillOfMaterialsAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await BillOfMaterialsAppService.DeleteByIdsAsync(SelectedBillOfMaterials.Select(x => x.Id).ToList());
            }

            SelectedBillOfMaterials.Clear();
            AllBillOfMaterialsSelected = false;

            await GetBillOfMaterialsAsync();
        }

        private void CloseModalBillOfMaterialAsync()
        {
            AddBillOfMaterialModal.Hide();
            InvokeAsync(StateHasChanged);
        }
        
        private async void SaveModalBillOfMaterialInputAsync(BillOfMaterialDto obj)
        {
            AddBillOfMaterialModal.Hide();
            await InvokeAsync(StateHasChanged);
        }

        private void OpenBillOfMaterialDetailAsync(BillOfMaterialDto contextItem)
        {
            NavigationManager.NavigateTo($"/bill-of-materials-detail/{contextItem.Id}");
        }

        private void RemoveBillOfMaterialDetailAsync(BillOfMaterialDto contextItem)
        {
            throw new NotImplementedException();
        }
    }
}
