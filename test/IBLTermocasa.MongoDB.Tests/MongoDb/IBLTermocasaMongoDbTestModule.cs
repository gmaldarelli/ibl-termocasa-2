using System;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;

namespace IBLTermocasa.MongoDB;

[DependsOn(
    typeof(IBLTermocasaApplicationTestModule),
    typeof(IBLTermocasaMongoDbModule)
)]
public class IBLTermocasaMongoDbTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = IBLTermocasaMongoDbFixture.GetRandomConnectionString();
        });
    }
}
