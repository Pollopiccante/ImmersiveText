using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MyTestMapping : GenericMapping
{
    public override List<string> GetAdditionalTextDimensions()
    {
        throw new System.NotImplementedException();
    }

    public override List<string> GetAdditionalEffectDimensions()
    {
        throw new System.NotImplementedException();
    }


    private class TextDim
    {
        public static readonly string LETTER = "Letter";
        public static readonly string SUBPATH = "SubPath";
        public static readonly string SADNESS = "Sadness";
        public static readonly string CHARACTER = "Character";
        public static readonly string PROFOUND_BASIC = "ProfoundBasic";
        public static readonly string SILENT_LOUD = "SilentLoud";
        public static readonly string SEA_REALATED = "SeaRelated";
        public static readonly string URBAN_NATURE = "UrbanNature";
    }
    
    private class EffectDim
    {
        public static readonly string LETTER = "Letter";
        public static readonly string SUBPATH = "SubPath";
        public static readonly string INDEX_START = "indexStart";
        public static readonly string INDEX_END = "indexEnd";
        
        public static readonly string COLOR = "color";
        public static readonly string SCALE = "scale";
        public static readonly string METALIC = "metallic";
        public static readonly string XWave = "xwave";
    }
    
    public override void ApplyToVFXDataPoint(VfxDataPoint dpToApplyTo, Dictionary<string, object> inputData)
    {
        
        if (inputData.Keys.Contains(EffectDim.LETTER))
            dpToApplyTo.letter = (char)inputData[EffectDim.LETTER];      
        if (inputData.Keys.Contains(EffectDim.SUBPATH))
            dpToApplyTo.subPathStrategy = (PathStrategy)inputData[EffectDim.SUBPATH];
        if (inputData.Keys.Contains(EffectDim.INDEX_START))
            dpToApplyTo.indexStart = (int)inputData[EffectDim.INDEX_START];
        if (inputData.Keys.Contains(EffectDim.INDEX_END))
            dpToApplyTo.indexEnd = (int)inputData[EffectDim.INDEX_END];
        if (inputData.Keys.Contains(EffectDim.COLOR))
            dpToApplyTo.color = (Color) inputData[EffectDim.COLOR];
        if (inputData.Keys.Contains(EffectDim.SCALE))
            dpToApplyTo.scale = (float) inputData[EffectDim.SCALE];
        if (inputData.Keys.Contains(EffectDim.METALIC))
            dpToApplyTo.metalic = (float) inputData[EffectDim.METALIC];
        if (inputData.Keys.Contains(EffectDim.XWave))
            dpToApplyTo.XWaveMotion = (WaveMotionData) inputData[EffectDim.XWave];

    }

    private void FillDefaults(Dictionary<string, object> inpDict)
    {
        
        Dictionary<string, object> defaultDict = new Dictionary<string, object>()
        {
            [TextDim.LETTER] = "@",
            [TextDim.SADNESS] = 0f,
            [TextDim.SILENT_LOUD] = 0f,
            [TextDim.SEA_REALATED] = 0f,
            [TextDim.SUBPATH] = SkipPathStrategy.Default(),
            [TextDim.CHARACTER] = "",
            [TextDim.URBAN_NATURE] = 0f,
            [TextDim.PROFOUND_BASIC] = "",
        };

        HashSet<string> nullKeys = new HashSet<string>();
        foreach (string key in inpDict.Keys)
            if (inpDict[key] == null)
                nullKeys.Add(key);
        foreach (string key in nullKeys)
        {
            inpDict[key] = defaultDict[key];
        }
        
    }

    public override Dictionary<string, object> SimpleConvert(Dictionary<string, object> textDimensions, int index)
    {
        FillDefaults(textDimensions);
        Dictionary<string, object> dataOut = new Dictionary<string, object>();
        
        // foreach (string textDimensionsKey in textDimensions.Keys)
        // {
        //     object value = textDimensions[textDimensionsKey];
        //     if (value == null)
        //         continue;
        //     
        //     if (textDimensionsKey.Equals("Sadness"))
        //         dataOut.Add("blueness",(float)value);
        //     if (textDimensionsKey.Equals("Letter"))
        //         dataOut.Add("Letter",(char)value);
        //     if (textDimensionsKey.Equals("SubPath"))
        //         dataOut.Add("SubPath",(PathStrategy)value);
        // }
        // dataOut.Add("indexStart", index);
        // dataOut.Add("indexEnd", index + 1);
        
        
        /////////////////////////////////////////
        
        // default fly in, start end index
        dataOut[EffectDim.INDEX_START] = index;
        dataOut[EffectDim.INDEX_END] = index + 1;
        
        // effect dimension parts influenced by multiple text dimensions
        float colorProgress = (Mathf.Sin(((index % 30) / 30f * 360f) * Mathf.Deg2Rad) + 1) / 2f;
        Color finalColor = Color.Lerp(new Color(0,0,128), new Color(173,216,230), colorProgress); // default color is sin wave blue
        float finalScale = 2.1f;
        
        // MAPPING:
        // Sadness: smaller, add darkness to color
        const float lowSadnessScale = 1f;
        const float highSadnessScale = 0.3f;
        const float lowSadnessDarknessOffset = 0;
        const float highSadnessDarknessOffset = 230;

        // Character: Ishmael: Blue
        if ((string)textDimensions[TextDim.CHARACTER] == "Ishmael")
        {
            finalColor = new Color(0, 0, 255);
        }
        // Sadness: darker, and smaller
        if ((float)textDimensions[TextDim.SADNESS] > 0f)
        {
            finalScale *= Mathf.Lerp(lowSadnessScale, highSadnessScale, (float)textDimensions[TextDim.SADNESS]);
            float darknessOffset = Mathf.Lerp(lowSadnessDarknessOffset, highSadnessDarknessOffset, (float)textDimensions[TextDim.SADNESS]);
            finalColor = new Color(finalColor.r - darknessOffset, finalColor.g - darknessOffset, finalColor.b - darknessOffset);
        }
        // Basic: no metallic reflection, add some randomness to color, add some randomness to scale (minimal)
        if ((string)textDimensions[TextDim.PROFOUND_BASIC] == "basic")
        {
            const float randomnessDegree = 0.2f;
            const float approachPercentage = 0.8f;
            // color
            float h, s, v;
            Color.RGBToHSV(finalColor, out h, out s, out v);
            float hMax = (h + randomnessDegree / 2f) % 1f;
            float hMin = (h - randomnessDegree / 2f) % 1f;
            float sMax = (s + randomnessDegree / 2f) % 1f;
            float sMin = (s - randomnessDegree / 2f) % 1f;
            float vMax = (v + randomnessDegree / 2f) % 1f;
            float vMin = (v - randomnessDegree / 2f) % 1f;
            dataOut[EffectDim.METALIC] = 0f;
            Color randomColor = Random.ColorHSV(hMin, hMax, sMin, sMax, vMin, vMax);
            finalColor = Color.Lerp(finalColor, new Color(randomColor.r * 255, randomColor.g * 255, randomColor.b * 255), approachPercentage);
            // scale
            const float scaleRandomness = 0.1f;
            finalScale *= Random.Range(1 - scaleRandomness / 2f, 1 + scaleRandomness / 2f);
        }
        // Profound: max metallic reflection, max white, slow movement, slow fly in, slightly bigger
        if ((string)textDimensions[TextDim.PROFOUND_BASIC] == "profound")
        {
            dataOut[EffectDim.METALIC] = 1f;
            finalColor = new Color(255, 255, 255);
            
            dataOut[EffectDim.XWave] = new WaveMotionData(0.1f, 0.5f, 0);
            finalScale *= 1.3f;
            const float flyInTime = 10f;
            float endIndex = index + 1;
            float startIndex = Mathf.Max((index + 1) - flyInTime, 0);
            dataOut[EffectDim.INDEX_START] = (int)startIndex;
            dataOut[EffectDim.INDEX_END] = (int)endIndex;
        }
        
        // Silent: small
        // Loud: big
        const float silentLoundScalingFactor = 2f;
        if ((float)textDimensions[TextDim.SILENT_LOUD] < 0f)
        {
            finalScale /= Mathf.Abs((float)textDimensions[TextDim.SILENT_LOUD]) * silentLoundScalingFactor;
        }else if ((float)textDimensions[TextDim.SILENT_LOUD] > 0f)
        {
            finalScale *= Mathf.Abs((float)textDimensions[TextDim.SILENT_LOUD]) * silentLoundScalingFactor;
        }
        
        // sea related: wave motion, dark blue and white
        if ((float)textDimensions[TextDim.SEA_REALATED] != 0f)
        {
            Color[] deepOceanColors = new Color[5];
            ColorUtility.TryParseHtmlString("#001a33", out deepOceanColors[0]);
            ColorUtility.TryParseHtmlString("#003366", out deepOceanColors[1]);
            ColorUtility.TryParseHtmlString("#004080", out deepOceanColors[2]);
            ColorUtility.TryParseHtmlString("#0059b3", out deepOceanColors[3]);
            ColorUtility.TryParseHtmlString("#0066cc", out deepOceanColors[4]);

            int colorToPickFrom = Mathf.CeilToInt((float)textDimensions[TextDim.SEA_REALATED] * 5);
            int pickedColorIndex = Random.Range(0, Math.Min(colorToPickFrom, 5));
            Color pickedColor = deepOceanColors[pickedColorIndex];

            finalColor = Color.Lerp(finalColor, pickedColor, Mathf.Max(0.4f, (float)textDimensions[TextDim.SEA_REALATED]));
            
            // wave motion
            dataOut[EffectDim.XWave] = new WaveMotionData((float)textDimensions[TextDim.SEA_REALATED] / 2, 0.5f, (index % 10f) / 10f);
        }
        
        // Urban: fixed concrete size ratios (1, 2, 3, 4), like different sized Buildings
        // Nature: Developing Golden Ratio
        float scaleStep = finalScale / 2;
        if ((float)textDimensions[TextDim.URBAN_NATURE] < 0f) // Urban
        {
            finalScale = scaleStep * (1 + Random.Range(0, 4));
        }else if ((float)textDimensions[TextDim.URBAN_NATURE] > 0f) // Nature
        {
            float progressionAngle = (index % 30) / 30f * 360f;
            float rad = progressionAngle * Mathf.Deg2Rad;
            float sin = Mathf.Sin(rad);
            float sinProgress = (sin + 1) / 2f;
            finalScale = (scaleStep / 2f) + sinProgress * (scaleStep * 3.5f);
        }

        dataOut[EffectDim.COLOR] = finalColor;
        dataOut[EffectDim.SCALE] = finalScale;

        // if (textDimensions.Sadness.value > 0.5f)
        // {
        //     outDimensions.Scale.value = 5f;
        //     outDimensions.Color.value = new Color(255, 0, 0);
        //     outDimensions.XWave.value = new WaveMotionData(0.005f, 200, 0);
        //     
        //     outDimensions.Alpha.value = 0.2f;
        //     outDimensions.IndexStartEnd.value = new StartEndIndex(index, index + 10);
        // }
        // else
        // {
        //     outDimensions.XWave.value = new WaveMotionData(0, 0, 0);
        //     outDimensions.Metalic.value = 1f;
        //     outDimensions.IndexStartEnd.value = new StartEndIndex(index, index + 1);
        //     
        //     Color randomColor = Random.ColorHSV();
        //     outDimensions.Color.value = new Color(randomColor.r * 255, randomColor.g * 255, randomColor.b * 255);
        //     
        // }

        // pass through letter and subPathStrategy
        dataOut[EffectDim.LETTER] = textDimensions[TextDim.LETTER];
        dataOut[EffectDim.SUBPATH] = (PathStrategy)textDimensions[TextDim.SUBPATH];

        return dataOut;
    }

    public VfxDataPoint ApplyEffects()
    {
        throw new System.NotImplementedException();

    }
}
