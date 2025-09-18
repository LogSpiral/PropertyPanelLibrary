using Microsoft.Xna.Framework;
using PropertyPanelLibrary.BasicElements;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.InteractableHandlers;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.MouseHandlers;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.OptionDecorators;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Option.Writers;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Decorators;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Filters;
using PropertyPanelLibrary.PropertyPanelComponents.BuiltInProcessors.Panel.Sorters;
using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using SilkyUIFramework;
using SilkyUIFramework.Attributes;
using SilkyUIFramework.Elements;
using SilkyUIFramework.Extensions;
using System.Collections.Generic;

namespace PropertyPanelLibrary.PropertyPanelComponents;

[XmlElementMapping(nameof(PropertyPanel))]
public partial class PropertyPanel : UIElementGroup
{
    public SUIScrollViewAutoHideBar OptionList { get; init; }
    private readonly List<PropertyOption> _totalOptions = [];

    /// <summary>
    /// 挂起修改，重新筛选排序
    /// </summary>
    private bool _pendingUpdate;

    /// <summary>
    /// 挂起装饰器修改，重新进行装饰过程
    /// </summary>
    private bool _pendingDecorate;

    public PropertyPanel()
    {
        SetPadding(8);
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
            foreach (var option in _totalOptions)
            {
                option.Writer = Writer.Clone();
                option.InteractableHandler = InteractableHandler.Clone();
                option.MouseHandler = MouseHandler.Clone();
                option.Decorator = OptionDecorator.Clone();
            }
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
            field?.UnloadDecorate(this);
            _pendingDecorate = true;
            field = value;
        }
    }

    #endregion PanelProcessors

    #region OptionProcessors

    public IPropertyMouseHandler MouseHandler
    {
        private get => field ??= NoneMouseHandler.Instance;
        set
        {
            field = value ?? NoneMouseHandler.Instance;
            foreach (var options in _totalOptions)
                options.MouseHandler = value.Clone();
        }
    }

    public IPropertyValueWriter Writer
    {
        private get => field ??= DefaultWriter.Instance;
        set
        {
            _pendingUpdate = true;
            field = value ?? DefaultWriter.Instance;
            foreach (var options in _totalOptions)
                options.Writer = value.Clone();
        }
    }

    public IPropertyOptionDecorator OptionDecorator
    {
        get => field ??= LabelOptionDecorator.NewLabelDecorator();
        set
        {
            _pendingUpdate = true;
            field = value ?? LabelOptionDecorator.NewLabelDecorator();
            foreach (var options in _totalOptions)
                options.Decorator = value.Clone();
        }
    }

    public IPropertyOptionInteractableHandler InteractableHandler
    {
        get => field ??= NoneInteractableHandler.Instance;
        set
        {
            _pendingUpdate = true;
            field = value ?? NoneInteractableHandler.Instance;
            foreach (var options in _totalOptions)
                options.InteractableHandler = value.Clone();
        }
    }

    #endregion OptionProcessors

    protected override void UpdateStatus(GameTime gameTime)
    {
        bool pendingDecorateCache = _pendingDecorate;
        if (pendingDecorateCache)
        {
            _pendingDecorate = false;
            OptionList.Remove();
            Decorator.PreFillPanel(this);
        }

        if (_pendingUpdate)
        {
            _pendingUpdate = false;
            OptionList.Container.RemoveAllChildren();
            List<PropertyOption> resultList = [];
            Filter.FliteringOptionList(_totalOptions, resultList);
            Sorter.SortingOptionList(resultList);
            foreach (var option in resultList)
                OptionList.Container.Add(option);
        }

        if (pendingDecorateCache)
        {
            OptionList.Join(this);
            Decorator.PostFillPanel(this);
        }

        base.UpdateStatus(gameTime);
    }

    /// <summary>
    /// 强制重加载
    /// </summary>
    public void ForceReload()
    {
        _pendingDecorate = true;
        _pendingUpdate = true;
    }
}