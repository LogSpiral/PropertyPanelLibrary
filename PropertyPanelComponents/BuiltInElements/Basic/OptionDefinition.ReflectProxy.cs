using PropertyPanelLibrary.BasicElements;
using SilkyUIFramework.Extensions;
using System;
using System.Collections;
using System.Reflection;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Basic;

public partial class OptionDefinition
{
    private class ReflectProxyModeManager
    {
        public ReflectProxyModeManager(Type entityDefinitionType)
        {
            EntityDefinitionElementType = entityDefinitionType;
            Initialize();
        }

        // 原版DefinitionElement的Type，用来获取它的函数进而套壳
        public Type EntityDefinitionElementType;

        // 相应的配置UI实例
        public ConfigElement ElementInstance;

        /// <summary>
        /// 当前选中的元素
        /// </summary>
        public UIElement OptionChoice;

        /// <summary>
        /// 代理选中的选项元素
        /// 纯粹的绘制代理
        /// </summary>
        public SUIProxyElement _proxyOptionChoice;

        /// <summary>
        /// 用于创建单个选项元素
        /// </summary>
        public MethodInfo createDefinitionMethod;

        /// <summary>
        /// 用于调整单个选项元素
        /// </summary>
        public MethodInfo tweakDefinitionMethod;

        /// <summary>
        /// 用于生成所有的选项元素
        /// </summary>
        public MethodInfo getDefinitionListMethod;

        /// <summary>
        /// 用于筛选通过的选项元素
        /// </summary>
        public MethodInfo getPassedDefinitionListMethod;

        /// <summary>
        /// 用于获取选项列表
        /// </summary>
        public PropertyInfo optionProperty;

        /// <summary>
        /// 用于获取模组名筛选器
        /// </summary>
        public PropertyInfo filterModProperty;

        /// <summary>
        /// 用于获取选项名筛选器
        /// </summary>
        public PropertyInfo filterNameProperty;

        /// <summary>
        /// 设置选项的描述
        /// </summary>
        public PropertyInfo tooltipProperty;

        /// <summary>
        /// 用于设置上新的目标选项
        /// </summary>
        public MethodInfo setItemMethod;

        /// <summary>
        /// 选项元素
        /// </summary>
        public IList Options
        {
            get => optionProperty.GetValue(ElementInstance) as IList;
            set => optionProperty.SetValue(ElementInstance, value);
        }

        /// <summary>
        /// 模组名筛选器
        /// </summary>
        public UIFocusInputTextField FilterMod => filterModProperty.GetValue(ElementInstance) as UIFocusInputTextField;

        /// <summary>
        /// 选项名筛选器
        /// </summary>
        public UIFocusInputTextField FilterName => filterNameProperty.GetValue(ElementInstance) as UIFocusInputTextField;

        private void Initialize()
        {
            #region 获取函数
            var methods = EntityDefinitionElementType.GetMethods(BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Instance);
            byte bitsByte = 0;
            foreach (var method in methods)
            {
                switch (method.Name)
                {
                    case "CreateDefinitionOptionElement":
                        createDefinitionMethod = method;
                        bitsByte += 1;
                        break;

                    case "GetPassedOptionElements":
                        getPassedDefinitionListMethod = method;
                        bitsByte += 2;
                        break;

                    case "CreateDefinitionOptionElementList":
                        getDefinitionListMethod = method;
                        bitsByte += 4;
                        break;

                    case "TweakDefinitionOptionElement":
                        tweakDefinitionMethod = method;
                        bitsByte += 8;
                        break;
                }
                if (bitsByte == 15)
                    break;
            }

            #endregion 获取函数

            #region 获取属性

            optionProperty = EntityDefinitionElementType.GetProperty("Options", BindingFlags.Instance | BindingFlags.NonPublic);
            filterNameProperty = EntityDefinitionElementType.GetProperty("ChooserFilter", BindingFlags.Instance | BindingFlags.NonPublic);
            filterModProperty = EntityDefinitionElementType.GetProperty("ChooserFilterMod", BindingFlags.Instance | BindingFlags.NonPublic);

            #endregion 获取属性
        }


        public void AppendOptionChoice(OptionDefinition option)
        {
            ElementInstance = Activator.CreateInstance(EntityDefinitionElementType) as ConfigElement;
            IList list = null;
            int index = -1;
            if (option.MetaData is ListValueHandler listHandler)
            {
                list = listHandler.List;
                index = listHandler.Index;
            }
            ElementInstance.Bind(option.MetaData.VariableInfo, option.MetaData.Item, list, index);
            ElementInstance.OnBind();

            OptionChoice = createDefinitionMethod.Invoke(ElementInstance, []) as UIElement;
            tweakDefinitionMethod.Invoke(ElementInstance, [OptionChoice]);
            var proxyOptionChoice = _proxyOptionChoice = new SUIProxyElement(OptionChoice);
            proxyOptionChoice.LeftMouseClick += delegate
            {
                option._selectionExpanded = !option._selectionExpanded;
                option._pendingUpdateRequired = true;
            };
            proxyOptionChoice.SetTop(2);
            proxyOptionChoice.SetLeft(-4, 0, 0);
            proxyOptionChoice.SetPadding(0);
            proxyOptionChoice.Join(option);
            // 添加代理的选中元素
            setItemMethod = OptionChoice.GetType().GetMethod("SetItem", BindingFlags.Instance | BindingFlags.Public);
            tooltipProperty = OptionChoice.GetType().GetProperty("Tooltip", BindingFlags.Instance | BindingFlags.Public);
        }


        public void SetupList(OptionDefinition option, out bool isEmpty)
        {
            isEmpty = true;
            Options ??= getDefinitionListMethod.Invoke(ElementInstance, []) as IList;

            IList passed = getPassedDefinitionListMethod.Invoke(ElementInstance, []) as IList;
            foreach (var p in passed)
            {
                if (p is not UIElement element) return;
                SUIProxyElement proxy = new(element);
                proxy.LeftMouseClick += delegate
                {
                    var prop = p.GetType().GetProperty("Definition");
                    option.SetValue(prop.GetValue(p));
                    option._pendingUpdateRequired = true;
                    option._selectionExpanded = false;
                    setItemMethod?.Invoke(OptionChoice, [option.GetValue()]);
                    option._filteringName.InnerText.Text = "";
                };
                proxy.Join(option._optionView.Container);
                isEmpty = false;
            }
        }

        public void SetNameFilter(string content) => FilterName?.SetText(content);
        public void SetModFilter(string content) => FilterMod?.SetText(content);
        public void HoverText(OptionDefinition option)
        {
            if (_proxyOptionChoice.IsMouseHovering)
                UICommon.TooltipMouseText(tooltipProperty.GetValue(OptionChoice).ToString());
            else 
            {
                foreach (var elem in option._optionView.Container.Children)
                {
                    if (elem.IsMouseHovering && elem is SUIProxyElement proxy)
                    {
                        UICommon.TooltipMouseText(tooltipProperty.GetValue(proxy.InnerElement).ToString());
                        break;
                    }
                }
            }

        }
    }
}
