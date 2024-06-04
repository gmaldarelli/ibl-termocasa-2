using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using IBLTermocasa.Blazor.Components.Selector;
using IBLTermocasa.Components;
using IBLTermocasa.Products;
using IBLTermocasa.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using NUglify.Helpers;

namespace IBLTermocasa.Blazor.Components.Product;

public partial class ModalProductComponentInput
{
    
    [Parameter] public IEnumerable<ComponentDto> ComponentList { get; set; }
    [Parameter] public ProductComponentDto ProductComponent { get; set; }
    [Parameter] public EventCallback<ProductComponentDto> OnSave { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    [Parameter] public int MaxValue { get; set; } = 100;
    [Parameter] public int MinValue { get; set; } = 1;
    
    private bool IsMultiSelection = false;
    private string ProductLabelSingle = "";
    private string ProductLabelPlural = "";
    private string TextProperty = "Name";
    public Modal ProductComponentModal { get; set; }
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
        if(ComponentList == null || ProductComponent == null)
        {
            return;
        }
        else
        {
            ComponentListLookupDto = ComponentList.Select(x => new LookupDto<Guid>()
                {
                    Id = x.Id,
                    DisplayName = $"{x.Name}"
                }
            );
            SelectedComponentListLookupDto = new List<LookupDto<Guid>>();
            
            if(ProductComponent.Component != null)
            {
                new LookupDto<Guid>()
                {
                    Id = ProductComponent.Component.Id,
                    DisplayName = $"{ProductComponent.Component.Name}"
                };
            };
        }
        ProductLabelSingle = L["Product"];
        ProductLabelPlural = L["Products"];
    }

    private string ResoverDisplayName(Guid id)
    {
        var component = ComponentList.FirstOrDefault(x => x.Id == id);
        return (component != null) ? $"{component?.Name}" : id.ToString();
    }

    private void OnModalCancel(MouseEventArgs obj)
    { 
        OnCancel.InvokeAsync();
        StateHasChanged();
    }

    private void OnModalSave(MouseEventArgs obj)
    {
        var updatedList = ComponentList.Where(x => ElementSelectorInput.OutputSelectedComponents()
            .Select(y => y.Id).Contains(x.Id)).ToList();
        if(updatedList.Count > 0)
        {
            ProductComponent.Component = updatedList.First();
        }
        else
        {
            //TODO: Show validation error
        }
        OnSave.InvokeAsync(ProductComponent);
        StateHasChanged();
    }

    public void Hide()
    {
        _isModalOpen = false;
        ProductComponentModal.Hide();
        StateHasChanged();
    }
    
    public void Show()
    {
        _isModalOpen = true;
        ProductComponentModal.Show();
        StateHasChanged();
    }
    
    private void OnElementSelectorInputSelectedComponentsChanged(IEnumerable<LookupDto<Guid>> selectedComponents)
    {
        SelectedComponentListLookupDto = selectedComponents;
    }

    private void OnSelectorValueChanged(IEnumerable<Guid> selectedComponents)
    {
        StateHasChanged();
    }
}