using System;
using System.Threading.Tasks;
using Localization.Resources.AbpUi;
using Microsoft.Extensions.Configuration;
using IBLTermocasa.Localization;
using Volo.Abp.Account.Localization;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Authorization.Permissions;

namespace IBLTermocasa.Web.Public.Menus;

public class IBLTermocasaPublicMenuContributor : IMenuContributor
{
    private readonly IConfiguration _configuration;

    public IBLTermocasaPublicMenuContributor(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
        else if (context.Menu.Name == StandardMenus.User)
        {
            await ConfigureUserMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<IBLTermocasaResource>();

        // Home
        context.Menu.AddItem(
            new ApplicationMenuItem(
                IBLTermocasaPublicMenus.HomePage,
                l["Menu:HomePage"],
                "~/",
                icon: "fa fa-home",
                order: 1
            )
        );

        // ArticleSample
        context.Menu.AddItem(
            new ApplicationMenuItem(
                IBLTermocasaPublicMenus.ArticleSample,
                l["Menu:ArticleSample"],
                "~/article-sample",
                icon: "fa fa-file-signature",
                order: 2
                )
        );

        // Contact Us
        context.Menu.AddItem(
            new ApplicationMenuItem(
                IBLTermocasaPublicMenus.ContactUs,
                l["Menu:ContactUs"],
                "~/contact-us",
                icon: "fa fa-phone",
                order: 3
                )
        );

        return Task.CompletedTask;
    }

    private Task ConfigureUserMenuAsync(MenuConfigurationContext context)
    {
        var uiResource = context.GetLocalizer<AbpUiResource>();
        var accountResource = context.GetLocalizer<AccountResource>();
        
        var authServerUrl = _configuration["AuthServer:Authority"] ?? "~";
       
        context.Menu.AddItem(new ApplicationMenuItem("Account.Manage", accountResource["MyAccount"], $"{authServerUrl.EnsureEndsWith('/')}Account/Manage", icon: "fa fa-cog", order: 1000,  target: "_blank").RequireAuthenticated());
        context.Menu.AddItem(new ApplicationMenuItem("Account.SecurityLogs", accountResource["MySecurityLogs"], $"{authServerUrl.EnsureEndsWith('/')}Account/SecurityLogs", icon: "fa fa-user-shield", target: "_blank").RequireAuthenticated());
        context.Menu.AddItem(new ApplicationMenuItem("Account.Logout", uiResource["Logout"], url: "~/Account/Logout", icon: "fa fa-power-off", order: int.MaxValue - 1000).RequireAuthenticated());

        return Task.CompletedTask;
    }
}
