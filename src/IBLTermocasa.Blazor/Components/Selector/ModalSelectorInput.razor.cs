using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using IBLTermocasa.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Volo.Abp.Application.Dtos;

namespace IBLTermocasa.Blazor.Components.Selector;

public partial class ModalSelectorInput<TItem> where TItem : EntityDto<Guid>
{
    [Parameter]
    public string ModalTitleResolver { get; set; } = null!;

    [Parameter] public IEnumerable<TItem> ComponentList { get; set; }
    [Parameter] public IEnumerable<TItem> SelectedComponentList { get; set; }
    [Parameter] public bool IsMultiSelection { get; set; }
    [Parameter] public string? ComponentLabelSingle { get; set; }
    [Parameter] public string? ComponentLabelPlural { get; set; }
    [Parameter] public EventCallback<IEnumerable<TItem>> OnSave { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }
    [Parameter] public string TextProperty { get; set; }
    public Modal ComponentModal { get; set; }
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
        if(ComponentList == null || SelectedComponentList == null)
        {
            return;
        }
        else
        {
            ComponentListLookupDto = ComponentList.Select(x => new LookupDto<Guid>()
                {
                    Id = x.Id,
                    DisplayName = resoverDisplayName(x)
                }
            );
            SelectedComponentListLookupDto = SelectedComponentList.Select(x => new LookupDto<Guid>()
                {
                    Id = x.Id,
                    DisplayName = resoverDisplayName(x)
                }
            );
        }
        
    }
    private void OnElementSelectorInputSelectedComponentsChanged(IEnumerable<LookupDto<Guid>> selectedComponents)
    {
        SelectedComponentListLookupDto = selectedComponents;
    }
    
    private string resoverDisplayName(TItem dto)
    {
        string displayName =dto.Id.ToString();
        try
        {
            displayName = dto.GetType().GetProperty(TextProperty).GetValue(dto).ToString();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error resolving display name: {e.Message}");
        }

        return displayName;
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
        //stampa id e nome dei componenti selezionati
        foreach (var item in updatedList)
        {
            Console.WriteLine($"Componente selezionato: {item.Id} - {item.GetType().GetProperty(TextProperty).GetValue(item)}");
        }
        OnSave.InvokeAsync(updatedList);
        StateHasChanged();
    }

    public void Hide()
    {
        _isModalOpen = false;
        ComponentModal.Hide();
        StateHasChanged();
    }
    
    public void Show()
    {
        _isModalOpen = true;
        ComponentModal.Show();
        StateHasChanged();
    }

    private void OnSelectorValueChanged(IEnumerable<Guid> selectedComponents)
    {
        // nothing to do
    }
}