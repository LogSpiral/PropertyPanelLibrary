using PropertyPanelLibrary.PropertyPanelComponents.Core;
using PropertyPanelLibrary.PropertyPanelComponents.Interfaces.Panel;
using SilkyUIFramework.BasicComponents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using tModPorter;

namespace PropertyPanelLibrary.PropertyPanelComponents.BuiltInElements.Collection;

public class OptionHashSet : OptionCollection
{
    private Type setType;

    public List<ISetElementWrapper> DataWrapperList { get; set; }


    MethodInfo addMethod;
    MethodInfo clearMethod;
    MethodInfo removeMethod; // TODO 增加删去单独一个元素的装饰器
    PropertyFieldWrapper wrappermemberInfo;
    protected override void AddItem()
    {
        addMethod ??= Data.GetType().GetMethods().FirstOrDefault(m => m.Name == "Add");
        addMethod?.Invoke(Data, [CreateCollectionElementInstance(setType)]);
    }

    protected override void ClearCollection()
    {
        clearMethod ??= Data.GetType().GetMethods().FirstOrDefault(m => m.Name == "Clear");
        clearMethod?.Invoke(Data, []);
    }

    protected override void PrepareTypes()
    {
        setType = MetaData.VariableInfo.Type.GetGenericArguments()[0];
        JsonDefaultListValueAttribute = ConfigManager.GetCustomAttributeFromCollectionMemberThenElementType<JsonDefaultListValueAttribute>(MetaData.VariableInfo.MemberInfo, setType);
    }

    protected override void Register(Mod mod)
    {
        PropertyOptionSystem.RegistreOptionToTypeComplex(this, type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(HashSet<>));
    }
    protected override IPropertyOptionFiller GetInternalPanelFiller(object data)
    {
        var genericType = typeof(SetElementWrapper<>).MakeGenericType(setType);

        DataWrapperList = new List<ISetElementWrapper>();


        var valuesEnumerator = ((IEnumerable)Data).GetEnumerator();
        int i = 0;

        while (valuesEnumerator.MoveNext())
        {
            ISetElementWrapper proxy = (ISetElementWrapper)Activator.CreateInstance(genericType, [valuesEnumerator.Current, Data]);
            DataWrapperList.Add(proxy);
            wrappermemberInfo ??= ConfigManager.GetFieldsAndProperties(this).ToList().First(x => x.Name == "DataWrapperList");
            i++;
        }
        return base.GetInternalPanelFiller(DataWrapperList);
    }
}
