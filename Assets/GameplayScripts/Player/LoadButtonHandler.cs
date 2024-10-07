using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.VFX;

public class LoadButtonHandler : MonoBehaviour
{

    public VFXDataScriptableObject threeDPrintingVfxData;
    public VFXDataScriptableObject randomWalkVfxData;
    public VFXDataScriptableObject randomWalkAndObjectsVfxData;
    public VFXDataScriptableObject hilbertCurveVfxData;
    public VFXDataScriptableObject fractalVfxData;

    public DistanceBasedReaderController readerController;
    
    private GameObject threeDPrintingGo;
    private GameObject randomWalkGo;
    private GameObject randomWalkAndObjectsGo;
    private GameObject hilbertCurveGo;
    private GameObject fractalGo;

    private void ClearAll()
    {
        if (threeDPrintingGo != null)
            DestroyImmediate(threeDPrintingGo);
        if (randomWalkGo != null)
            DestroyImmediate(randomWalkGo);
        if (randomWalkAndObjectsGo != null)
            DestroyImmediate(randomWalkAndObjectsGo);
        if (hilbertCurveGo != null)
            DestroyImmediate(hilbertCurveGo);
        if (fractalGo != null)
            DestroyImmediate(fractalGo);
    }

    private void SetEffectOfReader(VisualEffect visualEffect)
    {
        readerController.SetEffect(visualEffect);
        readerController.Reset(); // reset reading position
    }
    
    public void Load3dPrinting()
    {
        ClearAll();
        threeDPrintingGo = VFXUtil.CreateEffectFromVFXData(threeDPrintingVfxData);
        SetEffectOfReader(threeDPrintingGo.GetComponent<VisualEffect>());
    }
    public void LoadRandomWalk()
    {
        ClearAll();
        randomWalkGo = VFXUtil.CreateEffectFromVFXData(randomWalkVfxData);
        SetEffectOfReader(randomWalkGo.GetComponent<VisualEffect>());
    }
    public void LoadRandomWalkAndObjects()
    {
        ClearAll();
        randomWalkAndObjectsGo = VFXUtil.CreateEffectFromVFXData(randomWalkAndObjectsVfxData);
        SetEffectOfReader(randomWalkAndObjectsGo.GetComponent<VisualEffect>());
    }
    public void LoadHilbertCurve()
    {
        ClearAll();
        hilbertCurveGo = VFXUtil.CreateEffectFromVFXData(hilbertCurveVfxData);
        SetEffectOfReader(hilbertCurveGo.GetComponent<VisualEffect>());
    }
    public void LoadFractal()
    {
        ClearAll();
        fractalGo = VFXUtil.CreateEffectFromVFXData(fractalVfxData);
        SetEffectOfReader(fractalGo.GetComponent<VisualEffect>());
    }
}
