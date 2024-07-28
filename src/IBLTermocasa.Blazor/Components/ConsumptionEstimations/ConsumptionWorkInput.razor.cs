using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using IBLTermocasa.Common;
using IBLTermocasa.ConsumptionEstimations;
using IBLTermocasa.ProfessionalProfiles;
using IBLTermocasa.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace IBLTermocasa.Blazor.Components.ConsumptionEstimations;

public partial class ConsumptionWorkInput
{
    [Inject] private IProfessionalProfilesAppService ProfessionalProfilesAppService { get; set; }
    public MudForm _form { get; set; } = new MudForm();
    private bool _success { get; set; }
    private string[] _errors { get; set; } = Array.Empty<string>();
    private readonly CultureInfo _cultureInfo = new CultureInfo("it-IT");
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    [Parameter] public List<LookupDto<Guid>> ProductListLookup { get; set; } = new();
    [Parameter] public List<LookupDto<Guid>> ProfessionalProfilesListLookup { get; set; } = new();
    [Parameter] public ConsumptionWorkDto ConsumptionWork { get; set; } = new();
    
    private async Task OnConsumptionProfessionalSelected(LookupDto<Guid>? lookupDto)
    {
        ConsumptionProfessionalSelected = lookupDto;
        if (lookupDto == null)
        {
            return;
        }
        var consumptionProfessionalSelected = await ProfessionalProfilesAppService.GetAsync(lookupDto.Id);
        ConsumptionWork.IdProfessionalProfile = consumptionProfessionalSelected.Id;
        ConsumptionWork.Code = consumptionProfessionalSelected.Code;
        ConsumptionWork.Name = consumptionProfessionalSelected.Name;
        ConsumptionWork.Price = consumptionProfessionalSelected.StandardPrice;
        StateHasChanged();
    }

    private Task<IEnumerable<LookupDto<Guid>>> SearchConsumptionProfessional(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Task.FromResult<IEnumerable<LookupDto<Guid>>>(ProfessionalProfilesListLookup);
        }
        var lookupDtos = ProfessionalProfilesListLookup.Where(x => x.DisplayName.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        return Task.FromResult(lookupDtos);
    }

    private void Cancel(MouseEventArgs obj)
    {
        MudDialog.Cancel();
    }

    private async void OnValidSubmit(MouseEventArgs obj)
    {
        await _form.Validate();
        if(_errors.Length > 0)
        {
            return;
        }
        if(ConsumptionWork.CostType == CostType.FIXED_FOR_WORK)
        {
            ConsumptionWork.ProductId = Guid.Empty;
        }
        MudDialog.Close(DialogResult.Ok(ConsumptionWork));
    }

    protected override Task OnParametersSetAsync()
    {
        if (ConsumptionWork.IdProfessionalProfile != Guid.Empty)
        {
            ConsumptionProfessionalSelected = ProfessionalProfilesListLookup.FirstOrDefault(x => x.Id == ConsumptionWork.IdProfessionalProfile);
        }
        if(ConsumptionWork.CostType == null)
        {
            ConsumptionWork.CostType = CostType.CALCULATED_FOR_PRODUCT;
            IsProductDependent = true;
        }
        SelectedCostTypeIdString = ConsumptionWork.CostType.ToString();
        return base.OnParametersSetAsync();
    }

    public LookupDto<Guid>? ConsumptionProfessionalSelected { get; set; }
    public string? SelectedCostTypeIdString { get; set; }
    public bool IsProductDependent { get; set; }

    private void OnSelectedCostTypeChanged(string? costTypeIdString)
    {
        Enum.TryParse<CostType>(costTypeIdString, out var costType);
        ConsumptionWork.CostType = costType;
        SelectedCostTypeIdString = costTypeIdString;
        IsProductDependent = costType is CostType.CALCULATED_FOR_PRODUCT or CostType.FIXED_FOR_PRODUCT;
        
    }
}