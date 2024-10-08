﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IBLTermocasa.RequestForQuotations;
using IBLTermocasa.Types;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Volo.Abp.BlazoriseUI.Components;

namespace IBLTermocasa.Blazor.Components.BillOfMaterial;

public partial class BillOfMaterialFromRrq
{
    [Inject]
    public IDialogService DialogService { get; set; }
    public IReadOnlyList<RequestForQuotationWithNavigationPropertiesDto> RequestForQuotationList { get; set; }
    public MudDataGrid<RequestForQuotationWithNavigationPropertiesDto> RequestForQuotationDataGrid { get; set; }

    private bool nestedVisible = false;
    public DialogOptions ConfirmGenerationMudDialogOption { get; set; } = new()
    {
        FullWidth= true,
        MaxWidth=MaxWidth.Medium,
        CloseButton= true,
        BackdropClick = true,
        NoHeader=false,
        Position=DialogPosition.Custom,
        CloseOnEscapeKey=false
    };
    public string ConfirmGenerationMudDialogText { get; set; }

    private async Task GenerateBillOfMaterial(RequestForQuotationWithNavigationPropertiesDto item)
    {
        var result = await BillOfMaterialsAppService.GenerateBillOfMaterial(item.RequestForQuotation.Id);
        string? bomNumber = result.First(x => x.Name == "BomNumber").Value.ToString();
        var parameters = new DialogParameters<ConfirmGenerationMudDialog>
        {
            { x => x.Message, L["ConfirmGenerationMudDialogMessage", bomNumber] }
        };

        var dialog = await DialogService.ShowAsync<ConfirmGenerationMudDialog>(L["ConfirmGenerationMudDialogTitle"], parameters,ConfirmGenerationMudDialogOption);
        var confirmationResult = await dialog.Result;
        if(confirmationResult.Canceled)
        {
            await LoadRequestForQuotations();
            await RequestForQuotationDataGrid.ReloadServerData();
            StateHasChanged();
        }else
        {
            NavigationManager.NavigateTo($"/bill-of-materials-detail/{bomNumber}");
        }
    }

    private async Task LoadRequestForQuotations()
    {

        RequestForQuotationList = (await RequestForQuotationsAppService.GetListAsync(new GetRequestForQuotationsInput(){Status = RfqStatus.NEW})).Items;
        await RequestForQuotationDataGrid.ReloadServerData();
        StateHasChanged();
    }

    protected override void OnParametersSet()
    {
        {
            _ = LoadRequestForQuotations();
            Console.WriteLine("OnParametersSet RequestForQuotationList.Count: " + RequestForQuotationList.Count);
            base.OnParametersSet();
        }
    }
}