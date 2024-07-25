using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.VFX;
using Object = UnityEngine.Object;

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
        vfxComponent.SetTexture("Scalings", vfxData.scaleTexture);
        vfxComponent.SetTexture("Colors", vfxData.colorTexture);
        vfxComponent.SetTexture("XWaveMotions", vfxData.xWaveMotionTexture);
        vfxComponent.SetTexture("Alpha_Smoothness_Metalic", vfxData.alphaSmoothnessMetalicTexture);
        // mesh alphabet
        for (int i = 0; i < TextUtil.alphabeth.Length; i++)
            vfxComponent.SetMesh($"Mesh_ {i}", vfxData.meshAlphabet.meshes[i]);
    }
    
    public static VFXDataScriptableObject CreateVFXDataFromPath(Path path, string text, AlphabethScriptableObject alphabet, List<float> letterScaling)
    {
        // get textures as pCaches
        TextInsertionResult insertionResult = path.Copy().ConvertToPointData(text, alphabet, letterScaling);

        // convert pCaches to textures
        Texture2D positionTexture = PointCacheToTexture2D(insertionResult.positionsTexture);
        Texture2D rotationTexture = PointCacheToTexture2D(insertionResult.rotationsTexture);
        Texture2D letterTexture = PointCacheToTexture2D(insertionResult.lettersTexture);
        Texture2D scalesTexture = PointCacheToTexture2D(insertionResult.scalesTexture);
        
        // create vfx data object, assign textures 
        VFXDataScriptableObject vfxData = ScriptableObject.CreateInstance<VFXDataScriptableObject>();
        vfxData.positionTexture = positionTexture;
        vfxData.rotationTexture = rotationTexture;
        vfxData.letterTexture = letterTexture;
        vfxData.scaleTexture = scalesTexture;
        // assign additional information
        vfxData.textureDimension = insertionResult.textureDimension;
        vfxData.letterScale = 1;
        vfxData.numberOfElements = text.Replace(" ", "").Length;
        // assign alphabet
        vfxData.meshAlphabet = alphabet;

        return vfxData;
    }

    public static void CreateEffectFromPath(Path path, string text, AlphabethScriptableObject alphabet, List<float> letterScaling)
    {
        CreateEffectFromVFXData(CreateVFXDataFromPath(path, text, alphabet, letterScaling));
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
    
    public static VFXDataScriptableObject CreateVFXDataFromCompleteAnnotation<TextDimensions, EffectDimensions>(CompleteAnnotation<TextDimensions> completeAnnotation, AbstractMapping<TextDimensions, EffectDimensions> mapping, Path path)
    {
        // convert to text dimension data points
        List<TextDimensions> textElements = completeAnnotation.Finish();
        
        // apply mapping to effect dimension data point space
        List<EffectDimensions> effectElements = mapping.ConvertMany(textElements);

        // apply effect dimension elements to vfx data points#
        List<VfxDataPoint> vfxDataPoints = new List<VfxDataPoint>();
        foreach (EffectDimensions eddp in effectElements)
        {
            VfxDataPoint vfxDataPoint = new VfxDataPoint();
            eddp.GetType().GetMethod("Apply").Invoke(eddp, new []{vfxDataPoint});
            vfxDataPoints.Add(vfxDataPoint);
        }

        // create vfx data from effect points
        AlphabethScriptableObject alphabet = Resources.Load<AlphabethScriptableObject>("alphabet/alphabeth_Absans-Regular");
        VFXDataScriptableObject vfxDataScriptableObject = VFXUtil.ToVfxDataScriptableObject(vfxDataPoints, path, alphabet);

        return vfxDataScriptableObject;
    }

    public static void CreateEffectFromCompleteAnnotation<TextDimensions, EffectDimensions>(CompleteAnnotation<TextDimensions> completeAnnotation, AbstractMapping<TextDimensions, EffectDimensions> mapping, Path path)
    {
        VFXDataScriptableObject data = CreateVFXDataFromCompleteAnnotation(completeAnnotation, mapping, path);
        CreateEffectFromVFXData(data);
    }

    public static VFXDataScriptableObject ToVfxDataScriptableObject(List<VfxDataPoint> dataPoints, Path basePath, AlphabethScriptableObject alphabet)
    {
        // STEP 1: Construct the final basePath, by inserting subPaths as specified by the dataPoints
        // group data by form sections
        List<Path> subPaths = new List<Path>();
        PathStrategy currentStrategy = null;
        string groupText = "";
        List<float> letterScaling = new List<float>();
        for (int i = 1; i < dataPoints.Count; i++)
        {
            VfxDataPoint dataPoint = dataPoints[i];
            
            if (currentStrategy == null)
            {
                currentStrategy = dataPoint.subPathStrategy;
                groupText += dataPoint.letter;
                letterScaling.Add(dataPoint.scale);
            }
            else if (currentStrategy == dataPoint.subPathStrategy)
            {
                groupText += dataPoint.letter;
                letterScaling.Add(dataPoint.scale);
            }
            else
            {
                subPaths.Add(currentStrategy.GetPath(groupText, letterScaling, alphabet));
                // reset
                groupText = "";
                letterScaling = new List<float>();
                currentStrategy = dataPoint.subPathStrategy;
            }
        }
        subPaths.Add(currentStrategy.GetPath(groupText, letterScaling, alphabet));

        // insert all sub paths into the base path
        subPaths.ForEach(subPath =>
        {
            basePath.InsertSubPath(subPath);
        });

        // STEP 2: create vfx data by inserting the complete text into the base path
        // gather complete text and letter scaling, and colors
        string completeText = "";
        List<float> completeLetterScaling = new List<float>();
        
        for (int i = 0; i < dataPoints.Count; i++)
        {
            completeText += dataPoints[i].letter;
            completeLetterScaling.Add(dataPoints[i].scale);
        }

        // get textures as pCaches
        TextInsertionResult insertionResult = basePath.Copy().ConvertToPointData(completeText, alphabet, completeLetterScaling);
        
        // create vfx data object, assign textures 
        VFXDataScriptableObject vfxData = ScriptableObject.CreateInstance<VFXDataScriptableObject>();
        vfxData.positionTexture = PointCacheToTexture2D(insertionResult.positionsTexture);;
        vfxData.rotationTexture = PointCacheToTexture2D(insertionResult.rotationsTexture);
        vfxData.letterTexture = PointCacheToTexture2D(insertionResult.lettersTexture);
        vfxData.scaleTexture = PointCacheToTexture2D(insertionResult.scalesTexture);

        // create shortened independent textures
        List<Vector3> colors = new List<Vector3>();
        List<Vector3> xWaveMotions = new List<Vector3>();
        List<Vector3> alphaSmoothnessMetalic = new List<Vector3>();
        for (int i = 0; i < insertionResult.positionsTexture.Count; i++)
        {
            VfxDataPoint dp = dataPoints[i];
            
            colors.Add(new Vector3(dp.color.r / 255f,dp.color.g / 255f,dp.color.b / 255f));
            xWaveMotions.Add(dp.XWaveMotion.ToVector());
            alphaSmoothnessMetalic.Add(new Vector3(dp.alpha, dp.smoothness, dp.metalic));
        }

        // apply path independent textures:
        vfxData.colorTexture = PointCacheToTexture2D(colors);
        vfxData.xWaveMotionTexture = PointCacheToTexture2D(xWaveMotions);
        vfxData.alphaSmoothnessMetalicTexture = PointCacheToTexture2D(alphaSmoothnessMetalic);

        // assign additional information
        vfxData.textureDimension = insertionResult.textureDimension;
        vfxData.letterScale = 1;
        vfxData.numberOfElements = completeText.Replace(" ", "").Length;
        // assign alphabet
        vfxData.meshAlphabet = alphabet;
        
       
        return vfxData;
    }

    public static void SaveVFXData(VFXDataScriptableObject data, string name)
    {
        string fileNameTemplate = DirConfiguration.Instance.vfxDataScriptableObjectDir + DirConfiguration.GetPCacheFileNamingTemplate();
        string posFileName = String.Format(fileNameTemplate, name, "pos");
        string rotFileName = String.Format(fileNameTemplate, name, "rot");
        string letterFileName = String.Format(fileNameTemplate, name, "letter");
        string scaleFileName = String.Format(fileNameTemplate, name, "scale");
        string colorFileName = String.Format(fileNameTemplate, name, "color");
        string xWaveFileName = String.Format(fileNameTemplate, name, "xwave");
        string alphaSmoothnessMetalicFileName = String.Format(fileNameTemplate, name, "AlSmMe");
        
        
        WritePointCache(data.positionTexture, posFileName);
        WritePointCache(data.rotationTexture, rotFileName);
        WritePointCache(data.letterTexture, letterFileName);
        WritePointCache(data.scaleTexture, scaleFileName);
        WritePointCache(data.colorTexture, colorFileName);
        WritePointCache(data.xWaveMotionTexture, xWaveFileName);
        WritePointCache(data.alphaSmoothnessMetalicTexture, alphaSmoothnessMetalicFileName);
        
        AssetDatabase.Refresh();

        // import pCaches
        AssetDatabase.ImportAsset(posFileName, ImportAssetOptions.ForceUpdate);
        AssetDatabase.ImportAsset(rotFileName, ImportAssetOptions.ForceUpdate);
        AssetDatabase.ImportAsset(letterFileName, ImportAssetOptions.ForceUpdate);
        AssetDatabase.ImportAsset(scaleFileName, ImportAssetOptions.ForceUpdate);
        AssetDatabase.ImportAsset(colorFileName, ImportAssetOptions.ForceUpdate);
        AssetDatabase.ImportAsset(xWaveFileName, ImportAssetOptions.ForceUpdate);
        AssetDatabase.ImportAsset(alphaSmoothnessMetalicFileName, ImportAssetOptions.ForceUpdate);

        AssetDatabase.Refresh();

        // read position part for each pCache
        Texture2D positionTexture = ReadTextureFromPointCache(posFileName);
        Texture2D rotationTexture = ReadTextureFromPointCache(rotFileName);
        Texture2D letterTexture = ReadTextureFromPointCache(letterFileName);
        Texture2D scaleTexture = ReadTextureFromPointCache(scaleFileName);
        Texture2D colorTexture = ReadTextureFromPointCache(colorFileName);
        Texture2D xWaveMotionTexture = ReadTextureFromPointCache(xWaveFileName);
        Texture2D alphaSmoothnessMetalicTexture = ReadTextureFromPointCache(alphaSmoothnessMetalicFileName);
            
        // create vfx data object, assign textures 
        data.positionTexture = positionTexture;
        data.rotationTexture = rotationTexture;
        data.letterTexture = letterTexture;
        data.scaleTexture = scaleTexture;
        data.colorTexture = colorTexture;
        data.xWaveMotionTexture = xWaveMotionTexture;
        data.alphaSmoothnessMetalicTexture = alphaSmoothnessMetalicTexture;

        // save vfx data as asset
        AssetDatabase.CreateAsset(data, DirConfiguration.Instance.vfxDataScriptableObjectDir + name + ".asset");
        AssetDatabase.SaveAssets();
        Debug.Log($"SAVED TO: {DirConfiguration.Instance.vfxDataScriptableObjectDir + name + ".asset"}");
    }

    private static void WritePointCache(Texture2D texture2D, string fileName)
    {
        List<Vector3> convertedTexture = texture2D.GetPixels().Select(px => new Vector3(px.r, px.g, px.b)).ToList();
        WritePointCache(convertedTexture, fileName);
    }
    private static void WritePointCache(List<Vector3> data, string fileName)
    {
        // write point cache file
        using (StreamWriter sw = File.CreateText(fileName))
        {
            // header
            sw.WriteLine("pcache");
            sw.WriteLine("format ascii 1.0");
            sw.WriteLine("comment Exported with PCache.cs");
            sw.WriteLine($"elements {data.Count}");
            sw.WriteLine("property float position.x");
            sw.WriteLine("property float position.y");
            sw.WriteLine("property float position.z");
            sw.WriteLine("end_header");
            // points
            foreach (Vector3 cachePoint in data)
            {
                Vector3 pos = cachePoint;
                sw.WriteLine($"{pos.x} {pos.y} {pos.z}");
            }
            sw.Close();
        }
    }
    private static Texture2D ReadTextureFromPointCache(string filename)
    {
        Object[] allAssetsAtPath = AssetDatabase.LoadAllAssetsAtPath(filename);
        Texture2D positionTexture = null;
        foreach (Object asset in allAssetsAtPath)
            if (asset.name == "position")
                positionTexture = (Texture2D)asset;

        if (positionTexture == null)
            throw new Exception("position was not found");

        return positionTexture;
    }
    
}
