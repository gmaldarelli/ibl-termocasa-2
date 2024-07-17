using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IBLTermocasa.BillOfMaterials;
using IBLTermocasa.Blazor.Components.RequestForQuotation;
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
    public bool IsDepositRequired => QuotationInput is { DepositRequired: false };
    public MudDataGrid<QuotationItemDto> QuotationProductMudDataGrid { get; set; }
    public int DesideredMarkup { get; set; }

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
                DesideredMarkup = (int)quotation.QuotationItems!.Average(x => x.MarkUp);
               
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

    private async Task OpenRfqDetailAsync()
    {
        var parameters = new DialogParameters
        {
            { "RequestForQuotation", RfqInput },
            { "DisplayReadOnly", true }
        };

        var dialog = await DialogService.ShowAsync<RequestForQuotationInput>(L["RequestForQuotation"],
            parameters, new DialogOptions
            {
                Position = DialogPosition.Custom,
                FullWidth = true,
                MaxWidth = MaxWidth.Medium
            });
        var result = await dialog.Result;
        StateHasChanged();
    }

    private DateTime? ToNullableDateTime(DateTime creationTime)
    {
        return creationTime == DateTime.MinValue ? null : creationTime;
    }

    private void OnChangeDiscount()
    {
        foreach (var quotationItem in QuotationInput.QuotationItems)
        {
            quotationItem.Discount = (double)QuotationInput.Discount;
            quotationItem.SellingPrice = quotationItem.TotalCost + (quotationItem.TotalCost * quotationItem.MarkUp / 100);
            quotationItem.FinalSellingPrice = quotationItem.SellingPrice - (quotationItem.SellingPrice * quotationItem.Discount / 100);
        }
        QuotationProductMudDataGrid.ReloadServerData();
        StateHasChanged();
    }
    
    private void OnChangeMarkUp(MouseEventArgs obj)
    {
        foreach (var quotationItem in QuotationInput.QuotationItems)
        {
            quotationItem.MarkUp = QuotationInput.MarkUp ?? 0;
            quotationItem.SellingPrice = quotationItem.TotalCost + (quotationItem.TotalCost * quotationItem.MarkUp / 100);
            quotationItem.FinalSellingPrice = quotationItem.SellingPrice - (quotationItem.SellingPrice * quotationItem.Discount / 100);
        }
        QuotationProductMudDataGrid.ReloadServerData();
        StateHasChanged();
    }

    private void OnSaveQuotation(MouseEventArgs obj)
    {
        var updateDto = ObjectMapper.Map<QuotationDto, QuotationUpdateDto>(QuotationInput!);
        QuotationsAppService.UpdateAsync(QuotationInput!.Id, updateDto);
        UiMessageService.Success(L["QuotationUpdated"]);
    }
}