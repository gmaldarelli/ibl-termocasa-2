using IBLTermocasa.Localization;
using Volo.Abp.Application.Services;

namespace IBLTermocasa;

/* Inherit your application services from this class.
 */
public abstract class IBLTermocasaAppService : ApplicationService
{
    protected IBLTermocasaAppService()
    {
        LocalizationResource = typeof(IBLTermocasaResource);
    }
}
