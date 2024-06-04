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

    private void OnSelectedValue(MudBlazor.Position value)
    {
        switch (value)
        {
            case MudBlazor.Position.Top:
                Position = MudBlazor.Position.Top;
                break;
            case MudBlazor.Position.Start:
                Position = MudBlazor.Position.Start;
                break;
            case MudBlazor.Position.Left:
                Position = MudBlazor.Position.Left;
                break;
            case MudBlazor.Position.Right:
                Position = MudBlazor.Position.Right;
                break;
            case MudBlazor.Position.End:
                Position = MudBlazor.Position.End;
                break;
            case MudBlazor.Position.Bottom:
                Position = MudBlazor.Position.Bottom;
                break;
        }
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
        StateHasChanged();
    }

    private void SaveModalSubProductInputAsync(SubProductDto obj)
    {
        if(Product.SubProducts == null)
        {
            Product.SubProducts = new List<SubProductDto>();
        }
        Product.SubProducts.Add(obj);
        AddSubProductModal.Hide();
        StateHasChanged();
    }

    private void SaveModalProductComponentInputAsync(ProductComponentDto obj)
    {
        if(Product.ProductComponents == null)
        {
            Product.ProductComponents = new List<ProductComponentDto>();
        }
        Product.ProductComponents.Add(obj);
        AddComponentModal.Hide();
        StateHasChanged();
    }
    
    private void SaveModalProductQuestionTemplateInputAsync(ProductQuestionTemplateDto obj)
    {
        if(Product.ProductQuestionTemplates == null)
        {
            Product.ProductQuestionTemplates = new List<ProductQuestionTemplateDto>();
        }
        Product.ProductQuestionTemplates.Add(obj);
        AddSubProductModal.Hide();
        StateHasChanged();
    }
    
    
    private void AddSubProductAsync(MouseEventArgs obj)
    {
        SelectedSubProducts = new SubProductDto();
        SelectedSubProducts.Id = Guid.NewGuid();
        AddSubProductModal.Show();
    }
    
    
    private async Task AddComponentAsync(MouseEventArgs obj)
    {
        ComponentList = (await ComponentsAppService.GetListAsync(new GetComponentsInput())).Items;
        AddComponentModal.Show();
    }
    
    
    private async Task AddQuestionTemplateAsync(MouseEventArgs obj)
    {
        QuestionTemplateList = (await QuestionTemplatesAppService.GetListAsync(new GetQuestionTemplatesInput())).Items;
        AddQuestionTemplateModal.Show();
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

    private void UpdateProductAsync()
    {
        var dto = ObjectMapper.Map<ProductDto, ProductUpdateDto>(Product);
        ProductService.UpdateAsync(Product.Id, dto);
    }

    private void CreateProductAsync()
    {
        var dto = ObjectMapper.Map<ProductDto, ProductCreateDto>(Product);
        ProductService.CreateAsync(dto);
    }

    private void ProductComponentCommittedItemChanges(ProductComponentDto obj)
    {
        List<ProductComponentDto> sortedList = Product.ProductComponents.ToList();
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
            if(!x.Id.Equals(obj.Id))
            {
                x.Order = x.Order < obj.Order ? x.Order : x.Order + 1;
            }else
            {
                x = obj;
            }
        });
        Product.ProductQuestionTemplates = sortedList.OrderBy(x => x.Order).ToList();
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