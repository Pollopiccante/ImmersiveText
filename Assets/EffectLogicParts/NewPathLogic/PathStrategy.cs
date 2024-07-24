using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class PathStrategy: ScriptableObject
{
    public abstract Path GetPath(string text, List<float> letterScaling, AlphabethScriptableObject alphabet);
}
