using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Blazorise;
using Blazorise.DataGrid;
using Force.DeepCloner;
using IBLTermocasa.Catalogs;
using IBLTermocasa.Common;
using IBLTermocasa.Contacts;
using IBLTermocasa.Organizations;
using IBLTermocasa.Products;
using IBLTermocasa.QuestionTemplates;
using IBLTermocasa.RequestForQuotations;
using IBLTermocasa.Shared;
using IBLTermocasa.Types;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;

namespace IBLTermocasa.Blazor.Components.RequestForQuotation;

public partial class RequestForQuotationInput
{
    private List<LookupDto<Guid>> OrganizationsCollection { get; set; } = new();
    private List<LookupDto<Guid>> ContactsCollection { get; set; } = new();
    private List<LookupDto<Guid>> AgentsCollection { get; set; } = new();
    private LookupDto<Guid> selectedOrganizationLookupDto = new();
    private LookupDto<Guid> selectedContactLookupDto = new();
    private LookupDto<Guid> selectedAgentLookupDto = new();
    [CascadingParameter] MudDialogInstance Dialog { get; set; }
    [Parameter] public RequestForQuotationDto RequestForQuotation { get; set; } = new();
    [Parameter] public EventCallback<RequestForQuotationDto> OnRequestForQuotationSaved { get; set; }
    [Parameter] public EventCallback<RequestForQuotationDto> OnRequestForQuotationCancel { get; set; }
    [Parameter] public bool DisplayReadOnly { get; set; }
    [Inject] private IMapper _mapper { get; set; }
    [Inject] public IOrganizationsAppService OrganizationsAppService { get; set; }
    [Inject] public IContactsAppService ContactsAppService { get; set; }
    [Inject] public IRequestForQuotationsAppService RequestForQuotationsAppService { get; set; }
    [Inject] public IDialogService DialogService { get; set; }
    [Inject] public IQuestionTemplatesAppService QuestionTemplatesAppService { get; set; }
    
    private bool _isComponentRendered;
    private string maskDecimal = @"^(\d+,\d*|\d*)$";
    private IMask maskDate = new DateMask("dd/MM/yyyy");
    
    private RequestForQuotationUpdateDto EditingRequestForQuotation { get; set; } = new();
    private RequestForQuotationDto InternalRequestForQuotation = new();
    public RequestForQuotationItemDto selectedRequestForQuotationItem { get; set; }
    public DataGrid<RequestForQuotationItemDto> RequestForQuotationItemsDataGrid { get; set; }
    
    //è importante sapere che OnParametersSetAsync viene chiamato prima di tutti, anche di OnInitializedAsync()
    
    private string GetButtonText()
    {
        return DisplayReadOnly ? "Back" : "Cancel";
    }
    protected override async Task OnParametersSetAsync()
    {
        if (RequestForQuotation == null)
        {
            RequestForQuotation = new RequestForQuotationDto();
        }
        InternalRequestForQuotation = RequestForQuotation.DeepClone();
        selectedOrganizationLookupDto = new LookupDto<Guid>(InternalRequestForQuotation.OrganizationProperty.Id, InternalRequestForQuotation.OrganizationProperty.Name);
        selectedContactLookupDto = new LookupDto<Guid>(InternalRequestForQuotation.ContactProperty.Id, InternalRequestForQuotation.ContactProperty.Name);
        selectedAgentLookupDto = new LookupDto<Guid>(InternalRequestForQuotation.AgentProperty.Id, InternalRequestForQuotation.AgentProperty.Name);
        await LoadData();

        StateHasChanged();
    }

    private async Task LoadData()
    {
        try
        {
            // Esegui le chiamate ai servizi per recuperare i dati necessari
            AgentsCollection = (await RequestForQuotationsAppService.GetIdentityUserLookupAsync(new LookupRequestDto())).Items.ToList();
            OrganizationsCollection = (await RequestForQuotationsAppService.GetOrganizationLookupCustomerAsync(new LookupRequestDto())).Items.ToList();
            ContactsCollection = (await RequestForQuotationsAppService.GetContactLookupAsync(new LookupRequestDto())).Items.ToList();
        }
        catch (Exception ex)
        {
            // Gestisci eventuali eccezioni qui
            Console.WriteLine($"Si è verificato un errore durante il caricamento dei dati: {ex.Message}");
        }
    }

    private async Task HandleValidSubmit()
    {
        Console.WriteLine("RequestForQuotationInput: RequestForQuotationValidations");
        // copy RequestForQuotation properties to EditingRequestForQuotation with mapper

        Console.WriteLine(
            $"InternalRequestForQuotation.ConcurrencyStamp: {InternalRequestForQuotation.ConcurrencyStamp}");
        EditingRequestForQuotation =
            _mapper.Map<RequestForQuotationDto, RequestForQuotationUpdateDto>(InternalRequestForQuotation);
        Console.WriteLine(
            $"EditingRequestForQuotation.ConcurrencyStamp: {EditingRequestForQuotation.ConcurrencyStamp}");
        var result =
            await RequestForQuotationsAppService.UpdateAsync(InternalRequestForQuotation.Id,
                EditingRequestForQuotation);
        Console.WriteLine("RequestForQuotationInput: UpdateAsync");
        RequestForQuotation = _mapper.Map<RequestForQuotationDto>(result);
        InternalRequestForQuotation = RequestForQuotation.DeepClone();

        await OnRequestForQuotationSaved.InvokeAsync();
        Dialog.Close(DialogResult.Ok(result));
        StateHasChanged();
    }

    private void HandleCancel()
    { 
        Dialog.Cancel();
        StateHasChanged();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            _isComponentRendered = true;
        }
    } 
    
    private async void UpdateValueOrganization(LookupDto<Guid> arg)
    {
        if (arg == null)
        {
            selectedOrganizationLookupDto = new LookupDto<Guid>();
            InternalRequestForQuotation.OrganizationProperty = new OrganizationPropertyDto();
            InternalRequestForQuotation.MailInfo = new MailInfoDto();
            InternalRequestForQuotation.PhoneInfo = new PhoneInfoDto();
        }
        else
        {
            selectedOrganizationLookupDto = arg;
            var organization = await OrganizationsAppService.GetAsync(arg.Id);
            InternalRequestForQuotation.MailInfo = organization.MailInfo != null ? organization.MailInfo : new MailInfoDto();
            InternalRequestForQuotation.PhoneInfo = organization.PhoneInfo != null ? organization.PhoneInfo : new PhoneInfoDto();
            InternalRequestForQuotation.OrganizationProperty =
                new OrganizationPropertyDto(organization.Id, organization.Name);
        }

        StateHasChanged();
    }

    private async void UpdateValueContact(LookupDto<Guid> arg)
    {
        if (arg == null)
        {
            selectedContactLookupDto = new LookupDto<Guid>();
            InternalRequestForQuotation.ContactProperty = new ContactPropertyDto();
        }
        else
        {
            selectedContactLookupDto = arg;
            var contact = await ContactsAppService.GetAsync(arg.Id);
            InternalRequestForQuotation.ContactProperty =
                new ContactPropertyDto(contact.Id, contact.ToStringNameSurname());
        }

        StateHasChanged();
    }

    private void UpdateValueAgent(LookupDto<Guid> arg)
    {
        if (arg == null)
        {
            selectedAgentLookupDto = new LookupDto<Guid>();
            InternalRequestForQuotation.AgentProperty = new AgentPropertyDto();
        }
        else
        {
            selectedAgentLookupDto = arg;
            InternalRequestForQuotation.AgentProperty = new AgentPropertyDto(arg.Id, arg.DisplayName);
        }

        StateHasChanged();
    }
    
    private async Task<IEnumerable<LookupDto<Guid>>> SearchOrganization(string value, CancellationToken token)
    {
        if (OrganizationsCollection == null || OrganizationsCollection.Count == 0)
            return new List<LookupDto<Guid>>();

        return await Task.Run(() =>
        {
            return string.IsNullOrEmpty(value)
                ? OrganizationsCollection.ToList()
                : OrganizationsCollection
                    .Where(x => x.DisplayName.Contains(value, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }, token);
    }

    private async Task<IEnumerable<LookupDto<Guid>>> SearchContact(string value, CancellationToken token)
    {
        if (ContactsCollection == null || ContactsCollection.Count == 0)
            return new List<LookupDto<Guid>>();

        return await Task.Run(() =>
        {
            return string.IsNullOrEmpty(value)
                ? ContactsCollection.ToList()
                : ContactsCollection.Where(x => x.DisplayName.Contains(value, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }, token);
    }

    private async Task<IEnumerable<LookupDto<Guid>>> SearchAgent(string value, CancellationToken token)
    {
        if (AgentsCollection == null || AgentsCollection.Count == 0)
            return new List<LookupDto<Guid>>();

        return await Task.Run(() =>
        {
            return string.IsNullOrEmpty(value)
                ? AgentsCollection.ToList()
                : AgentsCollection.Where(x => x.DisplayName.Contains(value, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }, token);
    }
}