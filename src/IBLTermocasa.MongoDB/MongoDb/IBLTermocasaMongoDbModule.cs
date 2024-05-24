using IBLTermocasa.Organizations;
using IBLTermocasa.Contacts;
using IBLTermocasa.Industries;
using IBLTermocasa.Subproducts;
using IBLTermocasa.Products;
using IBLTermocasa.ComponentItems;
using IBLTermocasa.Components;
using IBLTermocasa.Materials;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AuditLogging.MongoDB;
using Volo.Abp.BackgroundJobs.MongoDB;
using Volo.Abp.FeatureManagement.MongoDB;
using Volo.Abp.Identity.MongoDB;
using Volo.Abp.LanguageManagement.MongoDB;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.MongoDB;
using Volo.Abp.SettingManagement.MongoDB;
using Volo.Abp.TextTemplateManagement.MongoDB;
using Volo.Saas.MongoDB;
using Volo.Abp.BlobStoring.Database.MongoDB;
using Volo.Abp.Uow;
using Volo.Abp.Gdpr;
using Volo.Abp.OpenIddict.MongoDB;
using Volo.CmsKit.MongoDB;

namespace IBLTermocasa.MongoDB;

[DependsOn(
    typeof(IBLTermocasaDomainModule),
    typeof(AbpPermissionManagementMongoDbModule),
    typeof(AbpSettingManagementMongoDbModule),
    typeof(AbpIdentityProMongoDbModule),
    typeof(AbpOpenIddictProMongoDbModule),
    typeof(AbpBackgroundJobsMongoDbModule),
    typeof(AbpAuditLoggingMongoDbModule),
    typeof(AbpFeatureManagementMongoDbModule),
    typeof(LanguageManagementMongoDbModule),
    typeof(SaasMongoDbModule),
    typeof(TextTemplateManagementMongoDbModule),
    typeof(AbpGdprMongoDbModule),
    typeof(CmsKitProMongoDbModule),
    typeof(BlobStoringDatabaseMongoDbModule)
)]
public class IBLTermocasaMongoDbModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMongoDbContext<IBLTermocasaMongoDbContext>(options =>
        {
            options.AddDefaultRepositories();
            options.AddRepository<Material, Materials.MongoMaterialRepository>();

            options.AddRepository<Component, Components.MongoComponentRepository>();

            options.AddRepository<ComponentItem, ComponentItems.MongoComponentItemRepository>();

            options.AddRepository<Product, Products.MongoProductRepository>();

            options.AddRepository<Subproduct, Subproducts.MongoSubproductRepository>();

            options.AddRepository<Industry, Industries.MongoIndustryRepository>();

            options.AddRepository<Contact, Contacts.MongoContactRepository>();

            options.AddRepository<Organization, Organizations.MongoOrganizationRepository>();

        });

        Configure<AbpUnitOfWorkDefaultOptions>(options =>
        {
            options.TransactionBehavior = UnitOfWorkTransactionBehavior.Disabled;
        });
    }
}