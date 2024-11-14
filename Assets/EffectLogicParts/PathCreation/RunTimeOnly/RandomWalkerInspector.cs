#if (UNITY_EDITOR)

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RandomWalker), true)]
public class RandomWalkerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        RandomWalker castTarget = (RandomWalker) target;
        
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Reset"))
        {
            castTarget.Reset();
        }

        if (GUILayout.Button("Save as Scriptable Object"))
        {
           castTarget.SaveAsScriptableObject();
        }
    }
}
#endif