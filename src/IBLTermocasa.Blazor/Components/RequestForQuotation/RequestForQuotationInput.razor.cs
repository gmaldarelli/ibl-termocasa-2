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
using IBLTermocasa.RequestForQuotations;
using IBLTermocasa.Shared;
using IBLTermocasa.Types;
using Microsoft.AspNetCore.Components;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;

namespace IBLTermocasa.Blazor.Components.RequestForQuotation;

public partial class RequestForQuotationInput
{
    private Validations RequestForQuotationValidations { get; set; } = new();

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
    private LookupDto<Guid> selectedAgentLookupDto = new();
    
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
        await LoadData();
        await InitializeSelectedValues();

        StateHasChanged();
    }

    private async Task LoadData()
    {
        try
        {
            isLoading = true; // Imposta isLoading su true prima di iniziare il caricamento

            // Esegui le chiamate ai servizi per recuperare i dati necessari
            Organizations =
                await OrganizationsAppService.GetFilterTypeAsync(new GetOrganizationsInput(),
                    OrganizationType.CUSTOMER);
            Contacts = await ContactsAppService.GetListAsync(new GetContactsInput());
            
            Catalogs = await CatalogsAppService.GetListAsync(new GetCatalogsInput());

            var mySelfId = CurrentPrincipalAccessor.Principal.FindUserId();
            var userList = await UserAppService.GetListAsync(new GetIdentityUsersInput());
            userLookUpDtos = userList.Items.Select(x => new LookupDto<Guid> { Id = x.Id, DisplayName = x.UserName })
                .ToList();
            selectedAgentLookupDto = userLookUpDtos.FirstOrDefault(x => x.Id == mySelfId)!;
            

            isLoading = false; // Imposta isLoading su false una volta che i dati sono stati caricati
        }
        catch (Exception ex)
        {
            // Gestisci eventuali eccezioni qui
            Console.WriteLine($"Si è verificato un errore durante il caricamento dei dati: {ex.Message}");
        }
    }
    
    private async Task InitializeSelectedValues()
    {
        SelectedOrganization = Organizations.Items.FirstOrDefault(x => x.Id == InternalRequestForQuotation.OrganizationId);
        SelectedContact = Contacts.Items.FirstOrDefault(x => x.Id == InternalRequestForQuotation.ContactId);
        SelectedAgent = userLookUpDtos.FirstOrDefault(x => x.Id == InternalRequestForQuotation.AgentId);
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

    protected override void OnParametersSet()
    {
        UpdateValueOrganization();
        UpdateValueContact();
        UpdateValueAgent();
        StateHasChanged();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            _isComponentRendered = true;
        }
    }

    private void UpdateValueOrganization()
    {
        if (SelectedOrganization == null)
        {
            InternalRequestForQuotation.OrganizationId = Guid.Empty;
            InternalRequestForQuotation.OrganizationPropertyDto = null;
            InternalRequestForQuotation.MailInfo = new MailInfoDto();
            InternalRequestForQuotation.PhoneInfo = new PhoneInfoDto();
        }
        else
        {
            InternalRequestForQuotation.OrganizationId = SelectedOrganization.Id;
            var organization = Organizations.Items.FirstOrDefault(x => x.Id == SelectedOrganization.Id);
            SelectedOrganization = organization;
            InternalRequestForQuotation.MailInfo = organization.MailInfo != null ? organization.MailInfo : new MailInfoDto();
            InternalRequestForQuotation.PhoneInfo = organization.PhoneInfo != null ? organization.PhoneInfo : new PhoneInfoDto();
            InternalRequestForQuotation.OrganizationPropertyDto =
                new OrganizationPropertyDto(organization.Id, organization.Name);
        }

        StateHasChanged();
    }

    private void UpdateValueContact()
    {
        if (SelectedContact == null)
        {
            InternalRequestForQuotation.ContactId = Guid.Empty;
            InternalRequestForQuotation.ContactPropertyDto = null;
        }
        else
        {
            InternalRequestForQuotation.ContactId = SelectedContact.Id;
            var contact = Contacts.Items.FirstOrDefault(x => x.Id == SelectedContact.Id);
            SelectedContact = contact;
            InternalRequestForQuotation.ContactPropertyDto =
                new ContactPropertyDto(contact.Id, contact.Name + " " + contact.Surname);
        }

        StateHasChanged();
    }

    private void UpdateValueAgent()
    {
        if (SelectedAgent == null)
        {
            InternalRequestForQuotation.AgentId = Guid.Empty;
        }
        else
        {
            InternalRequestForQuotation.AgentId = SelectedAgent.Id;
            SelectedAgent = userLookUpDtos.FirstOrDefault(user => user.Id == SelectedAgent.Id);
        }

        StateHasChanged();
    }
    
    private string GetCatalogItemName(Guid catalogItemId)
    {
        var catalogItem = ListProductName.FirstOrDefault(item => item.Item1 == catalogItemId);
        return catalogItem.Item2;
    }
    
    private Task<IEnumerable<OrganizationDto>> SearchOrganization(string value)
    {
        if (Organizations == null || Organizations.TotalCount == 0)
            return new Task<IEnumerable<OrganizationDto>>(() => new List<OrganizationDto>());

        // Se il testo è null o vuoto, mostra l'elenco completo
        return string.IsNullOrEmpty(value)
            ? Task.FromResult<IEnumerable<OrganizationDto>>(Organizations.Items.ToList())
            : Task.FromResult<IEnumerable<OrganizationDto>>(Organizations.Items.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase)).ToList());
    }

    private Task<IEnumerable<ContactDto>> SearchContact(string value)
    {
        if (Contacts == null || Contacts.Items.Count == 0)
            return new Task<IEnumerable<ContactDto>>(() => new List<ContactDto>());

        // Se il testo è null o vuoto, mostra l'elenco completo
        return string.IsNullOrEmpty(value)
            ? Task.FromResult<IEnumerable<ContactDto>>(Contacts.Items.ToList())
            : Task.FromResult<IEnumerable<ContactDto>>(Contacts.Items.Where(x => x.ToStringNameSurname().Contains(value, StringComparison.InvariantCultureIgnoreCase)).ToList());
    }

    private Task<IEnumerable<LookupDto<Guid>>> SearchAgent(string value)
    {
        if (userLookUpDtos == null || userLookUpDtos.Count == 0)
            return Task.FromResult<IEnumerable<LookupDto<Guid>>>(new List<LookupDto<Guid>>());

        // Se il testo è null o vuoto, mostra l'elenco completo
        return string.IsNullOrEmpty(value)
            ? Task.FromResult<IEnumerable<LookupDto<Guid>>>(userLookUpDtos.ToList())
            : Task.FromResult<IEnumerable<LookupDto<Guid>>>(userLookUpDtos.Where(x => x.DisplayName.Contains(value, StringComparison.InvariantCultureIgnoreCase)).ToList());
    }
    
}