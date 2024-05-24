using IBLTermocasa.Organizations;
using IBLTermocasa.Contacts;
using IBLTermocasa.Industries;
using IBLTermocasa.Subproducts;
using IBLTermocasa.Products;
using IBLTermocasa.ComponentItems;
using IBLTermocasa.Components;
using IBLTermocasa.Materials;
using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;

namespace IBLTermocasa.MongoDB;

[ConnectionStringName("Default")]
public class IBLTermocasaMongoDbContext : AbpMongoDbContext
{
    public IMongoCollection<Organization> Organizations => Collection<Organization>();
    public IMongoCollection<Contact> Contacts => Collection<Contact>();
    public IMongoCollection<Industry> Industries => Collection<Industry>();
    public IMongoCollection<Subproduct> Subproducts => Collection<Subproduct>();
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

        modelBuilder.Entity<Subproduct>(b => { b.CollectionName = IBLTermocasaConsts.DbTablePrefix + "Subproducts"; });

        modelBuilder.Entity<Industry>(b => { b.CollectionName = IBLTermocasaConsts.DbTablePrefix + "Industries"; });

        modelBuilder.Entity<Contact>(b => { b.CollectionName = IBLTermocasaConsts.DbTablePrefix + "Contacts"; });

        modelBuilder.Entity<Organization>(b => { b.CollectionName = IBLTermocasaConsts.DbTablePrefix + "Organizations"; });
    }
}