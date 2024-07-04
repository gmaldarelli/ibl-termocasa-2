using IBLTermocasa.ConsumptionEstimations;
using IBLTermocasa.ProfessionalProfiles;
using IBLTermocasa.BillOfMaterials;
using IBLTermocasa.Catalogs;
using IBLTermocasa.RequestForQuotations;
using IBLTermocasa.QuestionTemplates;
using IBLTermocasa.Interactions;
using IBLTermocasa.Organizations;
using IBLTermocasa.Contacts;
using IBLTermocasa.Industries;
using IBLTermocasa.Products;
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
using Volo.Chat.MongoDB;
using Volo.FileManagement.MongoDB;

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
[DependsOn(typeof(ChatMongoDbModule))]
[DependsOn(typeof(FileManagementMongoDbModule))]
public class IBLTermocasaMongoDbModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMongoDbContext<IBLTermocasaMongoDbContext>(options =>
        {
            options.AddDefaultRepositories();
            options.AddRepository<Material, Materials.MongoMaterialRepository>();

            options.AddRepository<Component, Components.MongoComponentRepository>();

            options.AddRepository<Product, Products.MongoProductRepository>();

            options.AddRepository<Industry, Industries.MongoIndustryRepository>();

            options.AddRepository<Contact, Contacts.MongoContactRepository>();

            options.AddRepository<Organization, Organizations.MongoOrganizationRepository>();

            options.AddRepository<Interaction, Interactions.MongoInteractionRepository>();

            options.AddRepository<QuestionTemplate, QuestionTemplates.MongoQuestionTemplateRepository>();

            options.AddRepository<RequestForQuotation, RequestForQuotations.MongoRequestForQuotationRepository>();

            options.AddRepository<Catalog, Catalogs.MongoCatalogRepository>();

            options.AddRepository<BillOfMaterial, BillOfMaterials.MongoBillOfMaterialRepository>();

            options.AddRepository<ProfessionalProfile, ProfessionalProfiles.MongoProfessionalProfileRepository>();

            options.AddRepository<ConsumptionEstimation, ConsumptionEstimations.MongoConsumptionEstimationRepository>();

        });

        Configure<AbpUnitOfWorkDefaultOptions>(options =>
        {
            options.TransactionBehavior = UnitOfWorkTransactionBehavior.Disabled;
        });
    }
}