using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;


[RequireComponent(typeof(VisualEffect))]
public class FinishedListener : VFXOutputEventAbstractHandler
{
    public override bool canExecuteInEditor => true;

    public FinishedListener()
    {
        outputEvent = "Finished";
    }
    
    private VisualEffect _nextEffect;
    public void SetNextEffect(VisualEffect nextEffect)
    {
        
        _nextEffect = nextEffect;
    }
    
    public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute)
    {
        if (_nextEffect != null)
            _nextEffect.pause = false;
    }
}
