using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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


    public override void ApplyToVFXDataPoint(VfxDataPoint dpToApplyTo, Dictionary<string, object> inputData)
    {
        
        if (inputData.Keys.Contains("blueness"))
            dpToApplyTo.color = new Color(255, 255, 255 * (float)inputData["blueness"]);
        if (inputData.Keys.Contains("Letter"))
            dpToApplyTo.letter = (char)inputData["Letter"];
        if (inputData.Keys.Contains("indexStart"))
            dpToApplyTo.indexStart = (int)inputData["indexStart"];
        if (inputData.Keys.Contains("indexEnd"))
            dpToApplyTo.indexEnd = (int)inputData["indexEnd"];

    }

    public override Dictionary<string, object> SimpleConvert(Dictionary<string, object> textDimensions, int index)
    {
        Dictionary<string, object> dataOut = new Dictionary<string, object>();
        
        
        foreach (string textDimensionsKey in textDimensions.Keys)
        {
            object value = textDimensions[textDimensionsKey];
            if (value == null)
                continue;
            
            if (textDimensionsKey.Equals("Sadness"))
                dataOut.Add("blueness",(float)value);
            if (textDimensionsKey.Equals("Letter"))
                dataOut.Add("Letter",(char)value);
        }
        dataOut.Add("indexStart", index);
        dataOut.Add("indexEnd", index + 1);
        
        Debug.Log($"LETTER: {dataOut["Letter"]}");

        return dataOut;
    }

    public VfxDataPoint ApplyEffects()
    {
        throw new System.NotImplementedException();

    }
}
