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
    [Inject] public IMaterialsAppService MaterialsAppService { get; set; }

    [CascadingParameter] private MudDialogInstance MudDialogComponent { get; set; } = new();
    [Parameter] public List<Guid> ExclusionIds { get; set; } = new List<Guid>();
    public HashSet<MaterialDto> SelectedItems { get; set; } = new HashSet<MaterialDto>();
    public MaterialDto SelectedItem { get; set; }

    IEnumerable<MaterialDto>? _items = new List<MaterialDto>();
    private string _searchString;
    
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        GetMaterialsInput filterInput = new GetMaterialsInput()
        {
            MaxResultCount = 10,
        };
        await Search();
        StateHasChanged(); 
    }
   
    private Func<MaterialDto, bool> _quickFilter => x =>
    {   
        
        if (string.IsNullOrWhiteSpace(_searchString))
            return true;

        if (x.Code.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        if (x.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    };

    public MudDataGrid<MaterialDto> MaterialMudDataGrid { get; set; }

    private async Task Search()
    {
        GetMaterialsInput filterInput = new GetMaterialsInput()
        {
            MaxResultCount = 1000,
        };
        _items = (await MaterialsAppService.GetListAsync(filterInput)).Items;
        List<MaterialDto> _itemsTemp = new List<MaterialDto>();
        _items.ForEach(x =>
        {
            if(!ExclusionIds.Contains(x.Id))
            {
                _itemsTemp.Add(x);
            }
        });
        //_items = _itemsTemp;
    }
    private void Submit()
    {
        Console.WriteLine($":::::::::::::::::::::::::::::::::::::::::::::::SelectedItem: {SelectedItems.Count}");
       
        if(SelectedItems.Count > 0 && MudDialogComponent != null)
        {
            MudDialogComponent.Close(DialogResult.Ok(SelectedItems));
        }
        else
        {
            if(MudDialogComponent != null)
            {
                MudDialogComponent.Cancel();
            }
            else
            {
                Console.WriteLine($":::::::::::::::::::::::::::::::::::::::::::::::MudDialog: {MudDialogComponent.ToString()}");
            
            }
        }
        
    }

    private void Cancel()
    {
        MudDialogComponent.Cancel();
    }
}