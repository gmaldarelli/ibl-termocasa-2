using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using IBLTermocasa.Blazor.Components.Selector;
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
        
        var elementListExisted = Product.SubProducts.Select(x => new ExtendedLookUpDto<Guid>
        {
            DisplayName = x.Name,
            Id = x.ProductIds[0],
            ViewElementDto = new ViewElementDto()
        }).ToList();
        
        var subProductDialog = await DialogService.ShowAsync<ProductSubItemInput>("Seleziona Prodotto", new DialogParameters
        {
            { "ElementListLookupDto", ElementListLookupDto },
            { "DialogTitle", "Seleziona Prodotto" },
            { "ElementName", "Prodotto" },
            { "OrderMaxValue", Product.SubProducts.Count + 1 },
            { "OrderMinValue", 1 },
            { "ElementType",  typeof(SubProductDto) },
            { "ElementListExisted", elementListExisted }
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
                ProductIds= [selectedProduct.Id],
                Code = selectedProduct.ViewElementDto.Properties.FirstOrDefault(x => x.Name == "Code")?.Value.ToString() ?? "",
                Name = selectedProduct.DisplayName,
                Order = selectedProduct.ViewElementDto.Properties.FirstOrDefault(x => x.Name == "Order")?.Value as int? ?? Product.SubProducts.Count + 1,
                Mandatory = selectedProduct.ViewElementDto.Properties.FirstOrDefault(x => x.Name == "IsMandatory")?.Value as bool? ?? false
            };
            Product.SubProducts.RemoveAll(x => x.Id == subProduct.Id);
            Product.SubProducts.Add(subProduct);
            Product.SubProducts = Product.SubProducts.OrderBy(x => x.Order).ToList();
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
        var elementListExisted = Product.ProductComponents.Select(x => new ExtendedLookUpDto<Guid>
        {
            DisplayName = x.Name,
            Id = x.ComponentId,
            ViewElementDto = new ViewElementDto()
        }).ToList();
        var componentDialog = await DialogService.ShowAsync<ProductSubItemInput>("Seleziona Componente", new DialogParameters
        {
            { "ElementListLookupDto", ElementListLookupDto },
            { "DialogTitle", "Seleziona Componente" },
            { "ElementName", "Componente" },
            { "OrderMaxValue", Product.ProductComponents.Count + 1 },
            { "OrderMinValue", 1 },
            { "ElementType",  typeof(ProductComponentDto) },
            { "ElementListExisted", elementListExisted }
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
                ComponentId = selectedComponent.Id,
                Code = selectedComponent.ViewElementDto.Properties.FirstOrDefault(x => x.Name == "Code")?.Value.ToString() ?? "",
                Name = selectedComponent.DisplayName,
                Order = selectedComponent.ViewElementDto.Properties.FirstOrDefault(x => x.Name == "Order")?.Value as int? ?? Product.ProductComponents.Count + 1,
                Mandatory = selectedComponent.ViewElementDto.Properties.FirstOrDefault(x => x.Name == "IsMandatory")?.Value as bool? ?? false
            };
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
        
        var elementListExisted = Product.ProductQuestionTemplates.Select(x => new ExtendedLookUpDto<Guid>
        {
            DisplayName = x.Name,
            Id = x.QuestionTemplateId,
            ViewElementDto = new ViewElementDto()
        }).ToList();
        
        var questionTemplateDialog = await DialogService.ShowAsync<ProductSubItemInput>("Seleziona Template Domanda", new DialogParameters
        {
            { "ElementListLookupDto", ElementListLookupDto },
            { "DialogTitle", "Seleziona Template Domanda" },
            { "ElementName", "Template Domanda" },
            { "OrderMaxValue", Product.ProductQuestionTemplates.Count + 1 },
            { "OrderMinValue", 1 },
            { "ElementType",  typeof(ProductQuestionTemplateDto) },
            { "ElementListExisted", elementListExisted }
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
                QuestionTemplateId = selectedQuestionTemplate.Id,
                Code = selectedQuestionTemplate.ViewElementDto.Properties.FirstOrDefault(x => x.Name == "Code")?.Value.ToString() ?? "",
                Name = selectedQuestionTemplate.DisplayName,
                Order = selectedQuestionTemplate.ViewElementDto.Properties.FirstOrDefault(x => x.Name == "Order")?.Value as int? ?? Product.ProductQuestionTemplates.Count + 1,
                Mandatory = selectedQuestionTemplate.ViewElementDto.Properties.FirstOrDefault(x => x.Name == "IsMandatory")?.Value as bool? ?? false
            };
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
        if (!result.Cancelled)
        {
            Dictionary<string,string> formulas = (Dictionary<string, string>)result.Data;
            Product.ProductComponents.ForEach(component =>
            {
                component.ConsumptionCalculation = formulas[component.Code];
            });
            StateHasChanged();
        }
    }
}