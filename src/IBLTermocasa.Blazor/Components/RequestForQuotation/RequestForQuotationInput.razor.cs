using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
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
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;

namespace IBLTermocasa.Blazor.Components.RequestForQuotation;

public partial class RequestForQuotationInput
{
    private Validations RequestForQuotationValidations { get; set; } = new();
    private Modal RequestForQuotationModal { get; set; }
    private List<LookupDto<Guid>> OrganizationsCollection { get; set; } = new();
    private List<LookupDto<Guid>> ContactsCollection { get; set; } = new();
    private List<LookupDto<Guid>> AgentsCollection { get; set; } = new();
    private LookupDto<Guid> selectedOrganizationLookupDto = new();
    private LookupDto<Guid> selectedContactLookupDto = new();
    private LookupDto<Guid> selectedAgentLookupDto = new();
    [Inject] public IRequestForQuotationsAppService RequestForQuotationsAppService { get; set; }
    [Inject] public IQuestionTemplatesAppService QuestionTemplatesAppService { get; set; }
    
    
    
    
    
    
    
    
    
    private bool ReadOnly { get; set; } = false;

    private bool IsFullView { get; set; } = false;
    private RequestForQuotationCreateDto NewRequestForQuotation { get; set; } = new();

    private RequestForQuotationUpdateDto EditingRequestForQuotation { get; set; } = new();

    [Parameter] public RequestForQuotationDto RequestForQuotation { get; set; } = new();
    [Parameter] public EventCallback<RequestForQuotationDto> OnRequestForQuotationSaved { get; set; }
    [Parameter] public EventCallback<RequestForQuotationDto> OnRequestForQuotationCancel { get; set; }
    [Parameter] public bool DisplayReadOnly { get; set; }
    [Inject] private IMapper _mapper { get; set; }
    [Inject] public IOrganizationsAppService OrganizationsAppService { get; set; }
    [Inject] public IContactsAppService ContactsAppService { get; set; }
    [Inject] public ICatalogsAppService CatalogsAppService { get; set; }
    [Inject] public ICurrentPrincipalAccessor CurrentPrincipalAccessor { get; set; }
    [Inject] public IIdentityUserAppService UserAppService { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }


    private RequestForQuotationDto InternalRequestForQuotation = new();
    private PagedResultDto<CatalogWithNavigationPropertiesDto> Catalogs { get; set; } = new();
    private PagedResultDto<OrganizationDto> Organizations { get; set; } = new();
    private PagedResultDto<ContactDto> Contacts { get; set; } = new();
    public RequestForQuotationItemDto selectedRequestForQuotationItem { get; set; }
    public DataGrid<RequestForQuotationItemDto> RequestForQuotationItemsDataGrid { get; set; }
    
    private List<ProductDto> CatalogItemFilter { get; set; } = new();
    public List<(Guid, string?)> ListQuestionTemplate = new();
    private List<(Guid, string)> ListProductName = new();

    List<Guid> listGuidCatalogItems = new();
    private List<LookupDto<Guid>> userLookUpDtos = new();
    
    private OrganizationDto SelectedOrganization { get; set; }
    private ContactDto SelectedContact { get; set; }
    private LookupDto<Guid> SelectedAgent { get; set; }

    private bool isLoading = true;
    private bool _isComponentRendered = false;
    
    //è importante sapere che OnParametersSetAsync viene chiamato prima di tutti, anche di OnInitializedAsync()
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
            isLoading = true; // Imposta isLoading su true prima di iniziare il caricamento
            // Esegui le chiamate ai servizi per recuperare i dati necessari
            AgentsCollection = (await RequestForQuotationsAppService.GetIdentityUserLookupAsync(new LookupRequestDto())).Items.ToList();
            OrganizationsCollection = (await RequestForQuotationsAppService.GetOrganizationLookupCustomerAsync(new LookupRequestDto())).Items.ToList();
            ContactsCollection = (await RequestForQuotationsAppService.GetContactLookupAsync(new LookupRequestDto())).Items.ToList();
            isLoading = false; // Imposta isLoading su false una volta che i dati sono stati caricati
        }
        catch (Exception ex)
        {
            // Gestisci eventuali eccezioni qui
            Console.WriteLine($"Si è verificato un errore durante il caricamento dei dati: {ex.Message}");
        }
    }

    private async Task HandleValidSubmit()
    {
        if (await RequestForQuotationValidations.ValidateAll() == false)
        {
            return;
        }

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

        if (_isComponentRendered)
        {
            await OnRequestForQuotationSaved.InvokeAsync(RequestForQuotation);
            Console.WriteLine("RequestForQuotationInput: InvokeAsync");
        }
    }

    private async Task HandleCancel()
    {
        if (ReadOnly)
        {
            NavigationManager.NavigateTo("/request-for-quotations");
        }
        else
        {
            await OnRequestForQuotationCancel.InvokeAsync(RequestForQuotation);
        }
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
    
    private string GetCatalogItemName(Guid catalogItemId)
    {
        var catalogItem = ListProductName.FirstOrDefault(item => item.Item1 == catalogItemId);
        return catalogItem.Item2;
    }
    
    private async Task<IEnumerable<LookupDto<Guid>>> SearchOrganization(string value)
    {
        if (OrganizationsCollection == null || OrganizationsCollection.Count == 0)
            return new List<LookupDto<Guid>>();

        // Se il testo è null o vuoto, mostra l'elenco completo
        return string.IsNullOrEmpty(value)
            ? OrganizationsCollection.ToList()
            : OrganizationsCollection
                .Where(x => x.DisplayName.Contains(value, StringComparison.InvariantCultureIgnoreCase)).ToList();
    }

    private async Task<IEnumerable<LookupDto<Guid>>> SearchContact(string value)
    {
        if (ContactsCollection == null || ContactsCollection.Count == 0)
            return new List<LookupDto<Guid>>();

        // Se il testo è null o vuoto, mostra l'elenco completo
        return string.IsNullOrEmpty(value)
            ? ContactsCollection.ToList()
            : ContactsCollection.Where(x => x.DisplayName.Contains(value, StringComparison.InvariantCultureIgnoreCase)).ToList();
    }

    private async Task<IEnumerable<LookupDto<Guid>>> SearchAgent(string value)
    {
        if (AgentsCollection == null || AgentsCollection.Count == 0)
            return new List<LookupDto<Guid>>();

        // Se il testo è null o vuoto, mostra l'elenco completo
        return string.IsNullOrEmpty(value)
            ? AgentsCollection.ToList()
            : AgentsCollection.Where(x => x.DisplayName.Contains(value, StringComparison.InvariantCultureIgnoreCase)).ToList();
    }
}