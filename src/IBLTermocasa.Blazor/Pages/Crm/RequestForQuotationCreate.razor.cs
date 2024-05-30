using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using AutoMapper;
using Blazorise;
using Blazorise.Components;
using Blazorise.Extensions;
using IBLTermocasa.Catalogs;
using IBLTermocasa.Common;
using IBLTermocasa.Contacts;
using IBLTermocasa.Organizations;
using IBLTermocasa.Permissions;
using IBLTermocasa.Products;
using IBLTermocasa.QuestionTemplates;
using IBLTermocasa.RequestForQuotations;
using IBLTermocasa.Shared;
using IBLTermocasa.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;

namespace IBLTermocasa.Blazor.Pages.Crm;

public partial class RequestForQuotationCreate
{
    [Inject] private IMapper _mapper { get; set; }
    [Inject] public IIdentityUserAppService UserAppService { get; set; }
    [Inject] public IRequestForQuotationsAppService RequestForQuotationsAppService { get; set; }
    [Inject] public INotificationService notificationService { get; set; }
    [Inject] public IOrganizationsAppService OrganizationsAppService { get; set; }
    [Inject] public IQuestionTemplatesAppService QuestionTemplatesAppService { get; set; }
    [Inject] public IContactsAppService ContactsAppService { get; set; }
    [Inject] public ICurrentUser CurrentUser { get; set; }
    [Inject] public ICurrentPrincipalAccessor CurrentPrincipalAccessor { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public ICatalogsAppService CatalogsAppService { get; set; }
    [Inject] public IProductsAppService ProductsAppService { get; set; }
    protected List<BreadcrumbItem> BreadcrumbItems = new();

    private Validations RequestForQuotationValidations { get; set; } = new();
    private bool CanCreateRequestForQuotation { get; set; }
    public RequestForQuotationDto NewRequestForQuotationDto = new();
    public RequestForQuotationCreateDto NewRequestForQuotation { get; set; }
    public CatalogWithNavigationPropertiesDto SelectedCatalog { get; set; }
    private PagedResultDto<CatalogWithNavigationPropertiesDto> Catalogs { get; set; } = new();
    private PagedResultDto<OrganizationDto> Organizations { get; set; } = new();
    private PagedResultDto<ContactDto> Contacts { get; set; } = new();
    private List<RFQProductAndQuestionDto> RFQProductAndQuestions { get; set; } = new();

    private LookupDto<Guid> selectedAgentLookupDto = new();
    private bool disableContact = true;
    private int indexCatalog;

    private List<LookupDto<Guid>> userLookUpDtos = new();
    public MudAutocomplete<CatalogDto> CatalogAutocompleteRef { get; set; }
    private MudAutocomplete<ContactDto> ContactsAutoCompleteRef { get; set; }
    private ProductDto SelectedProduct { get; set; }
    private List<ProductDto> SelectedCatalogProducts { get; set; }
    
    [Range(1, int.MaxValue)]
    private int SelectedCatalogItemQuantity { get; set; } = 1;
    private List<RequestForQuotationItemDto> ListRequestForQuotationItems { get; set; } = new();
    private List<QuestionTemplateDto> ListQuestionTemplateSingleProduct { get; set; } = new();
    private RequestForQuotationItemDto selectedListViewItem { get; set; }

    private List<QuestionTemplateModel> QuestionTemplateValues = new();
    private string selectedStep = "1";
    private Steps stepsRef;
    private bool isSelecting = false;
    private string catalogNameSelected;
    private string productNameSelected;

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        Organizations =
            await OrganizationsAppService.GetFilterTypeAsync(new GetOrganizationsInput(), OrganizationType.CUSTOMER);
        Contacts = await ContactsAppService.GetListAsync(new GetContactsInput());
        Catalogs = await CatalogsAppService.GetListAsync(new GetCatalogsInput());
        var mySelfId = CurrentPrincipalAccessor.Principal.FindUserId();
        var userList = await UserAppService.GetListAsync(new GetIdentityUsersInput());
        //riempi con il contenuto di  userlist  una variabile userLookUpDtos di tipo List<LookUpDto>
        userLookUpDtos = userList.Items.Select(x => new LookupDto<Guid> { Id = x.Id, DisplayName = x.UserName })
            .ToList();
        NewRequestForQuotationDto.AgentId = (Guid)mySelfId!;
        selectedAgentLookupDto = userLookUpDtos.FirstOrDefault(x => x.Id == mySelfId)!;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await SetBreadcrumbItemsAsync();
            StateHasChanged();
        }
    }

    protected virtual ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new BreadcrumbItem(L["Menu:RequestForQuotations"]));
        return ValueTask.CompletedTask;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateRequestForQuotation =
            await AuthorizationService.IsGrantedAsync(IBLTermocasaPermissions.RequestForQuotations.Create);
    }

    private void UpdateValueAgent(LookupDto<Guid> arg)
    {
        if (arg == null)
        {
            NewRequestForQuotationDto.AgentId = Guid.Empty;
        }
        else
        {
            NewRequestForQuotationDto.AgentId = arg.Id;
        }

        StateHasChanged();
    }

    private async void UpdateValueOrganization(OrganizationDto arg)
    {
        if (arg == null)
        {
            NewRequestForQuotationDto.OrganizationId = Guid.Empty;
            NewRequestForQuotationDto.OrganizationPropertyDto = null;
            disableContact = true;
            NewRequestForQuotationDto.MailInfo = new MailInfoDto();
            NewRequestForQuotationDto.PhoneInfo = new PhoneInfoDto();
            ContactsAutoCompleteRef.Clear();
        }
        else
        {
            disableContact = false;
            NewRequestForQuotationDto.OrganizationId = arg.Id;
            var organization = Organizations.Items.FirstOrDefault(x => x.Id == arg.Id);
            NewRequestForQuotationDto.MailInfo = organization.MailInfo != null ? organization.MailInfo : new MailInfoDto();
            NewRequestForQuotationDto.PhoneInfo = organization.PhoneInfo != null ? organization.PhoneInfo : new PhoneInfoDto();
            NewRequestForQuotationDto.OrganizationPropertyDto =
                new OrganizationPropertyDto(organization.Id, organization.Name);
            //Bisogna mettere in attesa il thread per 100 millisecondi per evitare che il focus venga perso, perchè sulla stessa riga
            await Task.Delay(100);
            await ContactsAutoCompleteRef.FocusAsync();
        }

        StateHasChanged();
    }

    private void UpdateValueContact(ContactDto arg)
    {
        if (arg == null)
        {
            NewRequestForQuotationDto.ContactId = Guid.Empty;
            NewRequestForQuotationDto.ContactPropertyDto = null;
        }
        else
        {
            NewRequestForQuotationDto.ContactId = arg.Id;
            var contact = Contacts.Items.FirstOrDefault(x => x.Id == arg.Id);
            NewRequestForQuotationDto.ContactPropertyDto = new ContactPropertyDto(contact.Id, contact.ToStringNameSurname());
        }

        StateHasChanged();
    }

    private void UpdateValueCatalog(CatalogDto arg)
    {
        if (arg == null)
        {
            SelectedCatalog = null;
            SelectedProduct = null;
            SelectedCatalogItemQuantity = 1;
            QuestionTemplateValues.Clear();
        }
        else
        {
            SelectedProduct = null;
            disableContact = false;
            QuestionTemplateValues.Clear();
            SelectedCatalog = Catalogs.Items.FirstOrDefault(x => x.Catalog.Id == arg.Id);
            SelectedCatalogProducts = SelectedCatalog.Products;
        }
        StateHasChanged();
    }
    
    private void UpdateValueProduct(ProductDto arg)
    {
        if (arg == null)
        {
            QuestionTemplateValues.Clear();
            SelectedProduct = null;
        }
        else
        {
            SelectedProduct = SelectedCatalogProducts.FirstOrDefault(x => x.Id == arg.Id)!;
            if (!SelectedProduct.IsAssembled)
            {
                var questionTemplateIds = SelectedProduct.QuestionTemplates.Select(qt => qt.QuestionTemplateId).ToList();
                ListQuestionTemplateSingleProduct = QuestionTemplatesAppService.GetListByGuidsAsync(questionTemplateIds);
                foreach (var questionTemplate in ListQuestionTemplateSingleProduct)
                {
                    QuestionTemplateValues.Add(new QuestionTemplateModel(questionTemplate.Id, "", SelectedProduct.Id));
                }
                // Prendo tutte le domande associate ai sotto-prodotti e le mappo in DTO
                
            }else{
                RFQProductAndQuestions = RequestForQuotationsAppService.GetRfqProductAndQuestionsAsync(SelectedProduct.Id).Result.ToList();
                foreach (var rfqProductAndQuestion in RFQProductAndQuestions)
                {
                    var rfqProductId = rfqProductAndQuestion.Product.Id;
                    foreach (var questionTemplate in rfqProductAndQuestion.QuestionTemplates)
                    {
                        QuestionTemplateValues.Add(new QuestionTemplateModel(questionTemplate.Id, "", rfqProductId));
                    }
                }
            }
        }
        StateHasChanged();
    }

    private void OnSelectedItemChanged(RequestForQuotationItemDto selectedItem)
    {
        QuestionTemplateValues.Clear();
        selectedListViewItem = selectedItem;
        SelectedCatalogItemQuantity = selectedItem.Quantity;
        SelectedProduct = Catalogs.Items
            .SelectMany(catalog => catalog.Products)
            .FirstOrDefault(item => item.Id == selectedItem.ProductId);
        SelectedCatalog = Catalogs.Items.FirstOrDefault(catalog => catalog.Products.Contains(SelectedProduct));
        catalogNameSelected = SelectedCatalog.Catalog.Name;
        productNameSelected = SelectedProduct.Name;
        isSelecting = true;

        foreach (var answer in selectedListViewItem.Answers)
        {
            QuestionTemplateValues.Add(new QuestionTemplateModel(answer.QuestionId, answer.AnswerValue, selectedListViewItem.ProductId));
        }
        StateHasChanged();
    }

    private void SaveAnswerForRFQ()
    {
        
        ListRequestForQuotationItems.RemoveAll(x => x.ProductId.Equals(SelectedProduct.Id));
        SelectedCatalogItemQuantity = Math.Max(SelectedCatalogItemQuantity, 1);
        
        ListRequestForQuotationItems.Add(new RequestForQuotationItemDto
        {
            ProductId = SelectedProduct.Id,
            Answers = QuestionTemplateValues.Select(x => new Answer
            {
                QuestionId = x.QuestionId,
                AnswerValue = x.AnswerValue
            }).ToList(),
            Quantity = SelectedCatalogItemQuantity
        });
        
        UpdateValueCatalog(null);
        CatalogAutocompleteRef.Clear();
        isSelecting = false;
        StateHasChanged();
    }

    private void DeleteSelectedItem()
    {
        if (selectedListViewItem != null)
        {
            // Rimuovi l'elemento selezionato dalla lista
            ListRequestForQuotationItems.Remove(selectedListViewItem);
            // Effettua un clear del valore selezionato
            selectedListViewItem = null;
        }

        if (ListRequestForQuotationItems.Count == 0)
        {
            isSelecting = false;
        }

        UpdateValueCatalog(null);
        StateHasChanged();
    }

    private void AddNewItem()
    {
        UpdateValueCatalog(null);
        isSelecting = false;
        StateHasChanged();
    }
    
    private void OnClickSelectSingleItem(RequestForQuotationItemDto item)
    {
        OnSelectedItemChanged(item);
    }

    private async Task HandleValidSubmit()
    {
        try
        {
            // Creazione di una nuova richiesta di preventivo
            NewRequestForQuotationDto.RequestForQuotationItems = ListRequestForQuotationItems;
            NewRequestForQuotationDto.ConcurrencyStamp = Guid.NewGuid().ToString();
            NewRequestForQuotation =
                _mapper.Map<RequestForQuotationDto, RequestForQuotationCreateDto>(NewRequestForQuotationDto);
            Console.WriteLine(NewRequestForQuotationDto);
            var result = await RequestForQuotationsAppService.CreateAsync(NewRequestForQuotation);
            NavigationManager.NavigateTo("/request-for-quotations");

            // Esegui le azioni necessarie dopo il salvataggio
            // Ad esempio, puoi visualizzare un messaggio di conferma
            Console.WriteLine("Richiesta di preventivo salvata con successo!");
        }
        catch (Exception ex)
        {
            // Gestisci eventuali errori durante il salvataggio
            // Ad esempio, puoi visualizzare un messaggio di errore
            Console.WriteLine($"Si è verificato un errore durante il salvataggio: {ex.Message}");
        }
    }

    private async Task HandleValidDraft()
    {
        try
        {
            NewRequestForQuotationDto.ConcurrencyStamp = Guid.NewGuid().ToString();
            NewRequestForQuotation =
                _mapper.Map<RequestForQuotationDto, RequestForQuotationCreateDto>(NewRequestForQuotationDto);
            Console.WriteLine(NewRequestForQuotationDto);
            var result = await RequestForQuotationsAppService.CreateAsync(NewRequestForQuotation);
            StateHasChanged();
            NavigationManager.NavigateTo("/request-for-quotations");
        }
        catch (Exception ex)
        {
            throw new UserFriendlyException(ex.Message);
        }
    }

    private void HandleCancel()
    {
        // Reindirizzamento alla pagina /request-for-quotations
        NavigationManager.NavigateTo("/request-for-quotations");
    }
    
    private Task<IEnumerable<OrganizationDto>> SearchOrganization(string value)
    {
        if (Organizations == null || Organizations.TotalCount == 0)
            return new Task<IEnumerable<OrganizationDto>>(() => new List<OrganizationDto>());

        // Se il testo è null o vuoto, mostra l'elenco completo
        return string.IsNullOrEmpty(value)
            ? Task.FromResult<IEnumerable<OrganizationDto>>(Organizations.Items)
            : Task.FromResult(Organizations.Items.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase)));
    }

    private Task<IEnumerable<ContactDto>> SearchContact(string value)
    {
        if (Contacts == null || Contacts.Items.Count == 0)
            return new Task<IEnumerable<ContactDto>>(() => new List<ContactDto>());

        // Se il testo è null o vuoto, mostra l'elenco completo
        return string.IsNullOrEmpty(value)
            ? Task.FromResult<IEnumerable<ContactDto>>(Contacts.Items)
            : Task.FromResult(Contacts.Items.Where(x => x.ToStringNameSurname().Contains(value, StringComparison.InvariantCultureIgnoreCase)));
    }

    private Task<IEnumerable<LookupDto<Guid>>> SearchAgent(string value)
    {
        if (userLookUpDtos == null || userLookUpDtos.Count == 0)
            return Task.FromResult<IEnumerable<LookupDto<Guid>>>(new List<LookupDto<Guid>>());

        // Se il testo è null o vuoto, mostra l'elenco completo
        return string.IsNullOrEmpty(value)
            ? Task.FromResult<IEnumerable<LookupDto<Guid>>>(userLookUpDtos)
            : Task.FromResult(userLookUpDtos.Where(x => x.DisplayName.Contains(value, StringComparison.InvariantCultureIgnoreCase)));
    }

    private Task<IEnumerable<CatalogDto>> SearchCatalog(string value)
    {
        if (Catalogs.Items == null || Catalogs.Items.Count == 0)
            return new Task<IEnumerable<CatalogDto>>(() => new List<CatalogDto>());

        // Se il testo è null o vuoto, mostra l'elenco completo
        return string.IsNullOrEmpty(value)
            ? Task.FromResult(Catalogs.Items.Select(c => c.Catalog))
            : Task.FromResult(Catalogs.Items
                .Where(x => x.Catalog.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase))
                .Select(c => c.Catalog));
    }
    
    private Task<IEnumerable<ProductDto>> SearchProduct(string value)
    {
        if (SelectedCatalog.Products == null || SelectedCatalog.Products.Count == 0)
            return new Task<IEnumerable<ProductDto>>(() => new List<ProductDto>());
        // Se il testo è null o vuoto, mostra l'elenco completo
        return string.IsNullOrEmpty(value)
            ? Task.FromResult<IEnumerable<ProductDto>>(SelectedCatalogProducts)
            : Task.FromResult(SelectedCatalogProducts.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase)));
    }
}