using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MobyDickTextDimensionsDataPoint : BasicDimensionDataPoint 
{
    // moby dick annotation dimensions:
    // 1. urban <-> nature (float: -1 to 1)
    // 2. sea related (float: 0 to 1)
    // 3. quiet vs loud (float: -1 to 1)
    // 4. profound vs basic (string: profound | basic | default)
    // 5. sadness (0 to 1)
    // 6. narration (string: all-knowing | direct-to-reader | default)
    // 7. character (string: Ismael | Captain Ahab)
    public UrbanNatureDimension UrbanNature = new UrbanNatureDimension();
    public SeaRelatedDimension SeaRelated = new SeaRelatedDimension();
    public SilentLoudDimension SilentLoud = new SilentLoudDimension();
    public ProfoundBasicDimension ProfoundBasic = new ProfoundBasicDimension();
    public SadnessDimension Sadness = new SadnessDimension();
    public NarrationDimension Narration = new NarrationDimension();
    public CharacterDimension Character = new CharacterDimension();
    
}

public class MobyDickEffectDimensionsDataPoint : EffectDimensionDataPoint
{
    public ScaleEffectDimension Scale = new ScaleEffectDimension();
    public ColorEffectDimension Color = new ColorEffectDimension();
    public XWaveMotionEffectDimension XWave = new XWaveMotionEffectDimension();
    public AlphaEffectDimension Alpha = new AlphaEffectDimension();
    public SmoothnessEffectDimension Smoothness = new SmoothnessEffectDimension();
    public MetalicEffectDimension Metalic = new MetalicEffectDimension();
    public IndexStartEndEffectDimension IndexStartEnd = new IndexStartEndEffectDimension();
}

[CreateAssetMenu(fileName = "MobyDickTextArangement", menuName = "ScriptableObjects/Annotation/MobyDickTextArangement", order = 0)]
public class MobyDickTextArangement : TextArangement<MobyDickTextDimensionsDataPoint, MobyDickEffectDimensionsDataPoint> { }


public class MobyDickMapping : AbstractMapping<MobyDickTextDimensionsDataPoint, MobyDickEffectDimensionsDataPoint>
{
    public override MobyDickEffectDimensionsDataPoint Convert(MobyDickTextDimensionsDataPoint textDimensions, int index)
    {
        MobyDickEffectDimensionsDataPoint outDimensions = new MobyDickEffectDimensionsDataPoint();

        // default fly in, start end index
        outDimensions.IndexStartEnd.value = new StartEndIndex(index, index + 1);
        
        // effect dimension parts influenced by multiple text dimensions
        Color finalColor = new Color(155, 155, 155);
        float finalScale = 3f;
        
        // MAPPING:
        // Sadness: smaller, add darkness to color
        const float lowSadnessScale = 1f;
        const float highSadnessScale = 0.3f;
        const float lowSadnessDarknessOffset = 0;
        const float highSadnessDarknessOffset = 230;
        
        
        
        // Character: Ishmael: Blue
        if (textDimensions.Character.value == "Ishmael")
        {
            finalColor = new Color(0, 0, 255);
        }
        // Sadness: darker, and smaller
        if (textDimensions.Sadness.value > 0f)
        {
            finalScale *= Mathf.Lerp(lowSadnessScale, highSadnessScale, textDimensions.Sadness.value);
            float darknessOffset = Mathf.Lerp(lowSadnessDarknessOffset, highSadnessDarknessOffset, textDimensions.Sadness.value);
            finalColor = new Color(finalColor.r - darknessOffset, finalColor.g - darknessOffset, finalColor.b - darknessOffset);
        }
        // Basic: no metallic reflection, add some randomness to color, add some randomness to scale (minimal)
        if (textDimensions.ProfoundBasic.value == "basic")
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
            outDimensions.Metalic.value = 0f;
            Color randomColor = Random.ColorHSV(hMin, hMax, sMin, sMax, vMin, vMax);
            finalColor = Color.Lerp(finalColor, new Color(randomColor.r * 255, randomColor.g * 255, randomColor.b * 255), approachPercentage);
            // scale
            const float scaleRandomness = 0.1f;
            finalScale *= Random.Range(1 - scaleRandomness / 2f, 1 + scaleRandomness / 2f);
        }
        // Profound: max metallic reflection, max white, slow movement, slow fly in, slightly bigger
        if (textDimensions.ProfoundBasic.value == "profound")
        {
            outDimensions.Metalic.value = 1f;
            finalColor = new Color(255, 255, 255);
            
            outDimensions.XWave.value = new WaveMotionData(0.1f, 0.5f, 0);
            finalScale *= 1.3f;
            const float flyInTime = 10f;
            float endIndex = index + 1;
            float startIndex = Mathf.Max((index + 1) - flyInTime, 0);
            outDimensions.IndexStartEnd.value = new StartEndIndex(startIndex, endIndex);
        }
        
        
        outDimensions.Color.value = finalColor;
        outDimensions.Scale.value = finalScale;
        
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
        outDimensions.Letter = textDimensions.Letter;
        outDimensions.SubPathStrategy = textDimensions.SubPathStrategy;

        return outDimensions;
    }
}