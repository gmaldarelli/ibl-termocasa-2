using System;
using System.Threading.Tasks;
using IBLTermocasa.BillOfMaterials;
using IBLTermocasa.Quotations;
using IBLTermocasa.RequestForQuotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace IBLTermocasa.Blazor.Pages.Crm;

public partial class Quotation
{
    private QuotationDto QuotationInput = new QuotationDto();
    public RequestForQuotationDto RfqInput = new RequestForQuotationDto();
    private BillOfMaterialDto BillOfMaterialsInput = new BillOfMaterialDto();
    [Parameter] public string? Id { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }
    
    protected override async Task OnParametersSetAsync()
    {
        
        if(Id != null && Guid.TryParse(Id, out _) && Id != Guid.Empty.ToString())
        {
            var quotation = await QuotationsAppService.GetAsync(Guid.Parse(Id));
            if(quotation != null)
            {
                QuotationInput = quotation;
                RfqInput = await RequestForQuotationsAppService.GetAsync(quotation.IdRFQ);
                BillOfMaterialsInput = await BillOfMaterialsAppService.GetAsync(quotation.IdBOM);
            }
        }
    }

    private void OnSentQuotation(MouseEventArgs obj)
    {
        throw new NotImplementedException();
    }
}