
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

