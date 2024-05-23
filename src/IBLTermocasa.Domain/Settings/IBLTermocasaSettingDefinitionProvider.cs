using Volo.Abp.Settings;

namespace IBLTermocasa.Settings;

public class IBLTermocasaSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(IBLTermocasaSettings.MySetting1));
    }
}
