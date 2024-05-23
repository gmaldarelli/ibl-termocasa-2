using Volo.Abp.Modularity;

namespace IBLTermocasa;

/* Inherit from this class for your domain layer tests. */
public abstract class IBLTermocasaDomainTestBase<TStartupModule> : IBLTermocasaTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
