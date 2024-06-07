using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IBLTermocasa.Blazor.Components.RequestForQuotation;
using IBLTermocasa.Permissions;
using IBLTermocasa.RequestForQuotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using Volo.Abp.Http.Client;

namespace IBLTermocasa.Blazor.Pages.Crm;

public partial class RequestForQuotationDetails

{
    [Inject]
    public IRequestForQuotationsAppService RequestForQuotationService { get; set; }
    [Inject]
    public IAuthorizationService AuthorizationService { get; set; }
    
    
    [Parameter] public string? Id { get; set; }
    
    
    protected List<BreadcrumbItem> BreadcrumbItems = new();
    protected PageToolbar Toolbar { get; } = new PageToolbar();
    private bool CanCreateRequestForQuotation { get; set; }
    private bool CanEditRequestForQuotation { get; set; }
    private bool CanDeleteRequestForQuotation { get; set; }
    private bool IsNew = true;
    private RequestForQuotationDto RequestForQuotation { get; set; }
    public RFQNewOrDraft RFQsNewOrDraftComponent { get; set; }


    protected override async Task OnInitializedAsync()
    {
        if(Id != null && Guid.TryParse(Id, out _) && Id != Guid.Empty.ToString())
        {
            IsNew = false;
            Guid id = Guid.Parse(Id);
            RequestForQuotation = await LoadRequestForQuotationAsync(id);
        }
        else
        {
            IsNew = true;
            RequestForQuotation = new RequestForQuotationDto();
        }
        await SetBreadcrumbItemsAsync();
        await SetPermissionsAsync();
    }

    protected virtual async Task<RequestForQuotationDto> LoadRequestForQuotationAsync(Guid id)
    {
        try
        {
            var result = await RequestForQuotationService.GetAsync(id);
            return result;
        }
        catch (AbpRemoteCallException e)
        {
            await UiMessageService.Warn(L["RequestForQuotationNotFound"]);
            NavigationManager.NavigateTo("/request-for-quotations");
        }
        return null;
    }   


    private async Task SetPermissionsAsync()
    {
        CanCreateRequestForQuotation = await AuthorizationService
            .IsGrantedAsync(IBLTermocasaPermissions.RequestForQuotations.Create);
        CanEditRequestForQuotation = await AuthorizationService
            .IsGrantedAsync(IBLTermocasaPermissions.RequestForQuotations.Edit);
        CanDeleteRequestForQuotation = await AuthorizationService
            .IsGrantedAsync(IBLTermocasaPermissions.RequestForQuotations.Delete);
    }

    protected virtual ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:RequestForQuotations"], "/request-for-quotations"));
        if (RequestForQuotation != null)
        {
            if (IsNew)
            {
                BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:NewRequestForQuotation"], "/rfq-create"));
            }
            else
            {
                BreadcrumbItems.Add(new BreadcrumbItem($"{L["Menu:RequestForQuotationDraft"]} - {RequestForQuotation.QuoteNumber} ", $"/rfq-draft/{RequestForQuotation.Id}"));
            }
        }
        return ValueTask.CompletedTask;
    }


    private async void HandleRequestForQuotationSaved(RequestForQuotationDto obj)
    {
        //TODO: Implement this method
    }

    private async void HandleRequestForQuotationCancel(RequestForQuotationDto obj)
    {
        //TODO: Implement this method
    }
}