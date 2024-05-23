using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace IBLTermocasa.Web.Public;

[Dependency(ReplaceServices = true)]
public class IBLTermocasaBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "IBLTermocasa";
}
