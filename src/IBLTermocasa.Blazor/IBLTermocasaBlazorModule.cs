using System;
using System.Collections.Generic;
using System.Net.Http;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using IBLTermocasa.Blazor.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IBLTermocasa.Blazor.Components.Layout;
using IBLTermocasa.Blazor.Navigation;
using OpenIddict.Abstractions;
using Volo.Abp.Account.Pro.Admin.Blazor.WebAssembly;
using Volo.Abp.AspNetCore.Components.Web.LeptonTheme.Components;
using Volo.Abp.AspNetCore.Components.Web.Theming;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.AspNetCore.Components.WebAssembly.LeptonTheme;
using Volo.Abp.AuditLogging.Blazor.WebAssembly;
using Volo.Abp.Autofac.WebAssembly;
using Volo.Abp.AutoMapper;
using Volo.Abp.Gdpr.Blazor.Extensions;
using Volo.Abp.Gdpr.Blazor.WebAssembly;
using Volo.Abp.Identity.Pro.Blazor.Server.WebAssembly;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;
using Volo.Abp.LanguageManagement.Blazor.WebAssembly;
using Volo.Abp.OpenIddict.Pro.Blazor.WebAssembly;
using Volo.Abp.SettingManagement.Blazor.WebAssembly;
using Volo.Abp.TextTemplateManagement.Blazor.WebAssembly;
using Volo.Saas.Host.Blazor.WebAssembly;
using Volo.CmsKit.Pro.Admin.Blazor.WebAssembly;

using Volo.Abp.LeptonTheme.Management.Blazor.WebAssembly;
using Volo.Chat.Blazor;
using Volo.Chat.Blazor.WebAssembly;

namespace IBLTermocasa.Blazor;

[DependsOn(
    typeof(AbpAccountAdminBlazorWebAssemblyModule),
    typeof(AbpAspNetCoreComponentsWebAssemblyLeptonThemeModule),
    typeof(AbpAuditLoggingBlazorWebAssemblyModule),
    typeof(AbpAutofacWebAssemblyModule),
    typeof(AbpGdprBlazorWebAssemblyModule),
    typeof(AbpIdentityProBlazorWebAssemblyModule),
    typeof(AbpOpenIddictProBlazorWebAssemblyModule),
    typeof(AbpSettingManagementBlazorWebAssemblyModule),
    typeof(CmsKitProAdminBlazorWebAssemblyModule),
    typeof(LanguageManagementBlazorWebAssemblyModule),
    typeof(IBLTermocasaHttpApiClientModule),
    typeof(SaasHostBlazorWebAssemblyModule),
    typeof(TextTemplateManagementBlazorWebAssemblyModule),
	typeof(LeptonThemeManagementBlazorWebAssemblyModule)
)]
[DependsOn(typeof(ChatBlazorModule))]
    [DependsOn(typeof(ChatBlazorWebAssemblyModule))]
    public class IBLTermocasaBlazorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var environment = context.Services.GetSingletonInstance<IWebAssemblyHostEnvironment>();
        var builder = context.Services.GetSingletonInstance<WebAssemblyHostBuilder>();

        ConfigureAuthentication(builder);
        ConfigureHttpClient(context, environment);
        ConfigureBlazorise(context);
        ConfigureRouter(context);
        ConfigureUI(builder);
        ConfigureMenu(context);
        ConfigureAutoMapper(context);
        ConfigureCookieConsent(context);
        ConfigureExtraBlazoriseService(context);
    }
    
    private void ConfigureCookieConsent(ServiceConfigurationContext context)
    {
        context.Services.AddAbpCookieConsent(options =>
        {
            options.IsEnabled = true;
            options.CookiePolicyUrl = "/CookiePolicy";
            options.PrivacyPolicyUrl = "/PrivacyPolicy";
        });
    }


    private void ConfigureRouter(ServiceConfigurationContext context)
    {
        Configure<AbpRouterOptions>(options =>
        {
            options.AppAssembly = typeof(IBLTermocasaBlazorModule).Assembly;
        });
    }

    private void ConfigureMenu(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new IBLTermocasaMenuContributor(context.Services.GetConfiguration()));
        });
    }

    private void ConfigureBlazorise(ServiceConfigurationContext context)
    {
        context.Services
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons()
            
            ;
    }


    private static void ConfigureAuthentication(WebAssemblyHostBuilder builder)
    {
        builder.Services.AddOidcAuthentication(options =>
        {
            builder.Configuration.Bind("AuthServer", options.ProviderOptions);
            options.UserOptions.NameClaim = OpenIddictConstants.Claims.Name;
            options.UserOptions.RoleClaim = OpenIddictConstants.Claims.Role;

            options.ProviderOptions.DefaultScopes.Add("IBLTermocasa");
            options.ProviderOptions.DefaultScopes.Add("roles");
            options.ProviderOptions.DefaultScopes.Add("email");
            options.ProviderOptions.DefaultScopes.Add("phone");
        });
    }

    private static void ConfigureUI(WebAssemblyHostBuilder builder)
    {
        builder.RootComponents.Add<App>("#ApplicationContainer");
        builder.RootComponents.Add<HeadOutlet>("head::after");
    }

    private static void ConfigureHttpClient(ServiceConfigurationContext context, IWebAssemblyHostEnvironment environment)
    {
        context.Services.AddTransient(sp => new HttpClient
        {
            BaseAddress = new Uri(environment.BaseAddress)
        });
    }

    private void ConfigureAutoMapper(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<IBLTermocasaBlazorModule>();
        });
    }

    private void ConfigureExtraBlazoriseService(ServiceConfigurationContext context)
    {
        context.Services.AddScoped<SecureConfirmationService>();
        Configure<AbpDynamicLayoutComponentOptions>(options =>
        {
            options.Components.Add(typeof(BlazoriseServiceProviders), new Dictionary<string, object>());
        });
    }

}
