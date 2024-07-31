using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;


public enum AllowedGenericTypes
{
    INTEGER = 0,
    CHAR = 1,
    FLOAT = 2,
    STRING = 3,
    BOOL = 4,
    MESH_PATH_STRATEGY = 5,
    [Obsolete]
    GENERIC_SECTION = 6,
}

[Serializable]
public class ValueWrapper
{
    public object getValue(Type type)
    {
        if (type == typeof(char))
            return charValue;
        if (type == typeof(int))
            return integerValue;
        if (type == typeof(float))
            return floatValue;
        if (type == typeof(PathStrategy))
            return meshPathStrategyValue;
        
        throw new ArgumentException($"type {type} is not present in wrapper object");
    }
    
    public int integerValue;
    public char charValue;
    public float floatValue;
    public string stringValue;
    public bool boolValue;
    public PathStrategy meshPathStrategyValue;
    public GenericSection genericSectionValue;

    public static List<ValueWrapper> FromString(string s)
    {
        List<ValueWrapper> valuesList = new List<ValueWrapper>();
        foreach (char c in s)
        {
            ValueWrapper vw = new ValueWrapper();
            vw.charValue = c;
            valuesList.Add(vw);
        }
        return valuesList;
    }

    public static List<ValueWrapper> FromGenericSections(List<GenericSection> sections)
    {
        List<ValueWrapper> valuesList = new List<ValueWrapper>();
        foreach (GenericSection s in sections)
        {
            ValueWrapper vw = new ValueWrapper();
            vw.genericSectionValue = s;
            valuesList.Add(vw);
        }
        return valuesList;
    }
}



public abstract class GenericSection : ScriptableObject
{
    public AllowedGenericTypes type = AllowedGenericTypes.FLOAT;
    [HideInInspector]
    public ValueWrapper value = new ValueWrapper();
    [HideInInspector]
    public List<ValueWrapper> values = new List<ValueWrapper>();

    public abstract dynamic GetValueAt(int index);
    public abstract int GetLength();
    public abstract bool MustForceIterable();

    public bool Contains(int i)
    {
        return i < GetLength();
    }
 
    public virtual bool JustBaseInspector()
    {
        return false; 
    }
}

