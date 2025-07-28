using Terraria.ModLoader.Config;

namespace PropertyPanelLibrary.PropertyPanelComponents.Core;

public partial class PropertyOption
{
    public virtual string Label => MetaData.Label;
    public virtual string Tooltip => ConfigManager.GetLocalizedText<TooltipKeyAttribute, TooltipArgsAttribute>(MetaData.VariableInfo, "Tooltip") ?? "";
}