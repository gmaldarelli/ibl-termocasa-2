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
using IBLTermocasa.Products;
using IBLTermocasa.Permissions;
using IBLTermocasa.Shared;
using IBLTermocasa.Subproducts; 



namespace IBLTermocasa.Blazor.Pages
{
    public partial class Products
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        protected bool ShowAdvancedFilters { get; set; }
        private IReadOnlyList<ProductWithNavigationPropertiesDto> ProductList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateProduct { get; set; }
        private bool CanEditProduct { get; set; }
        private bool CanDeleteProduct { get; set; }
        private ProductCreateDto NewProduct { get; set; }
        private Validations NewProductValidations { get; set; } = new();
        private ProductUpdateDto EditingProduct { get; set; }
        private Validations EditingProductValidations { get; set; } = new();
        private Guid EditingProductId { get; set; }
        private Modal CreateProductModal { get; set; } = new();
        private Modal EditProductModal { get; set; } = new();
        private GetProductsInput Filter { get; set; }
        private DataGridEntityActionsColumn<ProductWithNavigationPropertiesDto> EntityActionsColumn { get; set; } = new();
        protected string SelectedCreateTab = "product-create-tab";
        protected string SelectedEditTab = "product-edit-tab";
        private ProductWithNavigationPropertiesDto? SelectedProduct;
        private IReadOnlyList<LookupDto<Guid>> Components { get; set; } = new List<LookupDto<Guid>>();
        
        private string SelectedComponentId { get; set; }
        
        private string SelectedComponentText { get; set; }

        private List<LookupDto<Guid>> SelectedComponents { get; set; } = new List<LookupDto<Guid>>();private IReadOnlyList<LookupDto<Guid>> QuestionTemplates { get; set; } = new List<LookupDto<Guid>>();
        
        private string SelectedQuestionTemplateId { get; set; }
        
        private string SelectedQuestionTemplateText { get; set; }

        private List<LookupDto<Guid>> SelectedQuestionTemplates { get; set; } = new List<LookupDto<Guid>>();
        
                #region Child Entities
        
                #region Subproducts

                private bool CanListSubproduct { get; set; }
                private bool CanCreateSubproduct { get; set; }
                private bool CanEditSubproduct { get; set; }
                private bool CanDeleteSubproduct { get; set; }
                private SubproductCreateDto NewSubproduct { get; set; }
                private Dictionary<Guid, DataGrid<SubproductWithNavigationPropertiesDto>> SubproductDataGrids { get; set; } = new();
                private int SubproductPageSize { get; } = 5;
                private DataGridEntityActionsColumn<SubproductWithNavigationPropertiesDto> SubproductEntityActionsColumns { get; set; } = new();
                private Validations NewSubproductValidations { get; set; } = new();
                private Modal CreateSubproductModal { get; set; } = new();
                private Guid EditingSubproductId { get; set; }
                private SubproductUpdateDto EditingSubproduct { get; set; }
                private Validations EditingSubproductValidations { get; set; } = new();
                private Modal EditSubproductModal { get; set; } = new();
                private IReadOnlyList<LookupDto<Guid>> ProductsCollection { get; set; } = new List<LookupDto<Guid>>();

            
                #endregion
        
        #endregion
        
        
        private List<ProductWithNavigationPropertiesDto> SelectedProducts { get; set; } = new();
        private bool AllProductsSelected { get; set; }
        
        public Products()
        {
            NewProduct = new ProductCreateDto();
            EditingProduct = new ProductUpdateDto();
            Filter = new GetProductsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            ProductList = new List<ProductWithNavigationPropertiesDto>();
            
            NewSubproduct = new SubproductCreateDto();
EditingSubproduct = new SubproductUpdateDto();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetComponentLookupAsync();


            await GetQuestionTemplateLookupAsync();


            await GetProductLookupAsync();
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
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Products"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewProduct"], async () =>
            {
                await OpenCreateProductModalAsync();
            }, IconName.Add, requiredPolicyName: IBLTermocasaPermissions.Products.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateProduct = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.Products.Create);
            CanEditProduct = await AuthorizationService
                            .IsGrantedAsync(IBLTermocasaPermissions.Products.Edit);
            CanDeleteProduct = await AuthorizationService
                            .IsGrantedAsync(IBLTermocasaPermissions.Products.Delete);
                            
            
            #region Subproducts
            CanListSubproduct = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.Subproducts.Default);
            CanCreateSubproduct = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.Subproducts.Create);
            CanEditSubproduct = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.Subproducts.Edit);
            CanDeleteSubproduct = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.Subproducts.Delete);
            #endregion                
        }

        private async Task GetProductsAsync()
        {
            Filter.MaxResultCount = PageSize;
            Filter.SkipCount = (CurrentPage - 1) * PageSize;
            Filter.Sorting = CurrentSorting;

            var result = await ProductsAppService.GetListAsync(Filter);
            ProductList = result.Items;
            TotalCount = (int)result.TotalCount;
            
            await ClearSelection();
        }

        protected virtual async Task SearchAsync()
        {
            CurrentPage = 1;
            await GetProductsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await ProductsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("IBLTermocasa") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/products/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&Code={HttpUtility.UrlEncode(Filter.Code)}&Name={HttpUtility.UrlEncode(Filter.Name)}&Description={HttpUtility.UrlEncode(Filter.Description)}&IsAssembled={Filter.IsAssembled}&IsInternal={Filter.IsInternal}", forceLoad: true);
        }

        private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<ProductWithNavigationPropertiesDto> e)
        {
            CurrentSorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");
            CurrentPage = e.Page;
            await GetProductsAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task OpenCreateProductModalAsync()
        {
            SelectedComponents = new List<LookupDto<Guid>>();
            

            SelectedQuestionTemplates = new List<LookupDto<Guid>>();
            

            NewProduct = new ProductCreateDto{
                
                
            };
            await NewProductValidations.ClearAll();
            await CreateProductModal.Show();
        }

        private async Task CloseCreateProductModalAsync()
        {
            NewProduct = new ProductCreateDto{
                
                
            };
            await CreateProductModal.Hide();
        }

        private async Task OpenEditProductModalAsync(ProductWithNavigationPropertiesDto input)
        {
            var product = await ProductsAppService.GetWithNavigationPropertiesAsync(input.Product.Id);
            
            EditingProductId = product.Product.Id;
            EditingProduct = ObjectMapper.Map<ProductDto, ProductUpdateDto>(product.Product);
            SelectedComponents = product.Components.Select(a => new LookupDto<Guid>{ Id = a.Id, DisplayName = a.Name}).ToList();

            SelectedQuestionTemplates = product.QuestionTemplates.Select(a => new LookupDto<Guid>{ Id = a.Id, DisplayName = a.QuestionText}).ToList();

            await EditingProductValidations.ClearAll();
            await EditProductModal.Show();
        }

        private async Task DeleteProductAsync(ProductWithNavigationPropertiesDto input)
        {
            await ProductsAppService.DeleteAsync(input.Product.Id);
            await GetProductsAsync();
        }

        private async Task CreateProductAsync()
        {
            try
            {
                if (await NewProductValidations.ValidateAll() == false)
                {
                    return;
                }
                NewProduct.ComponentIds = SelectedComponents.Select(x => x.Id).ToList();

                NewProduct.QuestionTemplateIds = SelectedQuestionTemplates.Select(x => x.Id).ToList();


                await ProductsAppService.CreateAsync(NewProduct);
                await GetProductsAsync();
                await CloseCreateProductModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }

        private async Task CloseEditProductModalAsync()
        {
            await EditProductModal.Hide();
        }

        private async Task UpdateProductAsync()
        {
            try
            {
                if (await EditingProductValidations.ValidateAll() == false)
                {
                    return;
                }
                EditingProduct.ComponentIds = SelectedComponents.Select(x => x.Id).ToList();

                EditingProduct.QuestionTemplateIds = SelectedQuestionTemplates.Select(x => x.Id).ToList();


                await ProductsAppService.UpdateAsync(EditingProductId, EditingProduct);
                await GetProductsAsync();
                await EditProductModal.Hide();                
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
        protected virtual async Task OnNameChangedAsync(string? name)
        {
            Filter.Name = name;
            await SearchAsync();
        }
        protected virtual async Task OnDescriptionChangedAsync(string? description)
        {
            Filter.Description = description;
            await SearchAsync();
        }
        protected virtual async Task OnIsAssembledChangedAsync(bool? isAssembled)
        {
            Filter.IsAssembled = isAssembled;
            await SearchAsync();
        }
        protected virtual async Task OnIsInternalChangedAsync(bool? isInternal)
        {
            Filter.IsInternal = isInternal;
            await SearchAsync();
        }
        

        private async Task GetComponentLookupAsync(string? newValue = null)
        {
            Components = (await ProductsAppService.GetComponentLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private void AddComponent()
        {
            if (SelectedComponentId.IsNullOrEmpty())
            {
                return;
            }
            
            if (SelectedComponents.Any(p => p.Id.ToString() == SelectedComponentId))
            {
                UiMessageService.Warn(L["ItemAlreadyAdded"]);
                return;
            }

            SelectedComponents.Add(new LookupDto<Guid>
            {
                Id = Guid.Parse(SelectedComponentId),
                DisplayName = SelectedComponentText
            });
        }

        private async Task GetQuestionTemplateLookupAsync(string? newValue = null)
        {
            QuestionTemplates = (await ProductsAppService.GetQuestionTemplateLookupAsync(new LookupRequestDto { Filter = newValue })).Items;
        }

        private void AddQuestionTemplate()
        {
            if (SelectedQuestionTemplateId.IsNullOrEmpty())
            {
                return;
            }
            
            if (SelectedQuestionTemplates.Any(p => p.Id.ToString() == SelectedQuestionTemplateId))
            {
                UiMessageService.Warn(L["ItemAlreadyAdded"]);
                return;
            }

            SelectedQuestionTemplates.Add(new LookupDto<Guid>
            {
                Id = Guid.Parse(SelectedQuestionTemplateId),
                DisplayName = SelectedQuestionTemplateText
            });
        }


    private bool ShouldShowDetailRow()
    {
        return CanListSubproduct;
    }
    
    public string SelectedChildTab { get; set; } = "subproduct-tab";
        
    private Task OnSelectedChildTabChanged(string name)
    {
        SelectedChildTab = name;
    
        return Task.CompletedTask;
    }


        private Task SelectAllItems()
        {
            AllProductsSelected = true;
            
            return Task.CompletedTask;
        }

        private Task ClearSelection()
        {
            AllProductsSelected = false;
            SelectedProducts.Clear();
            
            return Task.CompletedTask;
        }

        private Task SelectedProductRowsChanged()
        {
            if (SelectedProducts.Count != PageSize)
            {
                AllProductsSelected = false;
            }
            
            return Task.CompletedTask;
        }

        private async Task DeleteSelectedProductsAsync()
        {
            var message = AllProductsSelected ? L["DeleteAllRecords"].Value : L["DeleteSelectedRecords", SelectedProducts.Count].Value;
            
            if (!await UiMessageService.Confirm(message))
            {
                return;
            }

            if (AllProductsSelected)
            {
                await ProductsAppService.DeleteAllAsync(Filter);
            }
            else
            {
                await ProductsAppService.DeleteByIdsAsync(SelectedProducts.Select(x => x.Product.Id).ToList());
            }

            SelectedProducts.Clear();
            AllProductsSelected = false;

            await GetProductsAsync();
        }


        #region Subproducts
        
        private async Task OnSubproductDataGridReadAsync(DataGridReadDataEventArgs<SubproductWithNavigationPropertiesDto> e, Guid productId)
        {
            var sorting = e.Columns
                .Where(c => c.SortDirection != SortDirection.Default)
                .Select(c => c.Field + (c.SortDirection == SortDirection.Descending ? " DESC" : ""))
                .JoinAsString(",");

            var currentPage = e.Page;
            await SetSubproductsAsync(productId, currentPage, sorting: sorting);
            await InvokeAsync(StateHasChanged);
        }
        
        private async Task SetSubproductsAsync(Guid productId, int currentPage = 1, string? sorting = null)
        {
            var product = ProductList.FirstOrDefault(x => x.Product.Id == productId);
            if(product == null)
            {
                return;
            }

            var subproducts = await SubproductsAppService.GetListWithNavigationPropertiesByProductIdAsync(new GetSubproductListInput 
            {
                ProductId = productId,
                MaxResultCount = SubproductPageSize,
                SkipCount = (currentPage - 1) * SubproductPageSize,
                Sorting = sorting
            });

            product.Product.Subproducts = subproducts.Items.ToList();

            var subproductDataGrid = SubproductDataGrids[productId];
            
            subproductDataGrid.CurrentPage = currentPage;
            subproductDataGrid.TotalItems = (int)subproducts.TotalCount;
        }
        
        private async Task OpenEditSubproductModalAsync(SubproductWithNavigationPropertiesDto input)
        {
            var subproduct = await SubproductsAppService.GetAsync(input.Subproduct.Id);

            EditingSubproductId = subproduct.Id;
            EditingSubproduct = ObjectMapper.Map<SubproductDto, SubproductUpdateDto>(subproduct);
            await EditingSubproductValidations.ClearAll();
            await EditSubproductModal.Show();
        }
        
        private async Task CloseEditSubproductModalAsync()
        {
            await EditSubproductModal.Hide();
        }
        
        private async Task UpdateSubproductAsync()
        {
            try
            {
                if (await EditingSubproductValidations.ValidateAll() == false)
                {
                    return;
                }

                await SubproductsAppService.UpdateAsync(EditingSubproductId, EditingSubproduct);
                await SetSubproductsAsync(EditingSubproduct.ProductId);
                await EditSubproductModal.Hide();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        
        private async Task DeleteSubproductAsync(SubproductWithNavigationPropertiesDto input)
        {
            await SubproductsAppService.DeleteAsync(input.Subproduct.Id);
            await SetSubproductsAsync(input.Subproduct.ProductId);
        }
        
        private async Task OpenCreateSubproductModalAsync(Guid productId)
        {
            NewSubproduct = new SubproductCreateDto
            {
                ProductId = productId
            };

            await NewSubproductValidations.ClearAll();
            await CreateSubproductModal.Show();
        }
        
        private async Task CloseCreateSubproductModalAsync()
        {
            NewSubproduct = new SubproductCreateDto();

            await CreateSubproductModal.Hide();
        }
        
        private async Task CreateSubproductAsync()
        {
            try
            {
                if (await NewSubproductValidations.ValidateAll() == false)
                {
                    return;
                }

                await SubproductsAppService.CreateAsync(NewSubproduct);
                await SetSubproductsAsync(NewSubproduct.ProductId);
                await CloseCreateSubproductModalAsync();
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(ex);
            }
        }
        
                private async Task GetProductLookupAsync(string? filter = null)
        {
            ProductsCollection = (await SubproductsAppService.GetProductLookupAsync(new LookupRequestDto
            {
                Filter = filter
            })).Items;
        }
        
        #endregion
    }
}
