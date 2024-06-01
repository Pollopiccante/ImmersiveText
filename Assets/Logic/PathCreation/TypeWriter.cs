using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TypeWriter
{
    // This class abstracts the process of setting text parts onto a give line (mesh text path)
    // It is handled in the form of a strategy pattern
    // It also provides some utility to help during mesh text path creation

    public ApplyAnimationStrategy animationStrategy = new DefaultApplyBuildAnimation();

    
    public abstract EatResult Eat(MeshTextPath path, string text);
    public abstract float GetDistanceBetweenIndex(string inputString, int fromIndex, int toIndex);

    public abstract float floatGetHeightOfWord(string inputString);

    public float GetDistanceBetween(string inputString)
    {
        if (inputString == "")
            return 0;
        return GetDistanceBetweenIndex(inputString, 0, inputString.Length - 1);
    }

}
