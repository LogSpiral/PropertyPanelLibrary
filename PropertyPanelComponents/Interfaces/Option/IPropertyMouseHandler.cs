﻿using PropertyPanelLibrary.PropertyPanelComponents.Core;
using SilkyUIFramework;

namespace PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Option;

/// <summary>
/// 处理器：控制鼠标交互时的行为
/// </summary>
public interface IPropertyMouseHandler
{
    void LeftMouseClick(UIMouseEvent evt, PropertyOption option);

    void RightMouseClick(UIMouseEvent evt, PropertyOption option);

    void MiddleMouseClick(UIMouseEvent evt, PropertyOption option);

    /// <summary>
    /// 用于非单例模式的选项处理器
    /// </summary>
    /// <returns></returns>
    IPropertyMouseHandler Clone();
}