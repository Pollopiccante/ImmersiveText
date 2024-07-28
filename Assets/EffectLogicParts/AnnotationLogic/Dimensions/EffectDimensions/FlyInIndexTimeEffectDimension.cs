
public class FlyInIndexTimeEffectDimension : EffectDimension<float>
{
    public override void Apply(VfxDataPoint dataPoint)
    {
        dataPoint.flyInIndexTime = value;
    }

    public override float GetDefaultValue()
    {
        return 1;
    }
}
