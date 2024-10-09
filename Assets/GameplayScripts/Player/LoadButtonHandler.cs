using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class LoadButtonHandler : MonoBehaviour
{

    public VFXDataScriptableObject threeDPrintingVfxData;
    public VFXDataScriptableObject randomWalkVfxData;
    public VFXDataScriptableObject randomWalkAndObjectsVfxData;
    public VFXDataScriptableObject hilbertCurveVfxData;
    public VFXDataScriptableObject fractalVfxData;
    public InputField readerSpeedTextField;
    public InputField flyInSpeedTextField;
    public InputField currentIndexTextField;
    
    
    public DistanceBasedReaderController readerController;
    
    private GameObject threeDPrintingGo;
    private GameObject randomWalkGo;
    private GameObject randomWalkAndObjectsGo;
    private GameObject hilbertCurveGo;
    private GameObject fractalGo;

    [CanBeNull] private GameObject currentEffect = null;
    
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
        currentEffect = threeDPrintingGo;
    }
    public void LoadRandomWalk()
    {
        ClearAll();
        randomWalkGo = VFXUtil.CreateEffectFromVFXData(randomWalkVfxData);
        SetEffectOfReader(randomWalkGo.GetComponent<VisualEffect>());
        currentEffect = randomWalkGo;

    }
    public void LoadRandomWalkAndObjects()
    {
        ClearAll();
        randomWalkAndObjectsGo = VFXUtil.CreateEffectFromVFXData(randomWalkAndObjectsVfxData);
        SetEffectOfReader(randomWalkAndObjectsGo.GetComponent<VisualEffect>());
        currentEffect = randomWalkAndObjectsGo;

    }
    public void LoadHilbertCurve()
    {
        ClearAll();
        hilbertCurveGo = VFXUtil.CreateEffectFromVFXData(hilbertCurveVfxData);
        SetEffectOfReader(hilbertCurveGo.GetComponent<VisualEffect>());
        currentEffect = hilbertCurveGo;

    }
    public void LoadFractal()
    {
        ClearAll();
        fractalGo = VFXUtil.CreateEffectFromVFXData(fractalVfxData);
        SetEffectOfReader(fractalGo.GetComponent<VisualEffect>());
        currentEffect = fractalGo;

    }

    public void ApplyControlOptions()
    {
        if (currentEffect == null)
            return;
        
        currentEffect.GetComponent<ReaderStepper>().indicesPerSecond = float.Parse(readerSpeedTextField.text);
        currentEffect.GetComponent<IndexStepper>().stepsPerSecond = float.Parse(flyInSpeedTextField.text);
        currentEffect.GetComponent<IndexStepper>().currentIndexTime = float.Parse(currentIndexTextField.text);
    }

    public void LoadDataToTextFields()
    {
        if (currentEffect == null)
            return;

        readerSpeedTextField.SetTextWithoutNotify(currentEffect.GetComponent<ReaderStepper>().indicesPerSecond.ToString());
        flyInSpeedTextField.SetTextWithoutNotify(currentEffect.GetComponent<IndexStepper>().stepsPerSecond.ToString());
        currentIndexTextField.SetTextWithoutNotify(currentEffect.GetComponent<IndexStepper>().currentIndexTime.ToString());
    }
}
