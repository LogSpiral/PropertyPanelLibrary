using SilkyUIFramework.BasicElements;
using System;
using System.Collections.Generic;
using Terraria.ModLoader.Config;
using EDefinition = Terraria.ModLoader.Config.EntityDefinition;
using static PropertyPanelLibrary.PropertyPanelComponents.Core.PropertyOption;

namespace PropertyPanelLibrary.EntityDefinition;

public interface IEntityDefinitionHandler
{
    UIView CreateChoiceView(IMetaDataHandler metaData);
    void SetModFilter(string modName);
    void SetNameFilter(string definitionName);
    void InitializeOptionList(MouseEventHandler LeftClickHandler);
    IReadOnlyList<UIView> GetPassedOptionElements();
    void HandleHoverText();

    EDefinition GetDefinitionFromOption(UIView view);
    void SetDefinitionToOption(EDefinition definition);
}
