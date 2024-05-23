using Volo.Abp.Modularity;

namespace IBLTermocasa;

[DependsOn(
    typeof(IBLTermocasaApplicationModule),
    typeof(IBLTermocasaDomainTestModule)
)]
public class IBLTermocasaApplicationTestModule : AbpModule
{

}
