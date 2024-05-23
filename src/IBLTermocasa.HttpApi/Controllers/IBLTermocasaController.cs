using IBLTermocasa.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace IBLTermocasa.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class IBLTermocasaController : AbpControllerBase
{
    protected IBLTermocasaController()
    {
        LocalizationResource = typeof(IBLTermocasaResource);
    }
}
