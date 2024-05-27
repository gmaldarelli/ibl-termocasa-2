using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using IBLTermocasa.Localization;
using IBLTermocasa.Permissions;
using Volo.Abp.Account.Localization;
using Volo.Abp.AuditLogging.Blazor.Menus;
using Volo.Abp.Identity.Pro.Blazor.Navigation;
using Volo.Abp.LanguageManagement.Blazor.Menus;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.TextTemplateManagement.Blazor.Menus;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.OpenIddict.Pro.Blazor.Menus;
using Volo.Saas.Host.Blazor.Navigation;

namespace IBLTermocasa.Blazor.Navigation;

public class IBLTermocasaMenuContributor : IMenuContributor
{
    private readonly IConfiguration _configuration;

    public IBLTermocasaMenuContributor(IConfiguration configuration)
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

    private static Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<IBLTermocasaResource>();

        context.Menu.AddItem(new ApplicationMenuItem(
            IBLTermocasaMenus.Home,
            l["Menu:Home"],
            "/",
            icon: "fas fa-home",
            order: 1
        ));

        //HostDashboard
        context.Menu.AddItem(
            new ApplicationMenuItem(
                IBLTermocasaMenus.HostDashboard,
                l["Menu:Dashboard"],
                "/HostDashboard",
                icon: "fa fa-chart-line",
                order: 2
            ).RequirePermissions(IBLTermocasaPermissions.Dashboard.Host)
        );

        //TenantDashboard
        context.Menu.AddItem(
            new ApplicationMenuItem(
                IBLTermocasaMenus.TenantDashboard,
                l["Menu:Dashboard"],
                "/Dashboard",
                icon: "fa fa-chart-line",
                order: 2
            ).RequirePermissions(IBLTermocasaPermissions.Dashboard.Tenant)
        );

        /* Example nested menu definition:

        context.Menu.AddItem(
            new ApplicationMenuItem("Menu0", "Menu Level 0")
            .AddItem(new ApplicationMenuItem("Menu0.1", "Menu Level 0.1", url: "/test01"))
            .AddItem(
                new ApplicationMenuItem("Menu0.2", "Menu Level 0.2")
                    .AddItem(new ApplicationMenuItem("Menu0.2.1", "Menu Level 0.2.1", url: "/test021"))
                    .AddItem(new ApplicationMenuItem("Menu0.2.2", "Menu Level 0.2.2")
                        .AddItem(new ApplicationMenuItem("Menu0.2.2.1", "Menu Level 0.2.2.1", "/test0221"))
                        .AddItem(new ApplicationMenuItem("Menu0.2.2.2", "Menu Level 0.2.2.2", "/test0222"))
                    )
                    .AddItem(new ApplicationMenuItem("Menu0.2.3", "Menu Level 0.2.3", url: "/test023"))
                    .AddItem(new ApplicationMenuItem("Menu0.2.4", "Menu Level 0.2.4", url: "/test024")
                        .AddItem(new ApplicationMenuItem("Menu0.2.4.1", "Menu Level 0.2.4.1", "/test0241"))
                )
                .AddItem(new ApplicationMenuItem("Menu0.2.5", "Menu Level 0.2.5", url: "/test025"))
            )
            .AddItem(new ApplicationMenuItem("Menu0.2", "Menu Level 0.2", url: "/test02"))
        );

        */

        context.Menu.SetSubItemOrder(SaasHostMenus.GroupName, 3);

        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 5;

        //Administration->Identity
        administration.SetSubItemOrder(IdentityProMenus.GroupName, 1);

        //Administration->OpenId
        administration.SetSubItemOrder(OpenIddictProMenus.GroupName, 2);

        //Administration->Language Management
        administration.SetSubItemOrder(LanguageManagementMenus.GroupName, 3);

        //Administration->Text Template Management
        administration.SetSubItemOrder(TextTemplateManagementMenus.GroupName, 4);

        //Administration->Audit Logs
        administration.SetSubItemOrder(AbpAuditLoggingMenus.GroupName, 5);

        //Administration->Settings
        administration.SetSubItemOrder(SettingManagementMenus.GroupName, 6);

        context.Menu.AddItem(
            new ApplicationMenuItem(
                IBLTermocasaMenus.Materials,
                l["Menu:Materials"],
                url: "/materials",
                icon: "fa fa-file-alt",
                requiredPermissionName: IBLTermocasaPermissions.Materials.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                IBLTermocasaMenus.Components,
                l["Menu:Components"],
                url: "/components",
                icon: "fa fa-file-alt",
                requiredPermissionName: IBLTermocasaPermissions.Components.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                IBLTermocasaMenus.Products,
                l["Menu:Products"],
                url: "/products",
                icon: "fa fa-dolly",
                requiredPermissionName: IBLTermocasaPermissions.Products.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                IBLTermocasaMenus.Industries,
                l["Menu:Industries"],
                url: "/industries",
                icon: "fa fa-industry",
                requiredPermissionName: IBLTermocasaPermissions.Industries.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                IBLTermocasaMenus.Contacts,
                l["Menu:Contacts"],
                url: "/contacts",
                icon: "fa fa-id-card-alt",
                requiredPermissionName: IBLTermocasaPermissions.Contacts.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                IBLTermocasaMenus.Organizations,
                l["Menu:Organizations"],
                url: "/organizations",
                icon: "fa fa-building",
                requiredPermissionName: IBLTermocasaPermissions.Organizations.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                IBLTermocasaMenus.Interactions,
                l["Menu:Interactions"],
                url: "/interactions",
                icon: "fa fa-handshake",
                requiredPermissionName: IBLTermocasaPermissions.Interactions.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                IBLTermocasaMenus.QuestionTemplates,
                l["Menu:QuestionTemplates"],
                url: "/question-templates",
                icon: "fa fa-question-circle",
                requiredPermissionName: IBLTermocasaPermissions.QuestionTemplates.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                IBLTermocasaMenus.RequestForQuotations,
                l["Menu:RequestForQuotations"],
                url: "/request-for-quotations",
                icon: "fa fa-file-alt",
                requiredPermissionName: IBLTermocasaPermissions.RequestForQuotations.Default)
        );

        context.Menu.AddItem(
            new ApplicationMenuItem(
                IBLTermocasaMenus.Catalogs,
                l["Menu:Catalogs"],
                url: "/catalogs",
                icon: "fa fa-book",
                requiredPermissionName: IBLTermocasaPermissions.Catalogs.Default)
        );
        return Task.CompletedTask;
    }

    private async Task ConfigureUserMenuAsync(MenuConfigurationContext context)
    {
        var accountStringLocalizer = context.GetLocalizer<AccountResource>();
        var authServerUrl = _configuration["AuthServer:Authority"] ?? "";

        context.Menu.AddItem(new ApplicationMenuItem(
            "Account.Manage",
            accountStringLocalizer["MyAccount"],
            $"{authServerUrl.EnsureEndsWith('/')}Account/Manage",
            icon: "fa fa-cog",
            order: 1000,
            target: "_blank").RequireAuthenticated());

        context.Menu.AddItem(new ApplicationMenuItem(
            "Account.SecurityLogs",
            accountStringLocalizer["MySecurityLogs"],
            $"{authServerUrl.EnsureEndsWith('/')}Account/SecurityLogs",
            icon: "fa fa-user-shield",
            order: 1001,
            target: "_blank").RequireAuthenticated());

        await Task.CompletedTask;
    }
}