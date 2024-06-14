/*using System;
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
    [Inject] public ICurrentPrincipalAccessor CurrentPrincipalAccessor { get; set; }
    [Inject] public NavigationManager NavigationManager { get; set; }
    [Inject] public ICatalogsAppService CatalogsAppService { get; set; }
    protected List<BreadcrumbItem> BreadcrumbItems = new();

    private Validations RequestForQuotationValidations { get; set; } = new();
    private bool CanCreateRequestForQuotation { get; set; }
    public RequestForQuotationDto NewRequestForQuotationDto = new();
    public RequestForQuotationCreateDto NewRequestForQuotation { get; set; }
    public CatalogWithNavigationPropertiesDto SelectedCatalog { get; set; } = new();
    private PagedResultDto<CatalogWithNavigationPropertiesDto> Catalogs { get; set; } = new();
    private List<RFQProductAndQuestionDto> RFQProductAndQuestions { get; set; } = new();

    private LookupDto<Guid> selectedAgentLookupDto = new();
    private bool disableContact = true;
    private int indexCatalog;

    private List<LookupDto<Guid>> userLookUpDtos = new();
    public MudAutocomplete<CatalogDto> CatalogAutocompleteRef { get; set; }
    private MudAutocomplete<LookupDto<Guid>> ContactsAutoCompleteRef { get; set; }
    private ProductDto SelectedProduct { get; set; } = new();
    private List<ProductDto> SelectedCatalogProducts { get; set; }

    [Range(1, int.MaxValue)] private int SelectedCatalogItemQuantity { get; set; } = 1;
    private List<RequestForQuotationItemDto>? ListRequestForQuotationItems { get; set; } = new();
    private List<QuestionTemplateDto> ListQuestionTemplateSingleProduct { get; set; } = new();
    private RequestForQuotationItemDto selectedListViewItem { get; set; }

    private List<QuestionTemplateModel> QuestionTemplateValues = new();
    private string selectedStep = "1";
    private Steps stepsRef;
    private bool isSelecting = false;
    private Guid RequestForQuotationSelectingId { get; set; }
    private List<LookupDto<Guid>> OrganizationsCollection { get; set; } = new();
    private List<LookupDto<Guid>> ContactsCollection { get; set; } = new();
    private List<LookupDto<Guid>> AgentsCollection { get; set; } = new();
    private LookupDto<Guid> selectedOrganization;

    protected override async Task OnInitializedAsync()
    {
        await SetPermissionsAsync();
        OrganizationsCollection =
            (await RequestForQuotationsAppService.GetOrganizationLookupCustomerAsync(new LookupRequestDto())).Items
            .ToList();
        ContactsCollection = (await RequestForQuotationsAppService.GetContactLookupAsync(new LookupRequestDto())).Items
            .ToList();
        AgentsCollection = (await RequestForQuotationsAppService.GetIdentityUserLookupAsync(new LookupRequestDto()))
            .Items.ToList();
        Catalogs = await CatalogsAppService.GetListCatalogWithProducts(new GetCatalogsInput());
        var mySelfId = CurrentPrincipalAccessor.Principal.FindUserId();
        selectedAgentLookupDto = AgentsCollection.FirstOrDefault(x => x.Id == mySelfId);
        NewRequestForQuotationDto.AgentProperty =
            new AgentPropertyDto(selectedAgentLookupDto.Id, selectedAgentLookupDto.DisplayName);
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

    private async void UpdateValueOrganization(LookupDto<Guid> arg)
    {
        if (arg == null)
        {
            NewRequestForQuotationDto.OrganizationProperty = new OrganizationPropertyDto();
            disableContact = true;
            NewRequestForQuotationDto.MailInfo = new MailInfoDto();
            NewRequestForQuotationDto.PhoneInfo = new PhoneInfoDto();
            ContactsAutoCompleteRef.Clear();
        }
        else
        {
            disableContact = false;
            var organization = await OrganizationsAppService.GetAsync(arg.Id);
            NewRequestForQuotationDto.MailInfo =
                organization.MailInfo != null ? organization.MailInfo : new MailInfoDto();
            NewRequestForQuotationDto.PhoneInfo =
                organization.PhoneInfo != null ? organization.PhoneInfo : new PhoneInfoDto();
            NewRequestForQuotationDto.OrganizationProperty =
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
            NewRequestForQuotationDto.ContactProperty = new ContactPropertyDto();
        }
        else
        {
            var contact = await ContactsAppService.GetAsync(arg.Id);
            NewRequestForQuotationDto.ContactProperty =
                new ContactPropertyDto(contact.Id, contact.ToStringNameSurname());
        }

        StateHasChanged();
    }

    private void UpdateValueAgent(LookupDto<Guid> arg)
    {
        if (arg == null)
        {
            NewRequestForQuotationDto.AgentProperty = new AgentPropertyDto();
        }
        else
        {
            NewRequestForQuotationDto.AgentProperty = new AgentPropertyDto(arg.Id, arg.DisplayName);
        }

        StateHasChanged();
    }

    private void UpdateValueCatalog(CatalogDto arg)
    {
        if (arg == null)
        {
            ClearAll();
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
        ClearAll();
        isSelecting = false;
        StateHasChanged();
    }

    private async Task OnSelectedItemChanged(RequestForQuotationItemDto selectedItem)
    {
        ClearAll();
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

        ClearAll();
        StateHasChanged();
    }

    private void AddNewItem()
    {
        ClearAll();
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
            NewRequestForQuotationDto.Status = Status.NEW;
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
            NewRequestForQuotationDto.Status = Status.DRAFT;
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
            : ContactsCollection.Where(x => x.DisplayName.Contains(value, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
    }

    private Task<IEnumerable<LookupDto<Guid>>> SearchAgent(string value)
    {
        if (userLookUpDtos == null || userLookUpDtos.Count == 0)
            return Task.FromResult<IEnumerable<LookupDto<Guid>>>(new List<LookupDto<Guid>>());

        // Se il testo è null o vuoto, mostra l'elenco completo
        return string.IsNullOrEmpty(value)
            ? Task.FromResult<IEnumerable<LookupDto<Guid>>>(userLookUpDtos)
            : Task.FromResult(userLookUpDtos.Where(x =>
                x.DisplayName.Contains(value, StringComparison.InvariantCultureIgnoreCase)));
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

    private void ClearAll()
    {
        SelectedCatalog = new CatalogWithNavigationPropertiesDto();
        SelectedProduct = new ProductDto();
        SelectedCatalogItemQuantity = 1;
        QuestionTemplateValues.Clear();
        RFQProductAndQuestions.Clear();
    }
}*/