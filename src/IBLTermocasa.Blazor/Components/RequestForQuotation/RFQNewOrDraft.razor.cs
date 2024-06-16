using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using AutoMapper;
using Blazorise;
using IBLTermocasa.Catalogs;
using IBLTermocasa.Common;
using IBLTermocasa.Contacts;
using IBLTermocasa.Organizations;
using IBLTermocasa.Permissions;
using IBLTermocasa.Products;
using IBLTermocasa.QuestionTemplates;
using IBLTermocasa.RequestForQuotations;
using IBLTermocasa.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;
using BreadcrumbItem = Volo.Abp.BlazoriseUI.BreadcrumbItem;

namespace IBLTermocasa.Blazor.Components.RequestForQuotation;

public partial class RFQNewOrDraft
{
    [Inject] private IMapper _mapper { get; set; }
    [Inject] public IIdentityUserAppService UserAppService { get; set; }
    [Inject] public IRequestForQuotationsAppService RequestForQuotationsAppService { get; set; }
    [Inject] public INotificationService notificationService { get; set; }
    [Inject] public IOrganizationsAppService OrganizationsAppService { get; set; }
    [Inject] public IQuestionTemplatesAppService QuestionTemplatesAppService { get; set; }
    [Inject] public IContactsAppService ContactsAppService { get; set; }
    [Inject] public ICurrentPrincipalAccessor CurrentPrincipalAccessor { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public ICatalogsAppService CatalogsAppService { get; set; }
    [Parameter] public RequestForQuotationDto RequestForQuotation { get; set; }
    
    
    [Parameter] public EventCallback<RequestForQuotationDto> OnRequestForQuotationSaved { get; set; }
    
    [Parameter] public EventCallback<RequestForQuotationDto> OnRequestForQuotationCancel { get; set; }
    
    protected List<BreadcrumbItem> BreadcrumbItems = new();

    private Validations RequestForQuotationValidations { get; set; } = new();
    private bool CanCreateRequestForQuotation { get; set; }
    private bool CanEditRequestForQuotation { get; set; }
    public RequestForQuotationCreateDto NewRequestForQuotation { get; set; }
    public CatalogWithNavigationPropertiesDto SelectedCatalog { get; set; } = new();
    private PagedResultDto<CatalogWithNavigationPropertiesDto> Catalogs { get; set; } = new();
    private List<RFQProductAndQuestionDto> RFQProductAndQuestions { get; set; } = new();
    
    private bool disableContact = true;
    private MudAutocomplete<CatalogDto> CatalogAutocompleteRef { get; set; }
    private MudAutocomplete<LookupDto<Guid>> ContactsAutoCompleteRef { set; get; }
    private ProductDto SelectedProduct { get; set; } = new();
    private List<ProductDto> SelectedCatalogProducts { get; set; }

    [Range(1, int.MaxValue)] private int SelectedCatalogItemQuantity { get; set; } = 1;
    private List<RequestForQuotationItemDto>? ListRequestForQuotationItems { get; set; } = new();
    private List<QuestionTemplateDto> ListQuestionTemplateSingleProduct { get; set; } = new();
    private RequestForQuotationItemDto selectedListViewItem { get; set; } = new();

    private List<QuestionTemplateModel> QuestionTemplateValues = new();
    private string selectedStep = "1";
    private Steps stepsRef;
    private bool isSelecting = false;
    private Guid RequestForQuotationSelectingId { get; set; } = Guid.Empty;
    private List<LookupDto<Guid>> OrganizationsCollection { get; set; } = new();
    private List<LookupDto<Guid>> ContactsCollection { get; set; } = new();
    private List<LookupDto<Guid>> AgentsCollection { get; set; } = new();
    private LookupDto<Guid> selectedOrganizationLookupDto = new();
    private LookupDto<Guid> selectedContactLookupDto = new();
    private LookupDto<Guid> selectedAgentLookupDto = new();

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        AgentsCollection = (await RequestForQuotationsAppService.GetIdentityUserLookupAsync(new LookupRequestDto())).Items.ToList();
        OrganizationsCollection = (await RequestForQuotationsAppService.GetOrganizationLookupCustomerAsync(new LookupRequestDto())).Items.ToList();
        ContactsCollection = (await RequestForQuotationsAppService.GetContactLookupAsync(new LookupRequestDto())).Items.ToList();
        Catalogs = await CatalogsAppService.GetListCatalogWithProducts(new GetCatalogsInput());
        if (RequestForQuotation.Id == Guid.Empty)
        { 
            RequestForQuotation = new RequestForQuotationDto();
            var mySelfId = CurrentPrincipalAccessor.Principal.FindUserId();
            selectedAgentLookupDto = AgentsCollection.FirstOrDefault(x => x.Id == mySelfId);
            RequestForQuotation.AgentProperty = new AgentPropertyDto(selectedAgentLookupDto.Id, selectedAgentLookupDto.DisplayName);
        }
        else if (RequestForQuotation.Id != Guid.Empty)
        {
            selectedAgentLookupDto = new LookupDto<Guid>(RequestForQuotation.AgentProperty.Id, RequestForQuotation.AgentProperty.Name);
            selectedOrganizationLookupDto = new LookupDto<Guid>(RequestForQuotation.OrganizationProperty.Id, RequestForQuotation.OrganizationProperty.Name);
            selectedContactLookupDto = new LookupDto<Guid>(RequestForQuotation.ContactProperty.Id, RequestForQuotation.ContactProperty.Name);
            ListRequestForQuotationItems = RequestForQuotation.RequestForQuotationItems;
        }
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
        StateHasChanged();
        return ValueTask.CompletedTask;
    }

    private async Task SetPermissionsAsync()
    {
        CanCreateRequestForQuotation =
            await AuthorizationService.IsGrantedAsync(IBLTermocasaPermissions.RequestForQuotations.Create);
        CanEditRequestForQuotation =
            await AuthorizationService.IsGrantedAsync(IBLTermocasaPermissions.RequestForQuotations.Edit);
    }

    private async void UpdateValueOrganization(LookupDto<Guid> arg)
    {
        if (arg == null)
        {
            selectedOrganizationLookupDto = new LookupDto<Guid>();
            RequestForQuotation.OrganizationProperty = new OrganizationPropertyDto();
            disableContact = true;
            RequestForQuotation.MailInfo = new MailInfoDto();
            RequestForQuotation.PhoneInfo = new PhoneInfoDto();
            ContactsAutoCompleteRef.Clear();
        }
        else
        {
            selectedOrganizationLookupDto = arg;
            disableContact = false;
            var organization = await OrganizationsAppService.GetAsync(arg.Id);
            RequestForQuotation.MailInfo = organization.MailInfo != null ? organization.MailInfo : new MailInfoDto();
            RequestForQuotation.PhoneInfo = organization.PhoneInfo != null ? organization.PhoneInfo : new PhoneInfoDto();
            RequestForQuotation.OrganizationProperty =
                new OrganizationPropertyDto(organization.Id, organization.Name);
            //Bisogna mettere in attesa il thread per 100 millisecondi per evitare che il focus venga perso, perchè sulla stessa riga
            await Task.Delay(100);
            await ContactsAutoCompleteRef.FocusAsync();
        }

        StateHasChanged();
    }

    private async void UpdateValueContact(LookupDto<Guid> arg)
    {
        if (arg == null)
        {
            selectedContactLookupDto = new LookupDto<Guid>();
            RequestForQuotation.ContactProperty = new ContactPropertyDto();
            ContactsAutoCompleteRef.Clear();
        }
        else
        {
            selectedContactLookupDto = arg;
            var contact = await ContactsAppService.GetAsync(arg.Id);
            RequestForQuotation.ContactProperty =
                new ContactPropertyDto(contact.Id, contact.ToStringNameSurname());
        }

        StateHasChanged();
    }

    private void UpdateValueAgent(LookupDto<Guid> arg)
    {
        if (arg == null)
        {
            selectedAgentLookupDto = new LookupDto<Guid>();
            RequestForQuotation.AgentProperty = new AgentPropertyDto();
        }
        else
        {
            selectedAgentLookupDto = arg;
            RequestForQuotation.AgentProperty = new AgentPropertyDto(arg.Id, arg.DisplayName);
        }

        StateHasChanged();
    }

    private void UpdateValueCatalog(CatalogDto arg)
    {
        if (arg == null)
        {
            ClearAllFor2Step();
        }
        else
        {
            SelectedProduct = new ProductDto();
            disableContact = false;
            QuestionTemplateValues.Clear();
            SelectedCatalog = Catalogs.Items.FirstOrDefault(x => x.Catalog.Id == arg.Id);
            SelectedCatalogProducts = SelectedCatalog.Products;
        }

        StateHasChanged();
    }

    private async Task UpdateValueProduct(ProductDto arg)
    {
        if (arg == null)
        {
            QuestionTemplateValues.Clear();
            RFQProductAndQuestions.Clear();
            ListQuestionTemplateSingleProduct.Clear();
            SelectedProduct = new ProductDto();
        }
        else
        {
            SelectedProduct = arg;
            RFQProductAndQuestions.Clear();
            if (!SelectedProduct.IsAssembled)
            {
                var questionTemplateIds = SelectedProduct.ProductQuestionTemplates.Select(qt => qt.QuestionTemplateId)
                    .ToList();
                ListQuestionTemplateSingleProduct =
                    await QuestionTemplatesAppService.GetListByGuidsAsync(questionTemplateIds);
                RFQProductAndQuestions.Add(new RFQProductAndQuestionDto(SelectedProduct,
                    ListQuestionTemplateSingleProduct));
                foreach (var questionTemplate in ListQuestionTemplateSingleProduct)
                {
                    QuestionTemplateValues.Add(new QuestionTemplateModel(SelectedProduct.Id, SelectedProduct.Name,
                        false, questionTemplate.Id, questionTemplate.QuestionText, questionTemplate.AnswerType, ""));
                }
            }
            else
            {
                var result = await RequestForQuotationsAppService.GetRfqProductAndQuestionsAsync(SelectedProduct.Id);
                RFQProductAndQuestions = result.OrderByDescending(p => p.Product.IsAssembled)
                    .ToList();
                foreach (var rfqProductAndQuestion in RFQProductAndQuestions)
                {
                    var rfqProductId = rfqProductAndQuestion.Product.Id;
                    var rfqProductName = rfqProductAndQuestion.Product.Name;
                    var isAssembled = rfqProductAndQuestion.Product.IsAssembled;
                    foreach (var questionTemplate in rfqProductAndQuestion.QuestionTemplates)
                    {
                        QuestionTemplateValues.Add(new QuestionTemplateModel(rfqProductId, rfqProductName, isAssembled,
                            questionTemplate.Id, questionTemplate.QuestionText, questionTemplate.AnswerType, ""));
                    }
                }
            }
        }

        StateHasChanged();
    }


    private void SaveAnswerForRFQ()
    {
        // Rimuove gli elementi esistenti per il prodotto selezionato
        if (ListRequestForQuotationItems.Count != 0)
        {
            ListRequestForQuotationItems.RemoveAll(x => x.ProductItems.Any(pi =>
                pi.ProductId.Equals(SelectedProduct.Id) && isSelecting && x.Id.Equals(RequestForQuotationSelectingId)));
        }

        SelectedCatalogItemQuantity = Math.Max(SelectedCatalogItemQuantity, 1);
        var productItems = new List<ProductItemDto>();
        if (QuestionTemplateValues.Any(model => model.IsAssembled))
        {
            // Crea una nuova lista di ProductItemDto con le risposte
            var isfirstIteration = true;
            var productItemPrincipalId = Guid.Empty;
            QuestionTemplateValues = QuestionTemplateValues.OrderByDescending(q => q.IsAssembled).ToList();
            foreach (var questionTemplateValue in QuestionTemplateValues)
            {
                if (productItems.Any(pi => pi.ProductId == questionTemplateValue.ProductId))
                {
                    productItems.First(pi => pi.ProductId == questionTemplateValue.ProductId).Answers.Add(new AnswerDto
                    {
                        QuestionId = questionTemplateValue.QuestionId,
                        QuestionText = questionTemplateValue.QuestionText,
                        AnswerType = questionTemplateValue.AnswerType,
                        AnswerValue = questionTemplateValue.AnswerValue
                    });
                }
                else
                {
                    if (isfirstIteration)
                    {
                        productItems.Add(new ProductItemDto
                        {
                            Id = Guid.NewGuid(),
                            ProductId = questionTemplateValue.ProductId,
                            ProductName = questionTemplateValue.ProductName,
                            Answers = new List<AnswerDto>
                            {
                                new()
                                {
                                    QuestionId = questionTemplateValue.QuestionId,
                                    QuestionText = questionTemplateValue.QuestionText,
                                    AnswerType = questionTemplateValue.AnswerType,
                                    AnswerValue = questionTemplateValue.AnswerValue
                                }
                            }
                        });
                        isfirstIteration = false;
                        productItemPrincipalId = productItems.First().Id;
                    }
                    else
                    {
                        productItems.Add(new ProductItemDto
                        {
                            Id = Guid.NewGuid(),
                            ProductId = questionTemplateValue.ProductId,
                            ProductName = questionTemplateValue.ProductName,
                            ParentId = productItemPrincipalId,
                            Answers = new List<AnswerDto>
                            {
                                new()
                                {
                                    QuestionId = questionTemplateValue.QuestionId,
                                    QuestionText = questionTemplateValue.QuestionText,
                                    AnswerType = questionTemplateValue.AnswerType,
                                    AnswerValue = questionTemplateValue.AnswerValue
                                }
                            }
                        });
                    }
                }
            }
        }
        else
        {
            // Crea una nuova lista di ProductItemDto con le risposte
            productItems.Add(new ProductItemDto
            {
                Id = Guid.NewGuid(),
                ProductId = SelectedProduct.Id,
                ProductName = SelectedProduct.Name,
                Answers = QuestionTemplateValues.Select(x => new AnswerDto
                {
                    QuestionId = x.QuestionId,
                    QuestionText = x.QuestionText,
                    AnswerType = x.AnswerType,
                    AnswerValue = x.AnswerValue
                }).ToList()
            });
        }

        // Aggiunge il nuovo RequestForQuotationItemDto alla lista
        ListRequestForQuotationItems.Add(new RequestForQuotationItemDto
        {
            Id = Guid.NewGuid(),
            Order = ListRequestForQuotationItems.Count + 1,
            Quantity = SelectedCatalogItemQuantity,
            ProductItems = productItems
        });
        ListRequestForQuotationItems = ListRequestForQuotationItems
            .OrderBy(x => x.ProductItems.Any(productItem => productItem.ParentId == null)).ToList();

        // Pulisce il campo di ricerca e aggiorna lo stato
        CatalogAutocompleteRef.Clear();
        ClearAllFor2Step();
        isSelecting = false;
        StateHasChanged();
    }

    private async Task OnSelectedItemChanged(RequestForQuotationItemDto selectedItem)
    {
        ClearAllFor2Step();
        QuestionTemplateValues.Clear();
        SelectedCatalogItemQuantity = selectedItem.Quantity;
        selectedListViewItem = selectedItem;

        // Recupera il prodotto selezionato
        if (selectedItem.ProductItems.Count > 1)
        {
            SelectedProduct = Catalogs.Items
                .SelectMany(catalog => catalog.Products)
                .FirstOrDefault(item =>
                    item.Id == selectedItem.ProductItems.First(productItem => productItem.ParentId == null).ProductId);
        }
        else
        {
            SelectedProduct = Catalogs.Items
                .SelectMany(catalog => catalog.Products)
                .FirstOrDefault(item => item.Id == selectedItem.ProductItems.First().ProductId);
        }

        // Recupera il catalogo selezionato
        SelectedCatalog = Catalogs.Items.FirstOrDefault(catalog => catalog.Products.Contains(SelectedProduct));

        if (!SelectedProduct.IsAssembled)
        {
            // Recupera gli ID dei modelli di domande
            var questionTemplateIds =
                SelectedProduct.ProductQuestionTemplates.Select(qt => qt.QuestionTemplateId).ToList();

            // Recupera i modelli di domande
            ListQuestionTemplateSingleProduct =
                await QuestionTemplatesAppService.GetListByGuidsAsync(questionTemplateIds);
            RFQProductAndQuestions.Add(new RFQProductAndQuestionDto(SelectedProduct,
                ListQuestionTemplateSingleProduct));

            // Popola QuestionTemplateValues con le risposte corrispondenti
            foreach (var questionTemplate in ListQuestionTemplateSingleProduct)
            {
                foreach (var productItem in selectedItem.ProductItems)
                {
                    foreach (var answer in
                             productItem.Answers.Where(answer => answer.QuestionId == questionTemplate.Id))
                    {
                        QuestionTemplateValues.Add(new QuestionTemplateModel(
                            SelectedProduct.Id,
                            SelectedProduct.Name,
                            false,
                            questionTemplate.Id,
                            questionTemplate.QuestionText,
                            questionTemplate.AnswerType,
                            answer.AnswerValue));
                    }
                }
            }
        }
        else
        {
            // Recupera il prodotto e le sue domande
            var result = await RequestForQuotationsAppService.GetRfqProductAndQuestionsAsync(SelectedProduct.Id);

            // Ordina e aggiunge alla lista di RFQProductAndQuestions
            RFQProductAndQuestions = result.OrderByDescending(p => p.Product.IsAssembled).ToList();

            // Popola QuestionTemplateValues con le risposte corrispondenti
            foreach (var rfqProductAndQuestion in RFQProductAndQuestions)
            {
                var rfqProductId = rfqProductAndQuestion.Product.Id;
                var rfqProductName = rfqProductAndQuestion.Product.Name;
                var isAssembled = rfqProductAndQuestion.Product.IsAssembled;

                foreach (var questionTemplate in rfqProductAndQuestion.QuestionTemplates)
                {
                    foreach (var productItem in selectedItem.ProductItems)
                    {
                        foreach (var answer in productItem.Answers.Where(answer =>
                                     answer.QuestionId == questionTemplate.Id && productItem.ProductId == rfqProductId))
                        {
                            QuestionTemplateValues.Add(new QuestionTemplateModel(
                                rfqProductId,
                                rfqProductName,
                                isAssembled,
                                questionTemplate.Id,
                                questionTemplate.QuestionText,
                                questionTemplate.AnswerType,
                                answer.AnswerValue));
                        }
                    }
                }
            }
        }

        isSelecting = true;
        RequestForQuotationSelectingId = selectedItem.Id;
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

        ClearAllFor2Step();
        StateHasChanged();
    }

    private void AddNewItem()
    {
        ClearAllFor2Step();
        isSelecting = false;
        StateHasChanged();
    }

    private void OnClickSelectSingleItem(RequestForQuotationItemDto item)
    {
        OnSelectedItemChanged(item);
        StateHasChanged();
    }

    private async Task HandleValidSubmit()
    {
        try
        {
            // Creazione di una nuova richiesta di preventivo
            RequestForQuotation.RequestForQuotationItems = ListRequestForQuotationItems;
            RequestForQuotation.Status = RfqStatus.NEW;
            RequestForQuotation.ConcurrencyStamp = Guid.NewGuid().ToString();
            NewRequestForQuotation =
                _mapper.Map<RequestForQuotationDto, RequestForQuotationCreateDto>(RequestForQuotation);
            Console.WriteLine(RequestForQuotation);
            var result = await RequestForQuotationsAppService.CreateAsync(NewRequestForQuotation);
            NavigationManager.NavigateTo("/request-for-quotations");
            Console.WriteLine("Richiesta di preventivo salvata con successo!");
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Si è verificato un errore durante il salvataggio: {ex.Message}");
        }
    }

    private async Task HandleValidDraft()
    {
        try
        {
            RequestForQuotation.RequestForQuotationItems = ListRequestForQuotationItems;
            RequestForQuotation.Status = RfqStatus.DRAFT;
            RequestForQuotation.ConcurrencyStamp = Guid.NewGuid().ToString();
            NewRequestForQuotation =
                _mapper.Map<RequestForQuotationDto, RequestForQuotationCreateDto>(RequestForQuotation);
            Console.WriteLine(RequestForQuotation);
            var result = await RequestForQuotationsAppService.CreateAsync(NewRequestForQuotation);
            NavigationManager.NavigateTo("/request-for-quotations");
            StateHasChanged();
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
        StateHasChanged();
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
            : Task.FromResult(SelectedCatalogProducts.Where(x =>
                x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase)));
    }

    private void ClearAllFor2Step()
    {
        SelectedCatalog = new CatalogWithNavigationPropertiesDto();
        SelectedProduct = new ProductDto();
        SelectedCatalogItemQuantity = 1;
        QuestionTemplateValues.Clear();
        RFQProductAndQuestions.Clear();
    }
}