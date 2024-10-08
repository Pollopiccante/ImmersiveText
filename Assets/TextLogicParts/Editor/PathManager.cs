using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathManager : EditorWindow
{
    [MenuItem("Window/PathManager")]
    public static void ShowWindow()
    {
        GetWindow<PathManager>("PathManager");
    }


    public Object strategy;
    public GCodePathStrategy pathStrategy;
    
    
    private void OnGUI()
    {
        GUILayout.Label("Text to Scriptable Object Conversion");
        GUILayout.BeginVertical();
        
        strategy = EditorGUILayout.ObjectField("Strategy", strategy, typeof(Object), true);
        pathStrategy = (GCodePathStrategy)strategy;
        
        if (GUILayout.Button("Generate Path Line Renderer"))
        {
            PathLineRenderer.CreateFromPath(this.pathStrategy.GetPathPrototype());
        }
        GUILayout.EndVertical();
    }
}
