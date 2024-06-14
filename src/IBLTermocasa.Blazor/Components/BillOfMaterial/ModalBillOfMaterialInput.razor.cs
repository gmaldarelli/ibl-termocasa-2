using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Blazorise.Bootstrap5;
using IBLTermocasa.RequestForQuotations;
using IBLTermocasa.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using IBLTermocasa.BillOfMaterials;

namespace IBLTermocasa.Blazor.Components.BillOfMaterial;

public partial class ModalBillOfMaterialInput
{
    [Inject] private IMapper _mapper { get; set; }
    [Inject] public IRequestForQuotationsAppService RequestForQuotationsAppService { get; set; }
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    [Parameter] public EventCallback<BillOfMaterialDto> OnSave { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }
    public MudSelect<string> MudSelectComponent { get; set; }
    private List<LookupDto<Guid>> RequestForQuotationsCollection { get; set; }
    private LookupDto<Guid> selectedRFQLookupDto { get; set; }
    private RequestForQuotationDto SelectedRequestForQuotation { get; set; }
    public Modal BillOfMaterialModal { get; set; }
    private bool _isModalOpen;


    protected override async Task OnParametersSetAsync()
    {
        RequestForQuotationsCollection = (await RequestForQuotationsAppService.GetRequestForQuotationLookupAsync(new LookupRequestDto())).Items.ToList();
    }
    
    private async Task<IEnumerable<LookupDto<Guid>>> SearchRFQ(string value)
    {
        if (RequestForQuotationsCollection == null || RequestForQuotationsCollection.Count == 0)
            return new List<LookupDto<Guid>>();
        
        return string.IsNullOrEmpty(value)
            ? RequestForQuotationsCollection.ToList()
            : RequestForQuotationsCollection
                .Where(x => x.DisplayName.Contains(value, StringComparison.InvariantCultureIgnoreCase)).ToList();
    }
    
    private async void UpdateValueRequestForQuotation(LookupDto<Guid> arg)
    {
        if (arg == null)
        {
            selectedRFQLookupDto = new LookupDto<Guid>();
        }
        else
        {
            SelectedRequestForQuotation = await RequestForQuotationsAppService.GetAsync(arg.Id);
        }

        StateHasChanged();
    }
    
    private void Cancel()
    {
        MudDialog.Cancel(); 
    }

    private void Generate()
    {
        
    }
    
    public void Hide()
    {
        _isModalOpen = false;
        BillOfMaterialModal.Hide();
    }
    
    public void Show()
    {
        _isModalOpen = true;
        BillOfMaterialModal.Show();
        StateHasChanged();
    }
}