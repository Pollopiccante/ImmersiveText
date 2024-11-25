using System.Collections.Generic;
using System.Linq;

public abstract class AbstractMapping<TextDimensions, EffectDimensions>
{
    public abstract EffectDimensions Convert(TextDimensions textDimensions, int index);
    public List<EffectDimensions> ConvertMany(List<TextDimensions> textDimensionsListing)
    {
        
        return textDimensionsListing.Select((elem, i) => Convert(elem, i)).ToList();
    }
    
}


public abstract class GenericMapping : AbstractMapping<Dictionary<string, object>, Dictionary<string, object>>
{
    private List<string> basicTextDimensions;
    private List<string> basicEffectDimensions;
    
    public abstract List<string> GetAdditionalTextDimensions();
    public abstract List<string> GetAdditionalEffectDimensions();

    public abstract void ApplyToVFXDataPoint(VfxDataPoint dpToApplyTo, Dictionary<string, object> inputData);

    public abstract Dictionary<string, object> SimpleConvert(Dictionary<string, object> textDimensions, int index);
    
    public override Dictionary<string, object> Convert(Dictionary<string, object> textDimensions, int index)
    {
        return SimpleConvert(textDimensions, index);
    }

    public List<Dictionary<string, object>> NewConvertMany(List<Dictionary<string, object>> data)
    {
        return data.Select((d, i) => SimpleConvert(d, i)).ToList();
    }
}

