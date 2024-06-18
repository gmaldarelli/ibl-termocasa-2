using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using IBLTermocasa.Components;
using IBLTermocasa.Products;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace IBLTermocasa.Blazor.Components.Product;

public partial class ModalProductComponentInput
{
    
    [Parameter] public IEnumerable<ComponentDto> ComponentList { get; set; }
    [Parameter] public ProductComponentDto ProductComponent { get; set; }
    [Parameter] public EventCallback<ProductComponentDto> OnSave { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    [Parameter] public int OrderMaxValue { get; set; } = 100;
    [Parameter] public int OrderMinValue { get; set; } = 1;
    
    private bool IsMultiSelection = false;
    private string ProductLabelSingle = "";
    private string ProductLabelPlural = "";
    private string TextProperty = "Name";
    public Modal ProductComponentModal { get; set; }
    
    public MudSelect<string> MudSelectComponent { get; set; }

    private bool _isModalOpen;
    private string? GuidStringSelected;
    
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
            GuidStringSelected = ProductComponent?.ComponentId.ToString();
        }
        if(GuidStringSelected == null || GuidStringSelected == Guid.Empty.ToString())
        {
            GuidStringSelected = null;
        }
        ProductLabelSingle = L["Component"];
        ProductLabelPlural = L["Components"];
    }

    private string ResoverDisplayName(Guid id)
    {
        var component = ComponentList.FirstOrDefault(x => x.Id == id);
        return (component != null) ? $"{component?.Name}" : id.ToString();
    }

    private void OnModalCancel(MouseEventArgs obj)
    { 
        _isModalOpen = false;
        ProductComponentModal.Hide();
        OnCancel.InvokeAsync();
        StateHasChanged();
    }

    private async void OnModalSave(MouseEventArgs obj)
    {

        if(GuidStringSelected !=null )
        {
            ProductComponent.ComponentId = Guid.Parse(GuidStringSelected);
            Console.WriteLine("ProductComponent.ComponentId: " + ProductComponent.ComponentId);
        }
        else
        {
            //TODO: Show validation error
        }
        await OnSave.InvokeAsync(ProductComponent);
    }

    public void Hide()
    {
        _isModalOpen = false;
        ProductComponentModal.Hide();
    }
    
    public void Show()
    {
        _isModalOpen = true;
        ProductComponentModal.Show();
        StateHasChanged();
    }


    private void OnSelectorValueChanged(IEnumerable<Guid> selectedComponents)
    {
        StateHasChanged();
    }

    private string GetMultiSelectionText(List<string> selectedValues)
    {
        if (selectedValues.Count == 0 || selectedValues[0] == Guid.Empty.ToString())
        {
            return L["NothingSelected"];
        }

        var names = ComponentList.Where(x =>
            selectedValues.Contains(x.Id.ToString())).Select(x => x.Name).ToList();
        return $"{string.Join(", ", names)}";
    }

    private string ComponentName(string guidString)
    {
        return ComponentList.Where(x => x.Id.ToString().Equals(guidString)).Select(x => x.Name).FirstOrDefault()!;
    }
    private string ComponentCode(string guidString)
    {
        return ComponentList.Where(x => x.Id.ToString().Equals(guidString)).Select(x => x.Code).FirstOrDefault()!;
    }

    public void InitializetModal(ProductComponentDto selectedProductComponent, IReadOnlyList<ComponentDto> componentList)
    {
        _isModalOpen = false;
        ProductComponent = selectedProductComponent;
        ComponentList = componentList;
        _ = OnParametersSetAsync();
        StateHasChanged();
    }

    private void OnGuidStringSelectedChanged(string obj)
    {
        GuidStringSelected = obj;
        if(GuidStringSelected != null && GuidStringSelected != Guid.Empty.ToString() && ProductComponent.Name.IsNullOrEmpty())
        {
            ProductComponent.Name = ComponentName(GuidStringSelected);
        }
    }
}