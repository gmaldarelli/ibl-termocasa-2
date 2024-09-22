using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Components.Web.LeptonTheme;
using Volo.Abp.AspNetCore.Components.Web.LeptonTheme.Components.ApplicationLayout.Navigation;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;
using Volo.Abp.UI.Navigation;

namespace IBLTermocasa.Blazor;
[ExposeServices(typeof(MainMenuProvider))]
public class IBLTermocasaMainMenuProvider : MainMenuProvider
{
    private readonly IMenuManager _menuManager;


    public IBLTermocasaMainMenuProvider(IMenuManager menuManager, IObjectMapper<AbpAspNetCoreComponentsWebLeptonThemeModule> objectMapper, ILeptonSettingsProvider leptonSettings) : 
        base(menuManager, objectMapper, leptonSettings)
    {
        _menuManager = menuManager;
    }
    
    public  async Task<MenuViewModel> GetMenuAsync()
    {
        var menu = await _menuManager.GetMainMenuAsync();

        //Menu = _objectMapper.Map<ApplicationMenu, MenuViewModel>(menu);
        var result = new MenuViewModel
        {
            Menu = menu,
            Items = menu.Items.Select(CreateMenuItemViewModel).ToList()
        };
        result.SetParents();

        return result;
    }

    private MenuItemViewModel CreateMenuItemViewModel(ApplicationMenuItem applicationMenuItem)
    {
        var viewModel = new MenuItemViewModel
        {
            MenuItem = applicationMenuItem,
            Items = new List<MenuItemViewModel>()
        };

        foreach (var item in applicationMenuItem.Items)
        {
            viewModel.Items.Add(CreateMenuItemViewModel(item));
        }

        return viewModel;
    }
}