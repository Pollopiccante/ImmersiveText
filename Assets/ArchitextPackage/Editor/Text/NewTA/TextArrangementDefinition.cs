using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
[Serializable]
public enum DimensionType
{
    String, Int, ZeroToOne
}
[Serializable]
public class DimensionDefinition
{
    public string DimensionName = "";
    public DimensionType DimensionType = DimensionType.String;
}


[CreateAssetMenu(fileName = "TADefinition", menuName = "ScriptableObjects/TextArrangementDefinition", order = 1)]
public class TextArrangementDefinition : ScriptableObject
{
    
    public List<DimensionDefinition> definedDimensions;
    public string mappingName;


}
