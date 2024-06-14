using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Force.DeepCloner;
using IBLTermocasa.Products;
using IBLTermocasa.QuestionTemplates;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace IBLTermocasa.Blazor.Components.Product;

public partial class ModalProductQuestionTemplateInput
{
    
    [Parameter] public IEnumerable<QuestionTemplateDto> ComponentList { get; set; }
    [Parameter] public ProductQuestionTemplateDto ProductQuestionTemplate { get; set; }
    [Parameter] public EventCallback<ProductQuestionTemplateDto> OnSave { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    [Parameter] public int OrderMaxValue { get; set; } = 100;
    [Parameter] public int OrderMinValue { get; set; } = 1;
    
    private bool IsMultiSelection = false;
    private string ProductLabelSingle = "";
    private string ProductLabelPlural = "";
    private string TextProperty = "Name";
    public Modal ProductQuestionTemplateModal { get; set; }
    
    public MudSelect<string> MudSelectComponent { get; set; }

    private bool _isModalOpen;
    private string? GuidStringSelected;
    
    private void OpenModal()
    {
        _isModalOpen = true;
    }
    
    protected override async Task OnParametersSetAsync()
    {
        if(ComponentList == null || ProductQuestionTemplate == null)
        {
            return;
        }
        else
        {
            GuidStringSelected = ProductQuestionTemplate?.QuestionTemplateId.ToString();
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
        return (component != null) ? $"{component?.QuestionText}" : id.ToString();
    }

    private void OnModalCancel(MouseEventArgs obj)
    { 
        _isModalOpen = false;
        ProductQuestionTemplateModal.Hide();
        OnCancel.InvokeAsync();
        StateHasChanged();
    }

    private async void OnModalSave(MouseEventArgs obj)
    {

        if(GuidStringSelected !=null )
        {
            ProductQuestionTemplate.QuestionTemplateId = Guid.Parse(GuidStringSelected);
            Console.WriteLine("ProductQuestionTemplate.ComponentId: " + ProductQuestionTemplate.QuestionTemplateId);
        }
        else
        {
            //TODO: Show validation error
        }
        await OnSave.InvokeAsync(ProductQuestionTemplate);
    }

    public void Hide()
    {
        _isModalOpen = false;
        ProductQuestionTemplateModal.Hide();
    }
    
    public void Show()
    {
        _isModalOpen = true;
        ProductQuestionTemplateModal.Show();
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
            selectedValues.Contains(x.Id.ToString())).Select(x => x.QuestionText).ToList();
        return $"{string.Join(", ", names)}";
    }

    private string ComponentName(string guidString)
    {
        return ComponentList.Where(x => x.Id.ToString().Equals(guidString)).Select(x => x.QuestionText).FirstOrDefault();
    }


    public void InitializeModal(ProductQuestionTemplateDto selectedProductQuestionTemplate, IReadOnlyList<QuestionTemplateDto> componentList)
    {
        _isModalOpen = false;
        GuidStringSelected = ProductQuestionTemplate.QuestionTemplateId.ToString();
        ProductQuestionTemplate = selectedProductQuestionTemplate.DeepClone();
        ComponentList = componentList;
    
        // Reset any other necessary fields
        TextProperty = "Name";  // Example of resetting another property
        StateHasChanged();
     }
    

    private void OnGuidStringSelectedChanged(string obj)
    {
        GuidStringSelected = obj;
        if(GuidStringSelected != null && GuidStringSelected != Guid.Empty.ToString() && ProductQuestionTemplate.Name.IsNullOrEmpty())
        {
            ProductQuestionTemplate.Name = ComponentName(GuidStringSelected);
        }
    }
}