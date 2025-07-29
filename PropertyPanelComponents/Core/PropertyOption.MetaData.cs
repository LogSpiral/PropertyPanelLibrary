using Microsoft.Xna.Framework;
using SilkyUIFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;

namespace PropertyPanelLibrary.PropertyPanelComponents.Core;

public partial class PropertyOption
{

    /// <summary>
    /// 将选项元素与对象绑定
    /// </summary>
    public void Bind(object item,PropertyFieldWrapper variableInfo)
    {
        MetaData = new VariableInfoHandler() { Item = item, VariableInfo = variableInfo };
        Internal_Bind();
    }

    public void Bind(IList list, int index, object item = null, PropertyFieldWrapper variableInfo = null) 
    {
        MetaData = new ListValueHandler()
        {
            List = list,
            Index = index,
            VariableHandler = new VariableInfoHandler() { Item = item, VariableInfo = variableInfo }
        };
        Internal_Bind();
    }

    private void Internal_Bind() 
    {
        OverflowHidden = true;
        SetWidth(0, 1f);
        SetHeight(40, 0);
        BorderRadius = new(12);
        SetMargin(4);
        BackgroundColor = Color.Black * .25f;
        LayoutType = LayoutType.Custom;

        CheckAttributes();

        Decorator?.PreFillOption(this);
        FillOption();
        Decorator?.PostFillOption(this);
    }

    protected virtual void FillOption()
    {
    }

    protected virtual void CheckAttributes()
    {
    }

    /// <summary>
    /// 该控件所属的父控件
    /// </summary>
    public PropertyOption owner;

    public Type VariableType => MetaData.VariableType;

    /// <summary>
    /// 获取当前控件控制目标所拥有的特性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    protected T GetAttribute<T>() where T : Attribute => MetaData.GetAttribute<T>();

    /// <summary>
    /// 获取值
    /// <br></br>
    /// <br>我们有两种方式控制某个值</br>
    /// <br>一个是通过对象-成员信息</br>
    /// <br>一个是通过列表-下标信息</br>
    /// <br>列表优先，如果列表为null则用对象</br>
    /// </summary>
    /// <returns></returns>
    protected object GetValue() => MetaData.GetValue();

}