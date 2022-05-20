using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Station;
using UnityEngine;

public static class ReflectionUtils
{
    public static List<Type> FindSaveModuleTypes()
    {


        
        var lookupSingle = typeof(SaveModule<>);
        var lookupDictionary = typeof(AreaSaveModule<>);
        var foundsSingle = FindDerivedClassesFromAbstract(lookupSingle);
        var foundsDict = FindDerivedClassesFromAbstract(lookupDictionary);
        var foundTypes = foundsSingle.ToList();
        foundTypes.AddRange(foundsDict);
        
        return foundTypes; 
    }

    public static List<Type> FindAllClassFromInterface(Type target)
    {

        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => target.IsAssignableFrom(p))
            .Where(t => !t.Equals(target));
        return types.ToList();
    }
    public static List<Type> FindAllDbTypes()
    {
        var lookupSingle = typeof(SingleFieldDb<>);
        var lookupDictionary = typeof(DictGenericDatabase<>);
        var foundsSingle = FindDerivedClassesFromAbstract(lookupSingle);
        var foundsDict = FindDerivedClassesFromAbstract(lookupDictionary);
        var foundTypes = foundsSingle.ToList();
        foundTypes.AddRange(foundsDict);
        return foundTypes;
    }

    public static IEnumerable<Type> FindDerivedClasses(Type target)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(target));
    }

    public static List<Type> FindDerivedClassesFromAbstract(Type abstractType)
    {
        var lookup = abstractType;
        var founds = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(x => x.IsClass && !x.IsAbstract && x.IsInheritedFrom(lookup))
            .ToList();

        return founds;
    }

    private static bool IsInheritedFrom(this Type type, Type Lookup)
    {
        var baseType = type.BaseType;
        if (baseType == null)
            return false;

        if (baseType.IsGenericType
            && baseType.GetGenericTypeDefinition() == Lookup)
            return true;

        return baseType.IsInheritedFrom(Lookup);
    }
    
    public static Type[]  GetClassList<T>()
    {
        Type[] types = Assembly.GetExecutingAssembly().GetTypes();
        Type[] possible = (from Type type in types where type.IsSubclassOf(typeof(T)) select type).ToArray();

        return possible;
    }
}
