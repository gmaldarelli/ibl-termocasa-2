using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace IBLTermocasa;

[Dependency(ReplaceServices = true)]
public class IBLTermocasaBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "IBLTermocasa";
}
