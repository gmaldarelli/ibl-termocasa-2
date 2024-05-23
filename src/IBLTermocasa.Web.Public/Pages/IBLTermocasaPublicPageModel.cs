using IBLTermocasa.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace IBLTermocasa.Web.Public.Pages;

/* Inherit your Page Model classes from this class.
 */
public abstract class IBLTermocasaPublicPageModel : AbpPageModel
{
    protected IBLTermocasaPublicPageModel()
    {
        LocalizationResourceType = typeof(IBLTermocasaResource);
    }
}
