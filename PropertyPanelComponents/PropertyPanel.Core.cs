using Microsoft.Xna.Framework;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.InteractableHandlers;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.MouseHandlers;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.OptionDecorators;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.Writers;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Decorators;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Filters;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Sorters;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using SilkyUIFramework;
using SilkyUIFramework.BasicElements;
using SilkyUIFramework.Extensions;
using System.Collections.Generic;

namespace PropertyPanelLibrary.PropertyPanelComponents;

public partial class PropertyPanel : UIElementGroup
{
    public SUIScrollView OptionList { get; init; }
    private readonly List<PropertyOption> _totalOptions = [];

    /// <summary>
    /// 挂起修改，重新筛选排序
    /// </summary>
    private bool _pendingUpdate;

    private bool _pendingDecorate;

    public PropertyPanel()
    {
        SetMargin(8);
        OptionList = new();
        OptionList.SetWidth(0, 1);
        OptionList.SetHeight(0, 1);
        OptionList.Join(this);
    }

    #region PanelProcessors
    public IPropertyOptionFiller Filler
    {
        set
        {
            _pendingUpdate = true;
            _totalOptions.Clear();
            value.FillingOptionList(_totalOptions);
        }
    }

    public IPropertyOptionFilter Filter
    {
        private get => field ??= NoneFilter.Instance;
        set
        {
            _pendingUpdate = true;
            field = value;
        }
    }

    public IPropertyOptionSorter Sorter
    {
        private get => field ??= NoneSorter.Instance;
        set
        {
            _pendingUpdate = true;
            field = value;
        }
    }

    public IPropertyPanelDecorator Decorator
    {
        private get => field ??= NoneDecorator.Instance;
        set
        {
            _pendingUpdate = true;
            field = value;
        }
    }
    #endregion

    #region OptionProcessors
    public IPropertyMouseHandler MouseHandler
    {
        private get => field ??= NoneMouseHandler.Instance;
        set
        {
            _pendingUpdate = true;
            field = value;
        }
    }

    public IPropertyValueWriter Writer
    {
        private get => field ??= DefaultWriter.Instance;
        set
        {
            _pendingUpdate = true;
            field = value;
        }
    }

    public IPropertyOptionDecorator OptionDecorator 
    {
        get => field ??= LabelOptionDecorator.NewLabelDecorator();
        set 
        {
            _pendingUpdate = true;
            field = value;
        }
    }

    public IPropertyOptionInteractableHandler InteractableHandler
    {
        get => field ??= NoneInteractableHandler.Instance;
        set 
        {
            _pendingUpdate = true;
            field = value;
        }
    }
    #endregion



    protected override void UpdateStatus(GameTime gameTime)
    {
        if (!_pendingUpdate) return;
        _pendingUpdate = false;
        OptionList.Container.RemoveAllChildren();
        OptionList.Remove();
        Decorator.PreFillPanel(this);

        List<PropertyOption> resultList = [];
        Filter.FliteringOptionList(_totalOptions, resultList);
        Sorter.SortingOptionList(resultList);
        foreach (var option in resultList)
            OptionList.Container.AppendChild(option);

        OptionList.Join(this);
        Decorator.PostFillPanel(this);

        base.UpdateStatus(gameTime);
    }
}