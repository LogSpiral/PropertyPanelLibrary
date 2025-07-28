using System;
using System.Collections;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace PropertyPanelLibrary.PropertyPanelComponents.Core;

public partial class PropertyOption
{

    protected interface IMetaDataHandler
    {
        // 从元数据处理器中获取值
        object GetValue();

        // 从元数据处理器中获取目标类型
        Type VariableType { get; }

        // 从元数据中获取特性
        T GetAttribute<T>() where T : Attribute;

        string Label { get; }

#nullable enable
        PropertyFieldWrapper? VariableInfo { get; }
        object? Item { get; }
#nullable disable
    }

    /// <summary>
    /// 对象-信息式处理器
    /// </summary>
    protected class VariableInfoHandler : IMetaDataHandler
    {
        /// <summary>
        /// 控制的值所属的目标
        /// </summary>
        public object Item { get; init; }

        /// <summary>
        /// 控制的值对应的成员信息
        /// </summary>
        public PropertyFieldWrapper VariableInfo { get; init; }

        public Type VariableType => VariableInfo.Type;

        public T GetAttribute<T>() where T : Attribute => VariableInfo == null ? null : ConfigManager.GetCustomAttributeFromMemberThenMemberType<T>(VariableInfo, Item, null);

        public object GetValue() => VariableInfo.GetValue(Item);

        public string Label => ConfigManager.GetLocalizedText<LabelKeyAttribute, LabelArgsAttribute>(VariableInfo, "Label") ?? VariableInfo.Name;
    }

    /// <summary>
    /// 列表-索引式处理器
    /// </summary>
    protected class ListValueHandler : IMetaDataHandler
    {
        /// <summary>
        /// 控制的值所属的列表
        /// </summary>
        public IList List { get; init; }

        /// <summary>
        /// 控制的值对应的索引
        /// </summary>
        public int Index { get; init; }

#nullable enable
        /// <summary>
        /// 该List在所属对象中的元数据信息
        /// 如果不属于任何对象就是null
        /// </summary>
        public VariableInfoHandler? VariableHandler { get; init; }

        public PropertyFieldWrapper? VariableInfo => VariableHandler?.VariableInfo;

        public object? Item => VariableHandler?.Item;
#nullable disable
        public Type VariableType => GetValue().GetType();

        public T GetAttribute<T>() where T : Attribute => VariableHandler?.GetAttribute<T>();

        public object GetValue() => List[Index];

        public string Label => (Index + 1).ToString();
    }

    /// <summary>
    /// 元数据处理器对象
    /// </summary>
    protected IMetaDataHandler MetaData { get; set; }

}
