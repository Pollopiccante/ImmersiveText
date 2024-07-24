using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Test : MonoBehaviour
{
    [MenuItem("Mapping/Test")]
    static public void tests()
    {
        Mesh cube = Resources.Load<Mesh>("Meshes/MyCube");
        // input: annotated text
        CompleteAnnotation<DefaultTextDimensionsDataPoint> completeAnnotation = new CompleteAnnotation<DefaultTextDimensionsDataPoint>();

        // letters
        GenericCompoundSection letterSection = ScriptableObject.CreateInstance<GenericCompoundSection>();
        List<GenericSection> letterSectionSubsections = new List<GenericSection>();
        GenericEnumerableSection helloSection = ScriptableObject.CreateInstance<GenericEnumerableSection>();
        helloSection.values = ValueWrapper.FromString(String.Concat(Enumerable.Repeat("Hello", 400)));
        letterSectionSubsections.Add(helloSection);
        letterSection.values = ValueWrapper.FromGenericSections(letterSectionSubsections);
        completeAnnotation.Import<LetterEffectDimension>(letterSection);
        
        // humor
        GenericCompoundSection humorSection = ScriptableObject.CreateInstance<GenericCompoundSection>();
        List<GenericSection> humorSectionSubsections = new List<GenericSection>();
        GenericIdentitySection funnySection = ScriptableObject.CreateInstance<GenericIdentitySection>();
        ValueWrapper floatVW = new ValueWrapper();
        floatVW.floatValue = 2f;
        funnySection.value = floatVW;
        funnySection.length = 500;
        humorSectionSubsections.Add(funnySection);
        humorSection.values = ValueWrapper.FromGenericSections(humorSectionSubsections);
        completeAnnotation.Import<HumorDimension>(humorSection);
        
        // path
        GenericCompoundSection pathSection = ScriptableObject.CreateInstance<GenericCompoundSection>();
        List<GenericSection> pathSectionSubsections = new List<GenericSection>();
        GenericIdentitySection cubeSection = ScriptableObject.CreateInstance<GenericIdentitySection>();
        ValueWrapper cubeVW = new ValueWrapper();
        MeshPathStrategy cubeStrat = ScriptableObject.CreateInstance<MeshPathStrategy>();
        cubeStrat.mesh = cube;
        cubeVW.meshPathStrategyValue = cubeStrat;
        cubeSection.value = cubeVW;
        cubeSection.length = 1000;
        pathSectionSubsections.Add(cubeSection);
        pathSection.values = ValueWrapper.FromGenericSections(pathSectionSubsections);
        completeAnnotation.Import<SubPathEffectDimension>(pathSection);

        GenericCompoundSection meshSection = ScriptableObject.CreateInstance<GenericCompoundSection>();
        GenericCompoundSection angrySection = ScriptableObject.CreateInstance<GenericCompoundSection>();
        
        // completeAnnotation.Import<SubPathEffectDimension>(meshSection);
        completeAnnotation.Import<AngryDimension>(angrySection);

        Path testPath = new Path(new[] {new Vector3(0, 0, 0), new Vector3(3000, 0, 0)});
        
        // create effect
        VFXUtil.CreateEffectFromCompleteAnnotation(completeAnnotation, new DefaultMapping(), testPath);
    }
}
