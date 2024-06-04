using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using IBLTermocasa.Blazor.Components.Selector;
using IBLTermocasa.Components;
using IBLTermocasa.Products;
using IBLTermocasa.QuestionTemplates;
using IBLTermocasa.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using NUglify.Helpers;

namespace IBLTermocasa.Blazor.Components.Product;

public partial class ModalProductQuestionTemplateInput
{
    
    [Parameter] public IEnumerable<QuestionTemplateDto> ComponentList { get; set; }
    [Parameter] public ProductQuestionTemplateDto ProductQuestionTemplate { get; set; }
    [Parameter] public EventCallback<ProductQuestionTemplateDto> OnSave { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    [Parameter] public int MaxValue { get; set; } = 100;
    [Parameter] public int MinValue { get; set; } = 1;
    
    private bool IsMultiSelection = false;
    private string ProductLabelSingle = "";
    private string ProductLabelPlural = "";
    private string TextProperty = "Name";
    public Modal ProductQuestionTemplateModal { get; set; }
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
        if(ComponentList == null || ProductQuestionTemplate == null)
        {
            return;
        }
        else
        {
            ComponentListLookupDto = ComponentList.Select(x => new LookupDto<Guid>()
                {
                    Id = x.Id,
                    DisplayName = $"{x.QuestionText}"
                }
            );
            SelectedComponentListLookupDto = new List<LookupDto<Guid>>();
            
            if(ProductQuestionTemplate.QuestionTemplate != null)
            {
                new LookupDto<Guid>()
                {
                    Id = ProductQuestionTemplate.QuestionTemplate.Id,
                    DisplayName = $"{ProductQuestionTemplate.QuestionTemplate.QuestionText}"
                };
            };
        }
        ProductLabelSingle = L["Product"];
        ProductLabelPlural = L["Products"];
    }

    private string ResoverDisplayName(Guid id)
    {
        var component = ComponentList.FirstOrDefault(x => x.Id == id);
        return (component != null) ? $"{component?.QuestionText}" : id.ToString();
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
            ProductQuestionTemplate.QuestionTemplate = updatedList.First();
        }
        else
        {
            //TODO: Show validation error
        }
        OnSave.InvokeAsync(ProductQuestionTemplate);
        StateHasChanged();
    }

    public void Hide()
    {
        _isModalOpen = false;
        ProductQuestionTemplateModal.Hide();
        StateHasChanged();
    }
    
    public void Show()
    {
        _isModalOpen = true;
        ProductQuestionTemplateModal.Show();
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