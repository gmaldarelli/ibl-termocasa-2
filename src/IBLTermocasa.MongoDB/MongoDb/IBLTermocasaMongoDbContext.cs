using IBLTermocasa.ProfessionalProfiles;
using IBLTermocasa.BillOfMaterials;
using IBLTermocasa.Catalogs;
using IBLTermocasa.RequestForQuotations;
using IBLTermocasa.QuestionTemplates;
using IBLTermocasa.Interactions;
using Volo.Abp.Identity;
using Volo.Abp.Identity;
using Volo.Abp.Identity;
using IBLTermocasa.Organizations;
using IBLTermocasa.Contacts;
using IBLTermocasa.Industries;
using IBLTermocasa.Products;
using IBLTermocasa.Components;
using IBLTermocasa.Materials;
using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace IBLTermocasa.MongoDB;

[ConnectionStringName("Default")]
public class IBLTermocasaMongoDbContext : AbpMongoDbContext
{
    public IMongoCollection<ProfessionalProfile> ProfessionalProfiles => Collection<ProfessionalProfile>();
    public IMongoCollection<BillOfMaterial> BillOfMaterials => Collection<BillOfMaterial>();
    public IMongoCollection<Catalog> Catalogs => Collection<Catalog>();
    public IMongoCollection<RequestForQuotation> RequestForQuotations => Collection<RequestForQuotation>();
    public IMongoCollection<QuestionTemplate> QuestionTemplates => Collection<QuestionTemplate>();
    public IMongoCollection<Interaction> Interactions => Collection<Interaction>();
    public IMongoCollection<Organization> Organizations => Collection<Organization>();
    public IMongoCollection<Contact> Contacts => Collection<Contact>();
    public IMongoCollection<Industry> Industries => Collection<Industry>();
    public IMongoCollection<SubProduct> Subproducts => Collection<SubProduct>();
    public IMongoCollection<Product> Products => Collection<Product>();
    public IMongoCollection<ComponentItem> ComponentItems => Collection<ComponentItem>();
    public IMongoCollection<Component> Components => Collection<Component>();
    public IMongoCollection<Material> Materials => Collection<Material>();

    /* Add mongo collections here. Example:
     * public IMongoCollection<Question> Questions => Collection<Question>();
     */

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        //builder.Entity<YourEntity>(b =>
        //{
        //    //...
        //});

        modelBuilder.Entity<Material>(b => { b.CollectionName = IBLTermocasaConsts.DbTablePrefix + "Materials"; });

        modelBuilder.Entity<Component>(b => { b.CollectionName = IBLTermocasaConsts.DbTablePrefix + "Components"; });

        modelBuilder.Entity<ComponentItem>(b => { b.CollectionName = IBLTermocasaConsts.DbTablePrefix + "ComponentItems"; });

        modelBuilder.Entity<Product>(b => { b.CollectionName = IBLTermocasaConsts.DbTablePrefix + "Products"; });

        modelBuilder.Entity<SubProduct>(b => { b.CollectionName = IBLTermocasaConsts.DbTablePrefix + "SubProducts"; });

        modelBuilder.Entity<Industry>(b => { b.CollectionName = IBLTermocasaConsts.DbTablePrefix + "Industries"; });

        modelBuilder.Entity<Contact>(b => { b.CollectionName = IBLTermocasaConsts.DbTablePrefix + "Contacts"; });

        modelBuilder.Entity<Organization>(b => { b.CollectionName = IBLTermocasaConsts.DbTablePrefix + "Organizations"; });

        modelBuilder.Entity<Interaction>(b => { b.CollectionName = IBLTermocasaConsts.DbTablePrefix + "Interactions"; });

        modelBuilder.Entity<QuestionTemplate>(b => { b.CollectionName = IBLTermocasaConsts.DbTablePrefix + "QuestionTemplates"; });

        modelBuilder.Entity<RequestForQuotation>(b => { b.CollectionName = IBLTermocasaConsts.DbTablePrefix + "RequestForQuotations"; });

        modelBuilder.Entity<Catalog>(b => { b.CollectionName = IBLTermocasaConsts.DbTablePrefix + "Catalogs"; });

        modelBuilder.Entity<BillOfMaterial>(b => { b.CollectionName = IBLTermocasaConsts.DbTablePrefix + "BillOfMaterials"; });

        modelBuilder.Entity<ProfessionalProfile>(b => { b.CollectionName = IBLTermocasaConsts.DbTablePrefix + "ProfessionalProfiles"; });
    }
}