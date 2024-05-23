using IBLTermocasa.Localization;
using Volo.Abp.AspNetCore.Components;

namespace IBLTermocasa.Blazor;

public abstract class IBLTermocasaComponentBase : AbpComponentBase
{
    protected IBLTermocasaComponentBase()
    {
        LocalizationResource = typeof(IBLTermocasaResource);
    }
}
