using Volo.Abp.Modularity;

namespace IBLTermocasa;

[DependsOn(
    typeof(IBLTermocasaDomainModule),
    typeof(IBLTermocasaTestBaseModule)
)]
public class IBLTermocasaDomainTestModule : AbpModule
{

}
