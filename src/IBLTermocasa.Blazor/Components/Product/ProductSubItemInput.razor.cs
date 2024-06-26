using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IBLTermocasa.Common;
using IBLTermocasa.Products;
using IBLTermocasa.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace IBLTermocasa.Blazor.Components.Product;

public partial class ProductSubItemInput
{
    
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    [Parameter] public string? DialogTitle { get; set; }
    [Parameter] public string? ElementName { get; set; }
    [Parameter] public int? OrderMaxValue { get; set; } = 100;
    [Parameter] public int? OrderMinValue { get; set; } = 1;
    [Parameter] public Type? ElementType { get; set; } 
    [Parameter] public IEnumerable<ExtendedLookUpDto<Guid>>? ElementListLookupDto { get; set; }
    
    private ExtendedLookUpDto<Guid>? SelectedElement { get; set; }
    protected MudSelect<ExtendedLookUpDto<Guid>>? MudSelectElement { get; set; }
    public int? SelectedOrder { get; set; }
    public string? SelectedCode { get; set; }
    public string? SelectedName { get; set; }
    public bool? SelectedIsMandatory { get; set; }
    public MudForm form { get; set; } = new MudForm();
    public bool success { get; set; }
    public string[] errors { get; set; } = Array.Empty<string>();

    protected override Task OnParametersSetAsync()
    {
        if(ElementListLookupDto == null)
        {
            return base.OnParametersSetAsync();
        }
        return Task.CompletedTask;
    }


    private void OnItemSelected(ExtendedLookUpDto<Guid>? lookupDto)
    {
        if (lookupDto == null)
        {
            SelectedElement = null;
            return;
        }
        SelectedElement = lookupDto;
        SelectedOrder  = OrderMaxValue;
        SelectedCode = lookupDto.ViewElementDto.Properties.FirstOrDefault(x => x.Name == "Code")?.Value.ToString() ?? "";
        SelectedName = lookupDto.DisplayName;
        SelectedIsMandatory = (bool)(lookupDto.ViewElementDto.Properties.FirstOrDefault(x => x.Name == "IsMandatory")?.Value ?? true);
    }
    private Task<IEnumerable<ExtendedLookUpDto<Guid>>> SearchElement(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Task.FromResult(ElementListLookupDto);
        }
        var lookupDtos = ElementListLookupDto.Where(x => x.DisplayName.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        return Task.FromResult(lookupDtos);
    }

    private void Cancel()
    {
        MudDialog.Cancel();
    }

    private async Task OnValidSubmit()
    {
        
        await form.Validate();
        if(errors.Length > 0)
        {
            return;
        }
        SelectedElement = new ExtendedLookUpDto<Guid>();
        SelectedElement.DisplayName = SelectedName;
        SelectedElement.Id = Guid.NewGuid();
        SelectedElement.ViewElementDto.Properties =
        [
            new ViewElementPropertyDto<object>("Code", SelectedCode),
            new ViewElementPropertyDto<object>("IsMandatory", SelectedIsMandatory),
            new ViewElementPropertyDto<object>("Order", SelectedOrder),
            new ViewElementPropertyDto<object>("ElementType", ElementType)
        ];
        MudDialog.Close(DialogResult.Ok(SelectedElement));

    }
}