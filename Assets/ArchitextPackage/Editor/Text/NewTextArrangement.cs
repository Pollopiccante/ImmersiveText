using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class StringSectionMapping
{
    public string sectionName;
    public GenericSection Section;
}

[CreateAssetMenu(fileName = "newTA", menuName = "ScriptableObjects/NewTextArrangement", order = 1)]
public class NewTextArrangement : ScriptableObject
{
    public TextArrangementDefinition Definition;

    public TextScriptableObject text;
    public PathScriptableObject mainPath;
    public AlphabethScriptableObject alphabeth;
    
    
    public List<StringSectionMapping> genericSections;
    // public AnnotationData[] completeAnnotationData;
    
    public VFXDataScriptableObject CreateVfxEffectData()
    {
        // create mapping
        Type mapType = Type.GetType(Definition.mappingName);
        if (mapType == null)
            throw new ArgumentException($"mapping type \"{Definition.mappingName}\" was not found");
        GenericMapping mapping = (GenericMapping)Activator.CreateInstance(mapType);
        
        // create complete annotation
        // dimensions specified in object
        Dictionary<string, GenericSection> importedDimensions = new Dictionary<string, GenericSection>();
        foreach (StringSectionMapping data in genericSections)
            importedDimensions.Add(data.sectionName, data.Section);
        
        // letter dimension
        GenericEnumerableSection letterSection = CreateInstance<GenericEnumerableSection>();
        letterSection.values = ValueWrapper.FromString(TextUtil.ToSingleLine(text.GetContent()));
        importedDimensions.Add("Letter", letterSection);
        
        
        CompleteAnnotation<Dictionary<string, object>> completeTextNotation = new CompleteAnnotation<Dictionary<string, object>>(importedDimensions);


        return VFXUtil.newCreateVFX(completeTextNotation, mapping, mainPath.LoadPath(), alphabeth);
    }
    
    public void CreateVfxEffect()
    {
        VFXUtil.CreateEffectFromVFXData(CreateVfxEffectData());
    }

    public void SaveVfxData(string filename)
    {
        VFXUtil.SaveVFXData(CreateVfxEffectData(), filename);
    }
}
