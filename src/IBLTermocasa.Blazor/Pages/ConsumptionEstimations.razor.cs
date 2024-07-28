using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Web;
using Blazorise;
using IBLTermocasa.Blazor.Components.ConsumptionEstimations;
using IBLTermocasa.Common;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;
using IBLTermocasa.ConsumptionEstimations;
using IBLTermocasa.Permissions;
using IBLTermocasa.Products;
using IBLTermocasa.ProfessionalProfiles;
using IBLTermocasa.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;


namespace IBLTermocasa.Blazor.Pages
{
    public partial class ConsumptionEstimations
    {
        protected List<Volo.Abp.BlazoriseUI.BreadcrumbItem> BreadcrumbItems = new List<Volo.Abp.BlazoriseUI.BreadcrumbItem>();
        protected PageToolbar Toolbar {get;} = new PageToolbar();
        private IReadOnlyList<ConsumptionEstimationDto> ConsumptionEstimationList { get; set; }
        private int PageSize { get; } = LimitedResultRequestDto.DefaultMaxResultCount;
        private int CurrentPage { get; set; } = 1;
        private string CurrentSorting { get; set; } = string.Empty;
        private int TotalCount { get; set; }
        private bool CanCreateConsumptionEstimation { get; set; }
        private bool CanEditConsumptionEstimation { get; set; }
        private bool CanDeleteConsumptionEstimation { get; set; }
        private ConsumptionEstimationCreateDto NewConsumptionEstimation { get; set; }
        private ConsumptionEstimationUpdateDto EditingConsumptionEstimation { get; set; }
        private GetConsumptionEstimationsInput Filter { get; set; }
        private ConsumptionEstimationDto? SelectedConsumptionEstimation;
        Dictionary<PlaceHolderType, string> icons = new();
        private MudList MudListProductsList { get; set; }
        private MudListItem _selectedMudListItem;
        private Dictionary<Guid, MudListItem> _mudListItemReferences = new();
        private List<ProductDto> SelectedProducts { get; set; } = new();
        private ProductDto? SelectedProduct { get; set; } 
        private ConsumptionWorkDto SelectedConsumptionWorkDto { get; set; } = new();
        [Inject] private IJSRuntime JsRuntime { get; set; }
        
        [Inject] public IDialogService DialogService { get; set; }
        
        [Inject] private IProductsAppService ProductsAppService { get; set; }
        [Inject] private IProfessionalProfilesAppService ProfessionalProfilesAppService { get; set; }
        private List<LookupDto<Guid>> ProductsList { get; set; } = new();
        private List<LookupDto<Guid>> ProfessionalProfilesList { get; set; } = new();
        List<LookupDto<Guid>> SelectedProductListLookUp = new List<LookupDto<Guid>>();
        public MudTreeView<PlaceHolderTreeItemData> ProductMudTreeView { get; set; }
        public HashSet<PlaceHolderTreeItemData> TreeItems { get; set; } = new ();
        private Dictionary<Guid, MudListItem> _tempMudListItemReferences = new();
        private int activeTabIndex = 0;
        
        public ConsumptionEstimations()
        {
            NewConsumptionEstimation = new ConsumptionEstimationCreateDto();
            EditingConsumptionEstimation = new ConsumptionEstimationUpdateDto();
            Filter = new GetConsumptionEstimationsInput
            {
                MaxResultCount = PageSize,
                SkipCount = (CurrentPage - 1) * PageSize,
                Sorting = CurrentSorting
            };
            ConsumptionEstimationList = new List<ConsumptionEstimationDto>();
            InitComponentTreeView();
        }

        private async Task GetListProductsCollectionLookupAsync()
        {
            ProductsList = (await ProductsAppService.GetProductLookupAsync(new LookupRequestDto())).Items.ToList();
        }
        
        private async Task GetListProfessionalProfilesCollectionLookupAsync()
        {
            ProfessionalProfilesList = (await ProfessionalProfilesAppService.GetProfessionalProfileLookupAsync(new LookupRequestDto())).Items.ToList();
        }
        
        private async Task OnClickSelectProduct(LookupDto<Guid> item)
        {
            SelectedProduct = await ProductsAppService.GetAsync(item.Id);
            SelectedProducts = await ProductsAppService.GetListByIdAsync(SelectedProduct.SubProducts.Select(x => x.ProductId).ToList());

            if (_mudListItemReferences.TryGetValue(item.Id, out var listItem))
            {
                _selectedMudListItem = listItem;
            }
            _open = false;
            _openEnd = true;
            await FocusMudListItem(item.Id);
            await OnSelectedItemChanged(item.Id);
            SelectedProductListLookUp.Clear();
            SelectedProductListLookUp.Add(new LookupDto<Guid>(SelectedProduct.Id, SelectedProduct.Name));
            SelectedProduct.SubProducts.ForEach(x => SelectedProductListLookUp.Add(new LookupDto<Guid>(x.ProductId, x.Name)));
            StateHasChanged();
        }

        private async Task OnSelectedItemChanged(Guid idProduct)
        {
            SelectedConsumptionEstimation = await ConsumptionEstimationsAppService.GetAsyncByProduct(idProduct);
            var product = await ProductsAppService.GetAsync(idProduct);
            List<ProductDto> subProducts = new List<ProductDto>();
            foreach (var subProductItem in product.SubProducts)
            {
                var subProduct = await ProductsAppService.GetAsync(subProductItem.ProductId);
                subProducts.Add(subProduct);
            }
            TransformerUtils transformerUtils = new TransformerUtils();
            var treeItemData = transformerUtils.GenerateTreeItems(product, subProducts, icons);
            TreeItems = new HashSet<PlaceHolderTreeItemData>();
            TreeItems.Add(treeItemData);
        }


        private void InitComponentTreeView()
        {
            icons = new Dictionary<PlaceHolderType, string>
            {
                {PlaceHolderType.PRODUCT, MudBlazor.Icons.Material.Filled.AllInbox},
                {PlaceHolderType.PRODUCT_QUESTION_TEMPLATE, MudBlazor.Icons.Material.Filled.QuestionAnswer},
                {PlaceHolderType.PRODUCT_COMPONENT, MudBlazor.Icons.Material.Filled.Commit},
            };
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPermissionsAsync();
            await GetListProductsCollectionLookupAsync();
            await GetListProfessionalProfilesCollectionLookupAsync();
        }
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                foreach (var product in ProductsList)
                {
                    if (_tempMudListItemReferences.TryGetValue(product.Id, out var tempRef))
                    {
                        _mudListItemReferences[product.Id] = tempRef;
                    }
                }

                StateHasChanged();
            }
        }

        protected virtual ValueTask SetBreadcrumbItemsAsync()
        {
            BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:ConsumptionEstimations"]));
            return ValueTask.CompletedTask;
        }

        protected virtual ValueTask SetToolbarItemsAsync()
        {
            Toolbar.AddButton(L["ExportToExcel"], async () =>{ await DownloadAsExcelAsync(); }, IconName.Download);
            
            Toolbar.AddButton(L["NewConsumptionEstimation"], async () =>
            {
                
            }, IconName.Add, requiredPolicyName: IBLTermocasaPermissions.ConsumptionEstimations.Create);

            return ValueTask.CompletedTask;
        }

        private async Task SetPermissionsAsync()
        {
            CanCreateConsumptionEstimation = await AuthorizationService
                .IsGrantedAsync(IBLTermocasaPermissions.ConsumptionEstimations.Create);
            CanEditConsumptionEstimation = await AuthorizationService
                            .IsGrantedAsync(IBLTermocasaPermissions.ConsumptionEstimations.Edit);
            CanDeleteConsumptionEstimation = await AuthorizationService
                            .IsGrantedAsync(IBLTermocasaPermissions.ConsumptionEstimations.Delete);
                            
                            
        }

        private async Task DownloadAsExcelAsync()
        {
            var token = (await ConsumptionEstimationsAppService.GetDownloadTokenAsync()).Token;
            var remoteService = await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("IBLTermocasa") ?? await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            var culture = CultureInfo.CurrentUICulture.Name ?? CultureInfo.CurrentCulture.Name;
            if(!culture.IsNullOrEmpty())
            {
                culture = "&culture=" + culture;
            }
            await RemoteServiceConfigurationProvider.GetConfigurationOrDefaultOrNullAsync("Default");
            NavigationManager.NavigateTo($"{remoteService?.BaseUrl.EnsureEndsWith('/') ?? string.Empty}api/app/consumption-estimations/as-excel-file?DownloadToken={token}&FilterText={HttpUtility.UrlEncode(Filter.FilterText)}{culture}&ConsumptionProduct={HttpUtility.UrlEncode(Filter.IdProduct.ToString())}", forceLoad: true);
        }
        
        private Task OnBlurConsumptionComponentFormula(ConsumptionProductDto item)
        {
            SelectedConsumptionProductEstimation = item;
            return Task.CompletedTask;
        }

        private ConsumptionProductDto? SelectedConsumptionProductEstimation { get; set; }
        public MudTabs MudTabsConsumptionEstimations { get; set; }
        public MudTabPanel MudTabPanelConsumptionComponent { get; set; }
        public MudTabPanel MudTabPanelConsumptionProfessional { get; set; }
        public int MudTabsConsumptionEstimationsActiveIndex { get; set; } = 0;

        private async Task OnDoubleClickSelectConsumptionComponent(PlaceHolderTreeItemData context)
        {
            if (SelectedConsumptionProductEstimation != null)
            {
                if (SelectedConsumptionProductEstimation.ConsumptionComponentFormula == null)
                {
                    SelectedConsumptionProductEstimation.ConsumptionComponentFormula = $"{{{context.PlaceHolder}}}";
                }
                else
                {
                    SelectedConsumptionProductEstimation.ConsumptionComponentFormula += $"{{{context.PlaceHolder}}}";
                }
                // Assuming you have a way to get the ID or unique identifier of the MudTextField
                await JsRuntime.InvokeVoidAsync("focusHelper.setCursorToEnd", $"consumptionComponentField-{SelectedConsumptionProductEstimation.Id}");
            }
        }

        private async void OnClickSaveConsumptionComponent(MouseEventArgs obj)
        {
            if(SelectedConsumptionEstimation != null)
            {
                var element = ObjectMapper.Map<ConsumptionEstimationDto, ConsumptionEstimationUpdateDto>(SelectedConsumptionEstimation);
                await ConsumptionEstimationsAppService.UpdateAsync(SelectedConsumptionEstimation.Id, element);
                await UiMessageService.Success("Consumption Component added successfully");
            }
        }
        
        private void OnTabIndexChanged(int newIndex)
        {
            activeTabIndex = newIndex;
            switch (activeTabIndex)
            {
                //ConsumptionComponent
                case 0 when SelectedProduct.Id != Guid.Empty:
                    _open = false;
                    _openEnd = true;
                    break;
                case 0:
                    _open = true;
                    _openEnd = false;
                    break;
                //ConsumptionProfessionalProfile
                case 1 when SelectedProduct.Id != Guid.Empty:
                    _open = false;
                    _openEnd = true;
                    break;
                case 1:
                    _open = true;
                    _openEnd = false;
                    break;
            }
        }
        
        private async Task<IEnumerable<LookupDto<Guid>>> SearchConsumptionProfessional(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return ProfessionalProfilesList;
            }
            var lookupDtos = ProfessionalProfilesList.Where(x => x.DisplayName.Contains(value, StringComparison.InvariantCultureIgnoreCase));
            return lookupDtos;
        }
        
        private async Task OnConsumptionProfessionalSelected(LookupDto<Guid>? lookupDto)
        {
            if (lookupDto == null)
            {
                return;
            }
            
            var consumptionProfessionalSelected = await ProfessionalProfilesAppService.GetAsync(lookupDto.Id);
            //SelectedConsumptionWorkDto = new ConsumptionWorkDto(Guid.NewGuid(), consumptionProfessionalSelected.Id, consumptionProfessionalSelected.Code, consumptionProfessionalSelected.Name, consumptionProfessionalSelected.StandardPrice, "", 0);
            StateHasChanged();
        }

        private void SaveConsumptionWork(ConsumptionWorkDto consumptionWorkDto)
        {
            if(consumptionWorkDto.Id.Equals(Guid.Empty))
            {
                consumptionWorkDto.Id = Guid.NewGuid();
            }
            SelectedConsumptionWorkDto = consumptionWorkDto;
            
            
            if (SelectedConsumptionEstimation != null)
            {
                SelectedConsumptionEstimation.ConsumptionWork ??= [];

                if (SelectedConsumptionEstimation.ConsumptionWork.Any(item => item.Id == consumptionWorkDto.Id))
                {
                    var indexCe = SelectedConsumptionEstimation.ConsumptionWork.FindIndex(item =>
                        item.IdProfessionalProfile == SelectedConsumptionWorkDto.IdProfessionalProfile);
                    SelectedConsumptionEstimation.ConsumptionWork[indexCe] = SelectedConsumptionWorkDto;

                }
                else
                {
                    SelectedConsumptionEstimation.ConsumptionWork.Add(SelectedConsumptionWorkDto);
                }
            }
        }

        private async Task FocusMudListItem(Guid itemId)
        {
            await JsRuntime.InvokeVoidAsync("focusHelper.focusElement", $"mudListItem-{itemId}");
        }
        private async Task OnClickAddProfessionaProfile(MouseEventArgs obj)
        {
            await OpenProfileDialog();
        }
        private async Task OnClickEditProfessionaProfile(ConsumptionWorkDto dto)
        {
            await OpenProfileDialog(dto);
        }

        private async Task OpenProfileDialog(ConsumptionWorkDto dto = null)
        {

            if (dto == null)
            {
                dto = new ConsumptionWorkDto();
            }
            var dialog = await DialogService.ShowAsync<ConsumptionWorkInput>("Seleziona Profilo Professionale", new DialogParameters
            {
                { "ProductListLookup", SelectedProductListLookUp },
                { "ConsumptionWork", dto },
                { "ProfessionalProfilesListLookup", ProfessionalProfilesList }
            }, new DialogOptions
            {
                Position = DialogPosition.Custom,
                FullWidth = true,
                MaxWidth = MaxWidth.Medium
            });
        
            var dialogResult = await dialog.Result;
            if (!dialogResult.Canceled)
            {
                var consumptionWorkDto = dialogResult.Data as ConsumptionWorkDto;
                SaveConsumptionWork(consumptionWorkDto);
            }

            SelectedConsumptionWorkDto  = new ConsumptionWorkDto();
            OpenAddProfileCard = true;
            StateHasChanged();
        }

        public bool OpenAddProfileCard { get; set; }

        private void OnActivePanelIndexChanged(int index)
        {
            MudTabsConsumptionEstimationsActiveIndex = index;
            if(index == 1)
            {
                OpenAddProfileCard = false;
                SelectedConsumptionWorkDto = new ConsumptionWorkDto();
            }
        }

        private Task OnClickDeleteProfessionaProfile(ConsumptionWorkDto item)
        {
            if (SelectedConsumptionEstimation != null)
            {
                SelectedConsumptionEstimation.ConsumptionWork.RemoveAll(x => x.Id == item.Id);
            }
            return Task.CompletedTask;
        }

        private object DisplayProductFromId(Guid itemProductId)
        {
            return SelectedProductListLookUp.FirstOrDefault(x => x.Id == itemProductId)?.DisplayName ?? "N/A";
        }
    }
}
