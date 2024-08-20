using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Force.DeepCloner;
using IBLTermocasa.Catalogs;
using IBLTermocasa.Products;
using IBLTermocasa.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Volo.Abp.AspNetCore.Components.Messages;
using Volo.Abp.Http.Client;

namespace IBLTermocasa.Blazor.Components.Catalog;

public partial class CatalogInput
{
    [CascadingParameter] MudDialogInstance Dialog { get; set; }
    [Parameter] public CatalogDto Catalog { get; set; } = new();
    [Parameter] public EventCallback<CatalogDto> OnCatalogSaved { get; set; }
    [Parameter] public EventCallback<CatalogDto> OnCatalogCancel { get; set; }
    [Parameter] public bool DisplayReadOnly { get; set; }
    [Parameter] public bool IsNew { get; set; }
    [Inject] private IMapper _mapper { get; set; }
    [Inject] public ICatalogsAppService CatalogsAppService { get; set; }
    [Inject] public IDialogService DialogService { get; set; }
    [Inject] private IUiMessageService UiMessageService { get; set; }
    
    private List<LookupDto<Guid>> ProductsList { get; set; } = new();
    private bool success;
    private bool _isComponentRendered;
    private CatalogDto InternalCatalog = new();
    private LookupDto<Guid> selectedProductLookupDto = new();
    private bool _isLoading = true;

    //è importante sapere che OnParametersSetAsync viene chiamato prima di tutti, anche di OnInitializedAsync()
    
    private string GetButtonText()
    {
        return DisplayReadOnly ? "Back" : "Cancel";
    }
    protected override async Task OnParametersSetAsync()
    {
        InternalCatalog = IsNew ? new CatalogDto() : Catalog.DeepClone();
        StateHasChanged();
    }
    
    protected override async Task OnInitializedAsync()
    {
        ProductsList = (await CatalogsAppService.GetProductLookupAsync(new LookupRequestDto())).Items.OrderBy(x => x.DisplayName).ToList();
        _isLoading = false;
        StateHasChanged();
    }

    private async Task HandleValidSubmit()
    {
        try
        {
            CatalogDto result;
            if (IsNew)
            {
                result = await CatalogsAppService.CreateAsync(_mapper.Map<CatalogCreateDto>(InternalCatalog));
            }
            else
            {
                result = await CatalogsAppService.UpdateAsync(InternalCatalog.Id, _mapper.Map<CatalogUpdateDto>(InternalCatalog));
            }

            Catalog = _mapper.Map<CatalogDto>(result);
            InternalCatalog = Catalog.DeepClone();
            await OnCatalogSaved.InvokeAsync(InternalCatalog);
            Dialog.Close(DialogResult.Ok(result));
        }
        catch (AbpRemoteCallException ex)
        {
            // Log the exception and show a message to the user
            Console.WriteLine($"Errore durante la chiamata remota: {ex.Message}");
            // Optionally, you can use a MudBlazor Snackbar to show an error message to the user
            // _snackbar.Add("Errore durante il salvataggio del profilo. Per favore riprova.", Severity.Error);
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            Console.WriteLine($"Errore generico: {ex.Message}");
            // Optionally, show a message to the user
            // _snackbar.Add("Si è verificato un errore inaspettato. Per favore riprova.", Severity.Error);
        }
        finally
        {
            StateHasChanged();
        }
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
    
    private async Task<IEnumerable<LookupDto<Guid>>> SearchProduct(string value, CancellationToken token)
    {
        if (ProductsList == null || ProductsList.Count == 0)
            return new List<LookupDto<Guid>>();

        return await Task.Run(() =>
        {
            return string.IsNullOrEmpty(value)
                ? ProductsList
                : ProductsList
                    .Where(x => x.DisplayName.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }, token);
    }
    
    private void AddProduct()
    {
        if (selectedProductLookupDto.Id == Guid.Empty)
        {
            return;
        }
        
        if (InternalCatalog.Products.Any(p => p.ProductId == selectedProductLookupDto.Id))
        {
            UiMessageService.Warn(L["ItemAlreadyAdded"]);
            return;
        }
        
        InternalCatalog.Products.Add(new CatalogProductDto(InternalCatalog.Id, selectedProductLookupDto.Id));
    }
}