using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Web;
using Blazorise;
using Blazorise.DataGrid;
using IBLTermocasa.Blazor.Components.Product;
using IBLTermocasa.Common;
using Volo.Abp.BlazoriseUI.Components;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using IBLTermocasa.ConsumptionEstimations;
using IBLTermocasa.Permissions;
using IBLTermocasa.Products;
using IBLTermocasa.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;


namespace IBLTermocasa.Blazor.Pages
{
    public partial class ConsumptionEstimations
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<ConsumptionEstimationDto> ConsumptionEstimationList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateConsumptionEstimation { get; set; }
        private bool CanEditConsumptionEstimation { get; set; }
        private bool CanDeleteConsumptionEstimation { get; set; }
        private ConsumptionEstimationCreateDto NewConsumptionEstimation { get; set; }
        private Validations NewConsumptionEstimationValidations { get; set; } = new();
        private ConsumptionEstimationUpdateDto EditingConsumptionEstimation { get; set; }
        private GetConsumptionEstimationsInput Filter { get; set; }
        private ConsumptionEstimationDto? SelectedConsumptionEstimation;
        private List<ConsumptionEstimationDto> SelectedConsumptionEstimations { get; set; } = new();
        private bool AllConsumptionEstimationsSelected { get; set; }
        Dictionary<PlaceHolderType, string> icons = new Dictionary<PlaceHolderType, string>();
        
        
        [Inject] private IProductsAppService ProductsAppService { get; set; }
        MudForm formConsumptionEstimation;
        bool successValidationConsumptionEstimation {get; set;}
        string[] errorsValidationConsumptionEstimation = { };
        private List<LookupDto<Guid>> ProductsList { get; set; } = new();
        public MudTreeView<PlaceHolderTreeItemData> ProductMudTreeView { get; set; }
        public HashSet<PlaceHolderTreeItemData> TreeItems { get; set; } = new HashSet<PlaceHolderTreeItemData>();
        public HashSet<PlaceHolderTreeItemData> SelectedValues { get; set; }


        private async Task GetListProductsCollectionLookupAsync()
        {
            ProductsList = (await ProductsAppService.GetProductLookupAsync(new LookupRequestDto())).Items.ToList();
        }
        
        private async Task OnClickSelectProduct(LookupDto<Guid> item)
        {
            SelectedProduct = await ProductsAppService.GetAsync(item.Id);
            SelectedProducts = await ProductsAppService.GetListByIdAsync(SelectedProduct.SubProducts.Select(x => x.ProductId).ToList());
            Console.WriteLine(":::::::::::::::::::::::::::::::::::OnClickSelectProduct: " + item);
            await OnSelectedItemChanged(item.Id);
            StateHasChanged();
        }

        public List<ProductDto> SelectedProducts { get; set; } = new List<ProductDto>();

        public ProductDto SelectedProduct { get; set; } = new ProductDto();

        private async Task OnSelectedItemChanged(Guid idProduct)
        {
            ConsumptionEstimationDto selectedConsumptionEstimation = new ConsumptionEstimationDto(
                id: Guid.NewGuid(),
                idProduct: idProduct,
                consumptionWork: new List<ConsumptionWorkDto>(), 
                consumptionProduct: new List<ConsumptionProductDto>()
                );
            List<ConsumptionProductDto> consumptionProduct = new List<ConsumptionProductDto>();
            foreach (var subProductItem in selectedConsumptionEstimation.ConsumptionProduct)
            {
                var subProduct = await ProductsAppService.GetAsync(subProductItem.IdProductComponent);
                consumptionProduct.Add(new ConsumptionProductDto(
                    id: subProductItem.Id,
                    idProductComponent: subProductItem.IdProductComponent,
                    consumptionComponentFormula: subProductItem.ConsumptionComponentFormula,
                    isValid: subProductItem.IsValid
                ));
            }
            Console.WriteLine(":::::::::::::::::::::::::::::::::::OnSelectedItemChanged idProduct: " + idProduct);
            var res = await ConsumptionEstimationsAppService.GetListAsync(Filter);
            Console.WriteLine(":::::::::::::::::::::::::::::::::::OnSelectedItemChanged res: " + res.Items.Count);
            var item = await ConsumptionEstimationsAppService.GetAsyncByProduct(idProduct);
            var product = await ProductsAppService.GetAsync(idProduct);
            List<ProductDto> subProducts = new List<ProductDto>();
            foreach (var subProductItem in product.SubProducts)
            {
                var subProduct = await ProductsAppService.GetAsync(subProductItem.ProductId);
                subProducts.Add(subProduct);
            }
            TransformerUtils transformerUtils = new TransformerUtils();
            var treeItemData = transformerUtils.GenerateTreeItems(product, subProducts, icons);
            TreeItems = new HashSet<PlaceHolderTreeItemData>();
            TreeItems.Add(treeItemData);
            Console.WriteLine(":::::::::::::::::::::::::::::::::::TreeItems: " + TreeItems.Count);
            Console.WriteLine(":::::::::::::::::::::::::::::::::::product: " + product);
            //Console.WriteLine(":::::::::::::::::::::::::::::::::::ConsumptionEstimations: " + item);
        }

        private void initComponentTreeView()
        {
            icons = new Dictionary<PlaceHolderType, string>
            {
                {PlaceHolderType.PRODUCT, MudBlazor.Icons.Material.Filled.AllInbox},
                {PlaceHolderType.PRODUCT_QUESTION_TEMPLATE, MudBlazor.Icons.Material.Filled.QuestionAnswer},
                {PlaceHolderType.PRODUCT_COMPONENT, MudBlazor.Icons.Material.Filled.Commit},
            };
        }
        
        
        public ConsumptionEstimations()
        {
            NewConsumptionEstimation = new ConsumptionEstimationCreateDto();
            EditingConsumptionEstimation = new ConsumptionEstimationUpdateDto();
            Filter = new GetConsumptionEstimationsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            ConsumptionEstimationList = new List<ConsumptionEstimationDto>();
            initComponentTreeView();
            
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetListProductsCollectionLookupAsync();
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:ConsumptionEstimations"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewConsumptionEstimation"], async () =>
            {
                
            }, IconName.Add, requiredPolicyName: IBLTermocasaPermissions.ConsumptionEstimations.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateConsumptionEstimation = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.ConsumptionEstimations.Create);
            CanEditConsumptionEstimation = await AuthorizationService
                            .IsGrantedAsync(IBLTermocasaPermissions.ConsumptionEstimations.Edit);
            CanDeleteConsumptionEstimation = await AuthorizationService
                            .IsGrantedAsync(IBLTermocasaPermissions.ConsumptionEstimations.Delete);
                            
                            
        }

        private async Task GetConsumptionEstimationsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await ConsumptionEstimationsAppService.GetListAsync(Filter);
            ConsumptionEstimationList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetConsumptionEstimationsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await ConsumptionEstimationsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("IBLTermocasa") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/consumption-estimations/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&ConsumptionProduct={HttpUtility.UrlEncode(Filter.IdProduct.ToString())}", forceLoad: true);
        }
        
        private Task ClearSelection()
        {
            AllConsumptionEstimationsSelected = false;
            SelectedConsumptionEstimations.Clear();
            
            return Task.CompletedTask;
        }


    }
}
