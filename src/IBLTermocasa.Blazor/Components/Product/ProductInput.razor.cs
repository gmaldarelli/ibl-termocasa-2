using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using IBLTermocasa.Blazor.Components.Selector;
using IBLTermocasa.Components;
using IBLTermocasa.Products;
using IBLTermocasa.QuestionTemplates;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace IBLTermocasa.Blazor.Components.Product;

public partial class ProductInput
{
    [Inject]
    public IProductsAppService ProductService { get; set; }
    
    [Inject]
    public IQuestionTemplatesAppService QuestionTemplatesAppService { get; set; }
    
    [Inject]
    public IComponentsAppService ComponentsAppService { get; set; }
    [Inject]
    public IDialogService DialogService { get; set; }
    [Parameter] public ProductDto Product { get; set; }
    
    [Parameter] public EventCallback<ProductDto> OnProductSaved { get; set; }
    
    [Parameter] public EventCallback<ProductDto> OnProductCancel { get; set; }

    private bool IsNew;

    private IReadOnlyList<ComponentDto> ComponentList = Array.Empty<ComponentDto>();
    private IReadOnlyList<QuestionTemplateDto> QuestionTemplateList = Array.Empty<QuestionTemplateDto>();
    
    public MudBlazor.Position Position { get; set; } = MudBlazor.Position.Left;

    public ModalProductComponentInput AddComponentModal { get; set; }
    public ModalProductQuestionTemplateInput AddQuestionTemplateModal { get; set; }

    private ComponentAutocompleteInput ComponentDropInput { get; set; }
    
    private MudTable<SubProductDto> SubProductsTable { get; set; }
    private MudTable<ProductComponentDto> ComponentsTable { get; set; }
    private MudTable<ProductQuestionTemplateDto> QuestionTemplateTable { get; set; }

    public ModalSubProductInput AddSubProductModal { get; set; }
    public IEnumerable<ProductDto> ProductList { get; set; } = Array.Empty<ProductDto>();

    public SubProductDto SelectedSubProducts { get; set; } = new SubProductDto()
    {
        Id = Guid.NewGuid()
    };

    public ProductComponentDto SelectedProductComponent { get; set; } = new ProductComponentDto();
    public ProductQuestionTemplateDto  SelectedProductQuestionTemplate { get; set; } = new ProductQuestionTemplateDto();
    public MudDataGrid<ProductComponentDto> ProductComponentMudDataGrid { get; set; }
    public MudDataGrid<ProductQuestionTemplateDto> ProductQuestionTemplateMudDataGrid { get; set; }
    public MudDataGrid<SubProductDto> SubProductMudDataGrid { get; set; }


    protected override Task OnInitializedAsync()
    {
        InitializeComponentAsync();
        return base.OnInitializedAsync();
    }

    private async void InitializeComponentAsync()
    {
        //TODO: Implement this method
    }

    protected override async Task OnParametersSetAsync()
    {
        if(Product == null)
        {
            Product = new ProductDto();
        }
        IsNew = Product.Id == Guid.Empty;
        ProductList = (await ProductService.GetListAsync(new GetProductsInput())).Items;
        await  base.OnParametersSetAsync();
    }

    private async Task LoadProductAsync(Guid id)
    {
        Product = await ProductService.GetAsync(id);
        
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        return base.OnAfterRenderAsync(firstRender);
    }

   
    
    private void CloseModalProductComponentAsync()
    {
        AddComponentModal.Hide();
        InvokeAsync(StateHasChanged);
    }
    private void CloseModalProductQuestionTemplateAsync()
    {
        AddQuestionTemplateModal.Hide();
        InvokeAsync(StateHasChanged);
    }

    private void CloseModalSubProductInputAsync()
    {
        AddSubProductModal.Hide();
        InvokeAsync(StateHasChanged);
    }

    private async void SaveModalSubProductInputAsync(SubProductDto obj)
    {
        if(Product.SubProducts == null)
        {
            Product.SubProducts = new List<SubProductDto>();
        }
        Product.SubProducts.RemoveAll(x => x.Id == obj.Id);
        Product.SubProducts.Add(obj);
        Product.SubProducts = Product.SubProducts.OrderBy(x => x.Order).ToList();
        await SubProductMudDataGrid.ReloadServerData();
        await InvokeAsync(StateHasChanged);
    }

    private async void SaveModalProductComponentInputAsync(ProductComponentDto obj)
    {

        AddComponentModal.Hide();
        if(Product.ProductComponents == null)
        {
            Product.ProductComponents = new List<ProductComponentDto>();
        }
        
        Product.ProductComponents.RemoveAll(x => x.ComponentId == obj.ComponentId);
        Product.ProductComponents.Add(obj);
        Product.ProductComponents = Product.ProductComponents.OrderBy(x => x.Order).ToList();
        await ProductComponentMudDataGrid.ReloadServerData();
        await InvokeAsync(StateHasChanged);
        
    }
    
    private  async void SaveModalProductQuestionTemplateInputAsync(ProductQuestionTemplateDto obj)
    {
        AddQuestionTemplateModal.Hide();
        if(Product.ProductQuestionTemplates == null)
        {
            Product.ProductQuestionTemplates = new List<ProductQuestionTemplateDto>();
        }

        Product.ProductQuestionTemplates.RemoveAll(x => x.QuestionTemplateId == obj.QuestionTemplateId);
        Product.ProductQuestionTemplates.Add(obj);
        Product.ProductQuestionTemplates = Product.ProductQuestionTemplates.OrderBy(x => x.Order).ToList();
        await ProductQuestionTemplateMudDataGrid.ReloadServerData();
        await InvokeAsync(StateHasChanged);
    }
    
    
    private async void AddSubProductAsync(MouseEventArgs obj)
    {
        SelectedSubProducts = new SubProductDto();
        SelectedSubProducts.Order = Product.SubProducts.Count + 1;
        AddSubProductModal.InitializetModal(SelectedSubProducts,  ProductList);
        AddSubProductModal.Show();
    }
    
    
    private async Task AddComponentAsync(MouseEventArgs obj)
    {
        /*
        ComponentList = (await ComponentsAppService.GetListAsync(new GetComponentsInput())).Items;
        AddComponentModal.ComponentList = ComponentList;
        SelectedProductComponent.Order = Product.ProductComponents.Count + 1;
        AddComponentModal.InitializetModal(SelectedProductComponent,  ComponentList);
        AddComponentModal.Show();
        */
        
        
        ComponentList = (await ComponentsAppService.GetListAsync(new GetComponentsInput())).Items;
        ProductComponentDto selectedProductComponent = new ProductComponentDto();
        selectedProductComponent.Order = Product.ProductQuestionTemplates.Count + 1;
        var parameters = new DialogParameters<ModalProductComponentInput>
        {
            {x => x.ProductComponent, selectedProductComponent},
            {x => x.ComponentList, ComponentList},
            {x => x.OrderMinValue, 1},
            {x => x.OrderMaxValue, Product.ProductComponents.Count + 1}
        };
        var dialog = await DialogService.ShowAsync<ModalProductComponentInput>("Select Product Components", parameters, new DialogOptions
        {
            Position = MudBlazor.DialogPosition.Center,
            FullWidth = true,
            MaxWidth = MaxWidth.Small
        });
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            var productComponent = (ProductComponentDto)result.Data;
            Product.ProductComponents.RemoveAll(x => x.ComponentId == productComponent.ComponentId);
            Product.ProductComponents.Add(productComponent);
            Product.ProductComponents = Product.ProductComponents.OrderBy(x => x.Order).ToList();
            await ProductComponentMudDataGrid.ReloadServerData();
            await InvokeAsync(StateHasChanged);
        }
        
        
        
    }
    
    
    private async Task AddQuestionTemplateAsync(MouseEventArgs obj)
    {

        
        QuestionTemplateList = (await QuestionTemplatesAppService.GetListAsync(new GetQuestionTemplatesInput())).Items;
        ProductQuestionTemplateDto selectedProductQuestionTemplate = new ProductQuestionTemplateDto();
        selectedProductQuestionTemplate.Order = Product.ProductQuestionTemplates.Count + 1;
        var parameters = new DialogParameters<ModalProductQuestionTemplateInput>
        {
            {x => x.ProductQuestionTemplate, selectedProductQuestionTemplate},
            {x => x.ComponentList, QuestionTemplateList},
            {x => x.OrderMinValue, 1},
            {x => x.OrderMaxValue, Product.ProductQuestionTemplates.Count + 1}
        };
        var dialog = await DialogService.ShowAsync<ModalProductQuestionTemplateInput>("Select Question Template", parameters, new DialogOptions
        {
            Position = MudBlazor.DialogPosition.Center,
            FullWidth = true,
            MaxWidth = MaxWidth.Small
        });
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            var productQuestionTemplate = (ProductQuestionTemplateDto)result.Data;
            Product.ProductQuestionTemplates.RemoveAll(x => x.QuestionTemplateId == productQuestionTemplate.QuestionTemplateId);
            Product.ProductQuestionTemplates.Add(productQuestionTemplate);
            Product.ProductQuestionTemplates = Product.ProductQuestionTemplates.OrderBy(x => x.Order).ToList();
            await ProductQuestionTemplateMudDataGrid.ReloadServerData();
            await InvokeAsync(StateHasChanged);
        }

        
        
    }

    private void SaveProductAsync(MouseEventArgs obj)
    {
        if(IsNew)
        {
            CreateProductAsync();
        }
        else
        {
            UpdateProductAsync();
        }
    }

    private async void UpdateProductAsync()
    {
        var dto = ObjectMapper.Map<ProductDto, ProductUpdateDto>(Product);
        Console.WriteLine("Product SubProducts.Count: " + Product.SubProducts.Count);
        Product = await ProductService.UpdateAsync(Product.Id, dto);
       StateHasChanged();
    }

    private async void CreateProductAsync()
    {
        var dto = ObjectMapper.Map<ProductDto, ProductCreateDto>(Product);
        Product = await ProductService.CreateAsync(dto);
        StateHasChanged();
    }

    private void ProductComponentCommittedItemChanges(ProductComponentDto obj)
    {
        List<ProductComponentDto> sortedList = Product.ProductComponents.ToList();
        sortedList.ForEach(x =>
        {
            if(!x.ComponentId.Equals(obj.ComponentId))
            {
                x.Order = x.Order < obj.Order ? x.Order : (x.Order + 1 >= sortedList.Count ? sortedList.Count : x.Order + 1);
            }else
            {
                x = obj;
            }
        });
        Product.ProductComponents = sortedList.OrderBy(x => x.Order).ToList();
        ProductComponentMudDataGrid.ReloadServerData();
        StateHasChanged();
    }

    private void SubProductDtoCommittedItemChanges(SubProductDto obj)
    {
        List<SubProductDto> sortedList = Product.SubProducts.ToList();
        sortedList.ForEach(x =>
        {
            if(!x.Id.Equals(obj.Id))
            {
                x.Order = x.Order < obj.Order ? x.Order : x.Order + 1;
            }else
            {
                x = obj;
            }
        });
        Product.SubProducts = sortedList.OrderBy(x => x.Order).ToList();
        StateHasChanged();
    }

    private void QuestionTemplateDtoCommittedItemChanges(ProductQuestionTemplateDto obj)
    {
        List<ProductQuestionTemplateDto> sortedList = Product.ProductQuestionTemplates.ToList();
        sortedList.ForEach(x =>
        {
            if(!x.QuestionTemplateId.Equals(obj.QuestionTemplateId))
            {
                x.Order = x.Order < obj.Order ? x.Order : (x.Order + 1 > sortedList.Count ? sortedList.Count : x.Order + 1);
            }else
            {
                x = obj;
            }
        });
        Product.ProductQuestionTemplates = sortedList.OrderBy(x => x.Order).ToList();
        ProductQuestionTemplateMudDataGrid.ReloadServerData();
        StateHasChanged();
    }

    private void DeleteProductComponentAsync(ProductComponentDto obj)
    {
        Product.ProductComponents.Remove(obj);
        ProductComponentMudDataGrid.ReloadServerData();
        StateHasChanged();
    }
    
    private void DeleteSubProductAsync(SubProductDto obj)
    {
        Product.SubProducts.Remove(obj);
        StateHasChanged();
    }
    
    private void DeleteProductQuestionTemplateAsync(ProductQuestionTemplateDto obj)
    {
        Product.ProductQuestionTemplates.Remove(obj);
        StateHasChanged();
    }
}