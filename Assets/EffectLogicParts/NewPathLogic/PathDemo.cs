using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PathDemo : MonoBehaviour
{
    public Mesh mesh;
    public AlphabethScriptableObject alphabethScriptableObject;
    public ChapterScriptableObject chapter;

    private void Test()
    {
        Path p = MeshToPath.ConvertMeshToPath(mesh,  0.1f);
        p = p.Scale(50);
        PathLineRenderer plr = PathLineRenderer.CreateFromPath(p);
        
        VFXUtil.CreateEffectFromPath(p, TextUtil.ToSingleLine(chapter.content), alphabethScriptableObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Test();
        }
    }
}
