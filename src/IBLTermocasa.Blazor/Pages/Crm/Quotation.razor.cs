using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IBLTermocasa.BillOfMaterials;
using IBLTermocasa.Permissions;
using IBLTermocasa.Quotations;
using IBLTermocasa.RequestForQuotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;

namespace IBLTermocasa.Blazor.Pages.Crm;

public partial class Quotation
{
    private QuotationDto? QuotationInput = new QuotationDto();
    public RequestForQuotationDto? RfqInput = new RequestForQuotationDto();
    private BillOfMaterialDto? BillOfMaterialsInput = new BillOfMaterialDto();
    private MudForm formQuotation;
    private bool successValidationQuotation;
    private string[] errorsValidationQuotation;
    private bool IsLoading = true;
    private bool CanCreateQuotation { get; set; }
    private bool CanEditQuotation { get; set; }
    private bool CanDeleteQuotation { get; set; }
    [Parameter] public string? Id { get; set; }
    public List<BreadcrumbItem> BreadcrumbItems { get; set; } = new();
    public PageToolbar? Toolbar { get; set; } = new PageToolbar();

    protected override async Task OnInitializedAsync()
    {
        await SetBreadcrumbItemsAsync();
        await SetPermissionsAsync();
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
            IsLoading = false;
        }
    }

    private void OnSentQuotation(MouseEventArgs obj)
    {
        throw new NotImplementedException();
    }
    
    private async Task SetPermissionsAsync()
    {
        CanCreateQuotation = await AuthorizationService
            .IsGrantedAsync(IBLTermocasaPermissions.Quotations.Create);
        CanEditQuotation = await AuthorizationService
            .IsGrantedAsync(IBLTermocasaPermissions.Quotations.Edit);
        CanDeleteQuotation = await AuthorizationService
            .IsGrantedAsync(IBLTermocasaPermissions.Quotations.Delete);
    }

    protected virtual ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:Quotations"], "/quotations"));
        if (QuotationInput != null)
        {
            BreadcrumbItems.Add(new BreadcrumbItem($"{L["Menu:Quotation"]} - {QuotationInput.Code} ", $"/quotation/{QuotationInput.Id}"));

        }
        return ValueTask.CompletedTask;
    }
}