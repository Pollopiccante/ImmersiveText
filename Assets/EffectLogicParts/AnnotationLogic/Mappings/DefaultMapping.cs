
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class DefaultTextDimensionsDataPoint : BasicDimensionDataPoint 
{
    public AngryDimension Angry = new AngryDimension();
    public HumorDimension Humor = new HumorDimension();
    
}

public class DefaultEffectDimensionsDataPoint : EffectDimensionDataPoint
{
    public ScaleEffectDimension Scale = new ScaleEffectDimension();
    public ColorEffectDimension Color = new ColorEffectDimension();
    public XWaveMotionEffectDimension XWave = new XWaveMotionEffectDimension();
    public AlphaEffectDimension Alpha = new AlphaEffectDimension();
    public SmoothnessEffectDimension Smoothness = new SmoothnessEffectDimension();
    public MetalicEffectDimension Metalic = new MetalicEffectDimension();
    public FlyInIndexTimeEffectDimension FlyInIndexTime = new FlyInIndexTimeEffectDimension();
}

[CreateAssetMenu(fileName = "TextArangement", menuName = "ScriptableObjects/Annotation/TextArangement", order = 0)]
public class DefaultTextArangement : TextArangement<DefaultTextDimensionsDataPoint, DefaultEffectDimensionsDataPoint> { }

public class DefaultMapping : AbstractMapping<DefaultTextDimensionsDataPoint, DefaultEffectDimensionsDataPoint>
{
    public override DefaultEffectDimensionsDataPoint Convert(DefaultTextDimensionsDataPoint textDimensions)
    {
        DefaultEffectDimensionsDataPoint outDimensions = new DefaultEffectDimensionsDataPoint();

        if (textDimensions.Angry.value > 0.5f)
        {
            outDimensions.Scale.value = 5f;
            outDimensions.Color.value = new Color(255, 0, 0);
            outDimensions.XWave.value = new WaveMotionData(0.005f, 200, 0);
            
            outDimensions.Alpha.value = 0.2f;
            outDimensions.FlyInIndexTime.value = 0.5f;
        }
        else
        {
            outDimensions.XWave.value = new WaveMotionData(0, 0, 0);
            outDimensions.Metalic.value = 1f;
            outDimensions.FlyInIndexTime.value = 5f;
        }

        if (textDimensions.Humor.value > 1f)
        {
            Color randomColor = Random.ColorHSV();
            outDimensions.Color.value = randomColor;
        }

        // pass through letter and subPathStrategy
        outDimensions.Letter = textDimensions.Letter;
        outDimensions.SubPathStrategy = textDimensions.SubPathStrategy;
        
        return outDimensions;
    }
}

