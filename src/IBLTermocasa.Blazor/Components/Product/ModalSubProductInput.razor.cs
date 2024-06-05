using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using IBLTermocasa.Blazor.Components.Selector;
using IBLTermocasa.Products;
using IBLTermocasa.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using NUglify.Helpers;

namespace IBLTermocasa.Blazor.Components.Product;

public partial class ModalSubProductInput
{
    
    [Parameter] public IEnumerable<ProductDto> ProductList { get; set; }
    [Parameter] public SubProductDto SubProduct { get; set; }
    [Parameter] public EventCallback<SubProductDto> OnSave { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }
    [Parameter] public int MaxValue { get; set; } = 100;
    [Parameter] public int MinValue { get; set; } = 1;
    
    private bool IsMultiSelection = true;
    private string ProductLabelSingle = "";
    private string ProductLabelPlural = "";
    private string TextProperty = "Name";
    public Modal SubProductModal { get; set; }
    public ElementSelectorInput ElementSelectorInput { get; set; }
    public IEnumerable<LookupDto<Guid>> ComponentListLookupDto { get; set; }
    public IEnumerable<LookupDto<Guid>> SelectedComponentListLookupDto { get; set; }
    
    private bool _isModalOpen;
    
    private void OpenModal()
    {
        _isModalOpen = true;
    }
    
    protected override async Task OnParametersSetAsync()
    {
        if(ProductList == null || SubProduct == null)
        {
            return;
        }
        else
        {
            ComponentListLookupDto = ProductList.Select(x => new LookupDto<Guid>()
                {
                    Id = x.Id,
                    DisplayName = $"{x.Code} - {x.Name}"
                }
            );
            SelectedComponentListLookupDto = SubProduct.ProductIds.Select(x => new LookupDto<Guid>()
                {
                    Id = x,
                    DisplayName = $"{ComponentListLookupDto.FirstOrDefault(y => y.Id.Equals(x))}"
                }
            );
        }
        ProductLabelSingle = L["Product"];
        ProductLabelPlural = L["Products"];
    }

    private string ResoverDisplayName(Guid id)
    {
        var productDto = ProductList.FirstOrDefault(x => x.Id == id);
        return (productDto != null) ? $"{productDto?.Code} - {productDto?.Name}" : id.ToString();
    }

    private void OnModalCancel(MouseEventArgs obj)
    { 
        OnCancel.InvokeAsync();
        StateHasChanged();
    }

    private void OnModalSave(MouseEventArgs obj)
    {
        var updatedList = ProductList.Where(x => ElementSelectorInput.OutputSelectedComponents()
            .Select(y => y.Id).Contains(x.Id)).ToList();
        if(updatedList.Count > 0)
        {
            SubProduct.ProductIds = updatedList.Select(x => x.Id).ToList();
            Console.WriteLine("SubProduct.Products: " + SubProduct.ProductIds.Count);
            SubProduct.Name = updatedList.First().Name;
        }
        else
        {
            //TODO: Show validation error
        }
        OnSave.InvokeAsync(SubProduct);
        StateHasChanged();
    }

    public void Hide()
    {
        _isModalOpen = false;
        SubProductModal.Hide();
        StateHasChanged();
    }
    
    public void Show()
    {
        _isModalOpen = true;
        SubProductModal.Show();
        StateHasChanged();
    }
    
    private void OnElementSelectorInputSelectedComponentsChanged(IEnumerable<LookupDto<Guid>> selectedComponents)
    {
        SelectedComponentListLookupDto = selectedComponents;
    }

    private void OnSelectorValueChanged(IEnumerable<Guid> selectedComponents)
    {
        List<string> names = ProductList.Where(x => selectedComponents.Contains(x.Id)).Select(x => x.Name).ToList();
        string suggestedName = this.FindCommonNamePart(names);
        SubProduct.Name = suggestedName;
        StateHasChanged();
    }

    private string FindCommonNamePart(List<string> names)
    {
        if(names.Count == 0)
        {
            return "";
        }
        if(names.Count == 1)
        {
            return names.First();
        }
        string commonPart = names.First();
        foreach (var name in names)
        {
            commonPart = this.FindCommonWord(commonPart, name);
        }
        if(commonPart == "")
        {
            return names.First();
        }
        return commonPart;
    }

    private string FindCommonWord(string commonPart, string name)
    {
        string[] commonPartArray = commonPart.Split(" ");
        string[] nameArray = name.Split(" ");
        string result = "";
        for (int i = 0; i < commonPartArray.Length; i++)
        {
            if(commonPartArray[i] == nameArray[i])
            {
                result += commonPartArray[i] + " ";
            }
            else
            {
                break;
            }
        }
        return result;
    }

    public void InitializetModal(SubProductDto selectedSubProducts,  IEnumerable<ProductDto> productList)
    {
        _isModalOpen = false;
        SubProduct = selectedSubProducts;
        ProductList = productList;
        _ = OnParametersSetAsync();
        StateHasChanged();
    }
}