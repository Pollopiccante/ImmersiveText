using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXUtil
{
    public static void CreateEffectFromVFXData(VFXDataScriptableObject vfxData)
    {
        // create vfx instance
        GameObject vfxObject = GameObject.Instantiate(Resources.Load("Prefabs/StaticWordMeshEffect") as GameObject, new Vector3(0, 0, 0), Quaternion.identity);
        VisualEffect vfxComponent = vfxObject.GetComponent<VisualEffect>();

        // assign data to vfx effect
        // basics
        vfxComponent.SetInt("NumberOfElement", vfxData.numberOfElements);
        vfxComponent.SetFloat("LetterScale", vfxData.letterScale);
        // textures
        vfxComponent.SetInt("Dimension", vfxData.textureDimension);
        vfxComponent.SetTexture("Positions", vfxData.positionTexture);
        vfxComponent.SetTexture("Rotations", vfxData.rotationTexture);
        vfxComponent.SetTexture("Letters", vfxData.letterTexture);
        // mesh alphabet
        for (int i = 0; i < TextUtil.alphabeth.Length; i++)
            vfxComponent.SetMesh($"Mesh_ {i}", vfxData.meshAlphabet.meshes[i]);
    }
}
