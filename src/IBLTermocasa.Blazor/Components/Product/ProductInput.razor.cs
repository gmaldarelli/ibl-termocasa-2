using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IBLTermocasa.Common;
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
    
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Parameter] public ProductDto Product { get; set; }
    
    [Parameter] public EventCallback<ProductDto> OnProductSaved { get; set; }
    
    [Parameter] public EventCallback<ProductDto> OnProductCancel { get; set; }

    private bool IsNew;

    private IReadOnlyList<ComponentDto> ComponentList = Array.Empty<ComponentDto>();
    private IReadOnlyList<QuestionTemplateDto> QuestionTemplateList = Array.Empty<QuestionTemplateDto>();
    
    public Position Position { get; set; } = Position.Left;
    
    private MudTable<SubProductDto> SubProductsTable { get; set; }
    private MudTable<ProductComponentDto> ComponentsTable { get; set; }
    private MudTable<ProductQuestionTemplateDto> QuestionTemplateTable { get; set; }

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
    
    private async void AddSubProductAsync(MouseEventArgs obj)
    {
        ProductList = (await ProductService.GetListAsync(new GetProductsInput())).Items;
        List<ExtendedLookUpDto<Guid>>? ElementListLookupDto  = new List<ExtendedLookUpDto<Guid>>();
        ProductList.ToList().ForEach(x =>
        {
            ElementListLookupDto.Add(new ExtendedLookUpDto<Guid>
            {
                DisplayName = x.Name,
                Id = x.Id,
                ViewElementDto = new ViewElementDto
                {
                    Properties = new List<ViewElementPropertyDto<object>>
                    {
                        new ViewElementPropertyDto<object>("Code", x.Code),
                        new ViewElementPropertyDto<object>("ElementType", typeof(SubProductDto))
                    }
                }
            });
        });
        
        var productLookUp = new ExtendedLookUpDto<Guid>
        {
            DisplayName = Product.Name,
            Id = Product.Id,
            ViewElementDto = new ViewElementDto()
        };
        var subProductDialog = await DialogService.ShowAsync<ProductSubItemInput>("Seleziona Prodotto", new DialogParameters
        {
            { "ElementListLookupDto", ElementListLookupDto },
            { "DialogTitle", "Seleziona Prodotto" },
            { "ElementName", "Prodotto" },
            { "OrderMaxValue", Product.SubProducts.Count + 1 },
            { "OrderMinValue", 1 },
            { "ElementType",  typeof(SubProductDto) },
            { "ElementListExisted", new List<ExtendedLookUpDto<Guid>>(){productLookUp} }
        }, new DialogOptions
        {
            Position = DialogPosition.Custom,
            FullWidth = true,
            MaxWidth = MaxWidth.Medium
        });
        
        var subProductDialogResult = await subProductDialog.Result;
        if(!subProductDialogResult.Canceled)
        {
            var selectedProduct = (ExtendedLookUpDto<Guid>)subProductDialogResult.Data;
            var subProduct = new SubProductDto
            {
                Id = Guid.NewGuid(),
                ParentId = Product.Id,
                ProductId = selectedProduct.Id,
                Code = selectedProduct.ViewElementDto.Properties.FirstOrDefault(x => x.Name == "Code")?.Value.ToString() ?? "",
                Name = selectedProduct.DisplayName,
                Order = selectedProduct.ViewElementDto.Properties.FirstOrDefault(x => x.Name == "Order")?.Value as int? ?? Product.SubProducts.Count + 1,
                Mandatory = selectedProduct.ViewElementDto.Properties.FirstOrDefault(x => x.Name == "IsMandatory")?.Value as bool? ?? false
            };
            foreach (var item in Product.SubProducts)
            {
                if(item.Code == subProduct.Code)
                {
                    subProduct.Code = subProduct.Code + " 1";
                    if(item.Name.Equals(subProduct.Name))
                    {
                        subProduct.Name = subProduct.Name + " 1";
                    }
                }
            }
            Product.SubProducts =  Product.SubProductReorder(subProduct);
            await SubProductMudDataGrid.ReloadServerData();
            await InvokeAsync(StateHasChanged);
        }
    }
    
    
    private async Task AddComponentAsync(MouseEventArgs obj)
    {
        ComponentList = (await ComponentsAppService.GetListAsync(new GetComponentsInput())).Items;
        List<ExtendedLookUpDto<Guid>>? ElementListLookupDto  = new List<ExtendedLookUpDto<Guid>>();
        ComponentList.ToList().ForEach(x =>
        {
            ElementListLookupDto.Add(new ExtendedLookUpDto<Guid>
            {
                DisplayName = x.Name,
                Id = x.Id,
                ViewElementDto = new ViewElementDto
                {
                    Properties = new List<ViewElementPropertyDto<object>>
                    {
                        new ViewElementPropertyDto<object>("Code", x.Code),
                        new ViewElementPropertyDto<object>("ElementType", typeof(ProductComponentDto))
                    }
                }
            });
        });
        var componentDialog = await DialogService.ShowAsync<ProductSubItemInput>("Seleziona Componente", new DialogParameters
        {
            { "ElementListLookupDto", ElementListLookupDto },
            { "DialogTitle", "Seleziona Componente" },
            { "ElementName", "Componente" },
            { "OrderMaxValue", Product.ProductComponents.Count + 1 },
            { "OrderMinValue", 1 },
            { "ElementType",  typeof(ProductComponentDto) },
            { "ElementListExisted", new List<ExtendedLookUpDto<Guid>>() }
        }, new DialogOptions
        {
            Position = DialogPosition.Custom,
            FullWidth = true,
            MaxWidth = MaxWidth.Medium
        });
        var componentDialogResult = await componentDialog.Result;
        if(!componentDialogResult.Canceled)
        {
            var selectedComponent = (ExtendedLookUpDto<Guid>)componentDialogResult.Data;
            var productComponent = new ProductComponentDto
            {
                Id = Guid.NewGuid(),
                ParentId = Product.Id,
                ComponentId = selectedComponent.Id,
                Code = selectedComponent.ViewElementDto.Properties.FirstOrDefault(x => x.Name == "Code")?.Value.ToString() ?? "",
                Name = selectedComponent.DisplayName,
                Order = selectedComponent.ViewElementDto.Properties.FirstOrDefault(x => x.Name == "Order")?.Value as int? ?? Product.ProductComponents.Count + 1,
                Mandatory = selectedComponent.ViewElementDto.Properties.FirstOrDefault(x => x.Name == "IsMandatory")?.Value as bool? ?? false
            };
            foreach (var item in Product.ProductComponents)
            {
                if(item.Code == productComponent.Code)
                {
                    productComponent.Code = productComponent.Code + " 1";
                    if(item.Name.Equals(productComponent.Name))
                    {
                        productComponent.Name = productComponent.Name + " 1";
                    }
                }
            }
            Product.ProductComponents =  Product.ProductComponentsReorder(productComponent);
            await ProductComponentMudDataGrid.ReloadServerData();
            await InvokeAsync(StateHasChanged);
        }
        
    }
    
    
    private async Task AddQuestionTemplateAsync(MouseEventArgs obj)
    {
        QuestionTemplateList = (await QuestionTemplatesAppService.GetListAsync(new GetQuestionTemplatesInput())).Items;
        List<ExtendedLookUpDto<Guid>>? ElementListLookupDto  = new List<ExtendedLookUpDto<Guid>>();
        QuestionTemplateList.ToList().ForEach(x =>
        {
            ElementListLookupDto.Add(new ExtendedLookUpDto<Guid>
            {
                DisplayName = x.QuestionText,
                Id = x.Id,
                ViewElementDto = new ViewElementDto
                {
                    Properties = new List<ViewElementPropertyDto<object>>
                    {
                        new ViewElementPropertyDto<object>("Code", x.Code),
                        new ViewElementPropertyDto<object>("ElementType", typeof(ProductQuestionTemplateDto))
                    }
                }
            });
        });
        
        var questionTemplateDialog = await DialogService.ShowAsync<ProductSubItemInput>("Seleziona Template Domanda", new DialogParameters
        {
            { "ElementListLookupDto", ElementListLookupDto },
            { "DialogTitle", "Seleziona Template Domanda" },
            { "ElementName", "Template Domanda" },
            { "OrderMaxValue", Product.ProductQuestionTemplates.Count + 1 },
            { "OrderMinValue", 1 },
            { "ElementType",  typeof(ProductQuestionTemplateDto) },
            { "ElementListExisted", new List<ExtendedLookUpDto<Guid>>()  }
        }, new DialogOptions
        {
            Position = DialogPosition.Custom,
            FullWidth = true,
            MaxWidth = MaxWidth.Medium
        });
        
        var questionTemplateDialogResult = await questionTemplateDialog.Result;
        if(!questionTemplateDialogResult.Canceled)
        {
            var selectedQuestionTemplate = (ExtendedLookUpDto<Guid>)questionTemplateDialogResult.Data;
            var productQuestionTemplate = new ProductQuestionTemplateDto
            {
                Id = Guid.NewGuid(),
                ParentId = Product.Id,
                QuestionTemplateId = selectedQuestionTemplate.Id,
                Code = selectedQuestionTemplate.ViewElementDto.Properties.FirstOrDefault(x => x.Name == "Code")?.Value.ToString() ?? "",
                Name = selectedQuestionTemplate.DisplayName,
                Order = selectedQuestionTemplate.ViewElementDto.Properties.FirstOrDefault(x => x.Name == "Order")?.Value as int? ?? Product.ProductQuestionTemplates.Count + 1,
                Mandatory = selectedQuestionTemplate.ViewElementDto.Properties.FirstOrDefault(x => x.Name == "IsMandatory")?.Value as bool? ?? false
            };
            
            foreach (var item in Product.ProductQuestionTemplates)
            {
                if(item.Code == productQuestionTemplate.Code)
                {
                    productQuestionTemplate.Code = productQuestionTemplate.Code + " 1";
                    if(item.Name.Equals(productQuestionTemplate.Name))
                    {
                        productQuestionTemplate.Name = productQuestionTemplate.Name + " 1";
                    }
                }
            }
            Product.ProductQuestionTemplates =  Product.ProductQuestionTemplatesReorder(productQuestionTemplate);
            await ProductQuestionTemplateMudDataGrid.ReloadServerData();
            await InvokeAsync(StateHasChanged);
        }
    }

    private async void SaveProductAsync(MouseEventArgs obj)
    {
        await formProductInfo.Validate();
        if(errorsValidationProductInfo.Length > 0)
        {
            return;
        }

        if(IsNew)
        {
            CreateProductAsync();
        }
        else
        {
            UpdateProductAsync();
        }
        this.AfterSave();
    }

    private async void UpdateProductAsync()
    {
        var dto = ObjectMapper.Map<ProductDto, ProductUpdateDto>(Product);
        Product = await ProductService.UpdateAsync(Product.Id, dto);
        IsNew = false;
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
        Product.ProductComponents.RemoveAll(x => x.Id.Equals(obj.Id));
        Product.ProductComponents = Product.ProductComponentsReorder(obj);
        ProductComponentMudDataGrid.ReloadServerData();
        StateHasChanged();
    }

    private void SubProductDtoCommittedItemChanges(SubProductDto obj)
    {
        Product.SubProducts.RemoveAll(x => x.Id.Equals(obj.Id));
        Product.SubProducts = Product.SubProductReorder(obj);
        StateHasChanged();
    }

    private void QuestionTemplateDtoCommittedItemChanges(ProductQuestionTemplateDto obj)
    {
        Product.ProductQuestionTemplates.RemoveAll(x => x.Id.Equals(obj.Id));
        Product.ProductQuestionTemplates = Product.ProductQuestionTemplatesReorder(obj);
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

    private async Task OpenConsumeCalculator()
    {
        Dictionary<string,string> inputFormulas = new();
        Product.ProductComponents.ForEach(component =>
        {
            inputFormulas.Add(component.Code, component.ConsumptionCalculation ?? "");
        });
        var parameters = new DialogParameters
        {
            { "Product", Product },
            { "Formulas", inputFormulas }
        };
        var dialog = await DialogService.ShowAsync<ConsumeCalculator>("", parameters,
            new DialogOptions
            {
                Position = DialogPosition.Custom,
                FullWidth = true,
                MaxWidth = MaxWidth.Large
            });
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            Dictionary<string,string> formulas = (Dictionary<string, string>)result.Data;
            Product.ProductComponents.ForEach(component =>
            {
                component.ConsumptionCalculation = formulas[component.Code];
            });
            StateHasChanged();
        }
    }

    private async void Cancel(MouseEventArgs obj)
    {
        var message = L["ConfirmCancelMessage"];
        if (!await UiMessageService.Confirm(message))
        {
            return;
        }
        else
        {
            NavigationManager.NavigateTo("/products");
        }
    }
    
    private async void AfterSave()
    {
        var message = L["ProductSavedMessage"];
        if (!await UiMessageService.Confirm(message))
        {
            NavigationManager.NavigateTo($"/products/{Product.Id}");
        }
        else
        {
            NavigationManager.NavigateTo("/products");
        }
    }
}