using PropertyPanelLibrary.PropertyPanelComponents.Core;
using SilkyUIFramework.Elements;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using EDefinition = Terraria.ModLoader.Config.EntityDefinition;

namespace PropertyPanelLibrary.EntityDefinition;

public abstract class EntityDefinitionCommonHandler : IEntityDefinitionHandler
{
    protected SUIEntityDefinitionOption OptionChoice { get; set; }
    protected List<SUIEntityDefinitionOption> TotalOptionList { get; } = [];
    protected string FilterName { get; set; } = "";
    protected string FilterMod { get; set; } = "";
    public abstract UIView CreateChoiceView(PropertyOption.IMetaDataHandler metaData);

    public virtual EDefinition GetDefinitionFromOption(UIView view)
    {
        if (view is SUIEntityDefinitionOption option)
            return option.Definition;
        return null;
    }



    public virtual void HandleHoverText()
    {
        if (OptionChoice.IsMouseHovering)
            UICommon.TooltipMouseText(OptionChoice.Tooltip);
        else
        {
            foreach (var elem in TotalOptionList)
            {
                if (elem.IsMouseHovering)
                {
                    UICommon.TooltipMouseText(elem.Tooltip);
                    break;
                }
            }
        }
    }


    public virtual void InitializeOptionList(MouseEventHandler LeftClickHandler)
    {
        if (TotalOptionList.Count > 0) return;
        FillingOptionList(TotalOptionList);
        foreach (var option in TotalOptionList)
            option.LeftMouseClick += LeftClickHandler;
    }

    protected abstract void FillingOptionList(List<SUIEntityDefinitionOption> options);

    public virtual void SetDefinitionToOption(EDefinition definition)
    {
        OptionChoice.Definition = definition;
    }

    public virtual void SetModFilter(string modName)
    {
        FilterMod = modName;
    }

    public virtual void SetNameFilter(string definitionName)
    {
        FilterName = definitionName;
    }
    public virtual IReadOnlyList<UIView> GetPassedOptionElements()
    {
        List<SUIEntityDefinitionOption> results = [];
        foreach (var view in TotalOptionList)
        {
            if (CheckPassOption(view))
                results.Add(view);
        }
        return results;
    }
    protected virtual bool CheckPassOption(SUIEntityDefinitionOption view)
    {
        if (!view.Definition.DisplayName.Contains(FilterName, StringComparison.OrdinalIgnoreCase))
            return false;

        string modname = "Terraria";


        if (view.Definition.Mod != modname) 
        {
            if (ModLoader.TryGetMod(view.Definition.Mod, out var mod))
                modname = mod.DisplayNameClean;
        }


        if (!modname.Contains(FilterMod, StringComparison.OrdinalIgnoreCase))
            return false;
        return true;
    }
}
