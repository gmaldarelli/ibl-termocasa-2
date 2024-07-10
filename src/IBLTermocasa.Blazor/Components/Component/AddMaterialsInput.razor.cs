using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IBLTermocasa.Materials;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using NUglify.Helpers;

namespace IBLTermocasa.Blazor.Components.Component;

public partial class AddMaterialsInput
{
    [CascadingParameter] private MudDialogInstance Dialog { get; set; }
    [Inject] public IMaterialsAppService MaterialsAppService { get; set; }

    [Parameter] public List<Guid> ExclusionIds { get; set; } = new List<Guid>();
    public HashSet<MaterialDto> SelectedItems { get; set; } = new HashSet<MaterialDto>();

    private IEnumerable<MaterialDto>? _items = new List<MaterialDto>();
    private string _searchString;

    protected override async Task OnInitializedAsync()
    {
        await Search();
    }

    private async Task Search()
    {
        GetMaterialsInput filterInput = new GetMaterialsInput()
        {
            MaxResultCount = 1000,
        };

        var result = await MaterialsAppService.GetListAsync(filterInput);
        _items = result.Items.Where(x => !ExclusionIds.Contains(x.Id) && 
                                         (string.IsNullOrWhiteSpace(_searchString) || 
                                          x.Code.Contains(_searchString, StringComparison.OrdinalIgnoreCase) || 
                                          x.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase)));
    }

    private void Submit()
    {
        Console.WriteLine($"SelectedItem: {SelectedItems.Count}");

        if (SelectedItems.Count > 0)
        {
            var selectedItemsList = SelectedItems.ToList();
            Dialog.Close(DialogResult.Ok(selectedItemsList));
        }
        else
        {
            Dialog.Cancel();
        }

        StateHasChanged();
    }

    private void Cancel()
    {
        Dialog.Cancel();
    }
}