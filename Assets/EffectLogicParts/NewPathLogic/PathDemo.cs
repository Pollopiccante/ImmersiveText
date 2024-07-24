using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PathDemo : MonoBehaviour
{
    public TextArangement<DefaultTextDimensionsDataPoint, DefaultEffectDimensionsDataPoint> textArangement;
    public Mesh mesh;
    public AlphabethScriptableObject alphabethScriptableObject;
    public ChapterScriptableObject chapter;

    private void Test()
    {
        // Path p = MeshToPath.ConvertMeshToPath(mesh,  0.1f);
        // p = p.Scale(50);
        // PathLineRenderer plr = PathLineRenderer.CreateFromPath(p);
        //
        // List<float> letterScaling = Enumerable.Repeat(1f ,TextUtil.ToSingleLine(chapter.content).Replace(" ", "").Length).ToList();
        //
        //
        // VFXUtil.CreateEffectFromPath(p, TextUtil.ToSingleLine(chapter.content), alphabethScriptableObject, letterScaling);
        textArangement.CreateVfxEffect();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Test();
        }
    }
}
