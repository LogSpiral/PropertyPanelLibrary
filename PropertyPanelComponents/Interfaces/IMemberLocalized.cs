using PropertyPanelLibrary.PropertyPanelComponents.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace PropertyPanelLibrary.PropertyPanelComponents.Interfaces;

public interface IMemberLocalized : ILoadable
{
    private static readonly Dictionary<Type, string> _cachedRootPath = [];
    private static readonly Dictionary<Type, IReadOnlyList<string>> _cachedSuffixes = [];

    public static string GetRootPath(Type type) 
    {
        if(_cachedRootPath.TryGetValue(type,out var result))
            return result;

        var instance = Activator.CreateInstance(type) as IMemberLocalized;

        result = instance.LocalizationRootPath;

        _cachedRootPath.Add(type, result);
        
        return result;
    }

    public static IReadOnlyList<string> GetSuffixes(Type type) 
    {
        if (_cachedSuffixes.TryGetValue(type, out var result))
            return result;

        var instance = Activator.CreateInstance(type) as IMemberLocalized;

        result = instance.LocalizationSuffixes;

        _cachedSuffixes.Add(type, result);

        return result;
    }

    public static void AutoRegister(IMemberLocalized memberLocalized)
    {
        var type = memberLocalized.GetType();
        foreach (var variableInfo in ConfigManager.GetFieldsAndProperties(type))
        {
            if (!variableInfo.CanWrite) continue;
            if (Attribute.IsDefined(variableInfo.MemberInfo, typeof(PropertyPanelIgnoreAttribute))) continue;
            var root = $"{memberLocalized.LocalizationRootPath}.{variableInfo.Name}";
            foreach (var suffix in memberLocalized.LocalizationSuffixes)
                Language.GetOrRegister($"{root}.{suffix}", () => variableInfo.Name);
        }
        InitializeCachedData(memberLocalized);
    }

    public static void InitializeCachedData(IMemberLocalized memberLocalized) 
    {
        var type = memberLocalized.GetType();
        _cachedRootPath[type] = memberLocalized.LocalizationRootPath;
        _cachedSuffixes[type] = memberLocalized.LocalizationSuffixes;
    }

    public static LocalizedText? GetLocalizedText(MemberInfo memberInfo, string suffix)
    {
        var type = memberInfo.DeclaringType;
        if (!type.IsAssignableTo(typeof(IMemberLocalized)))
            return null;
        var root = GetRootPath(type);
        return Language.GetOrRegister($"{root}.{memberInfo.Name}.{suffix}", () => memberInfo.Name);
    }
    void ILoadable.Load(Mod mod)
    {
        AutoRegister(this);
    }
    string LocalizationRootPath { get; }
    IReadOnlyList<string> LocalizationSuffixes { get; }
    void ILoadable.Unload()
    {
    }
}