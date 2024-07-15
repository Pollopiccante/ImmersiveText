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
    
    public static VFXDataScriptableObject CreateVFXDataFromPath(Path path, string text, AlphabethScriptableObject alphabet)
    {
        // get textures as pCaches
        TextInsertionResult insertionResult = path.Copy().ConvertToPointData(text, alphabet);

        // convert pCaches to textures
        Texture2D positionTexture = PointCacheToTexture2D(insertionResult.positionsTexture);
        Texture2D rotationTexture = PointCacheToTexture2D(insertionResult.rotationsTexture);
        Texture2D letterTexture = PointCacheToTexture2D(insertionResult.lettersTexture);
        
        // create vfx data object, assign textures 
        VFXDataScriptableObject vfxData = ScriptableObject.CreateInstance<VFXDataScriptableObject>();
        vfxData.positionTexture = positionTexture;
        vfxData.rotationTexture = rotationTexture;
        vfxData.letterTexture = letterTexture;
        // assign additional information
        vfxData.textureDimension = insertionResult.textureDimension;
        vfxData.letterScale = 1;
        vfxData.numberOfElements = text.Replace(" ", "").Length;
        // assign alphabet
        vfxData.meshAlphabet = alphabet;

        return vfxData;
    }

    public static void CreateEffectFromPath(Path path, string text, AlphabethScriptableObject alphabet)
    {
        CreateEffectFromVFXData(CreateVFXDataFromPath(path, text, alphabet));
    }
    
    private static Texture2D PointCacheToTexture2D(List<Vector3> pointCache)
    {
        int dimension = Mathf.CeilToInt(Mathf.Sqrt(pointCache.Count));
        Texture2D texture = new Texture2D(dimension, dimension, TextureFormat.RGBAFloat, false);
        for (int i = 0; i < pointCache.Count; i++)
        {
            Vector3 vec = pointCache[i];
            int x = i % dimension;
            int y = i / dimension;
            texture.SetPixel(x,y,new Color(vec.x, vec.y, vec.z));
        }
        texture.Apply();
        return texture;
    }
}
