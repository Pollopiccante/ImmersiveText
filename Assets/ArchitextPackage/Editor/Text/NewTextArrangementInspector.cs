using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(NewTextArrangement), true)]
public class NewTextArrangementInspector : Editor
{
    private string filename;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        NewTextArrangement castTarget = (NewTextArrangement) target;

        // load defined dimensions from definition if they are not present
        if (castTarget.Definition != null && castTarget.genericSections.Count != castTarget.Definition.definedDimensions.Count)
        {
            LoadDefinition(castTarget.Definition);
        }
        
        if (GUILayout.Button("Create Vfx Effect"))
        {
            castTarget.CreateVfxEffect();            
        }

        GUILayout.Label("File Name:");
        filename = GUILayout.TextField(filename);
        if (GUILayout.Button("Save Vfx Effect Data"))
        {
            castTarget.SaveVfxData(filename);            
        }
        
    }

    private void LoadDefinition(TextArrangementDefinition definition)
    {
        NewTextArrangement castTarget = (NewTextArrangement) target;
        
        // remove sections not mentioned in the definition, only allow singles of each dimension
        HashSet<string> validDimNames = definition.definedDimensions.Select(dd => dd.DimensionName).ToHashSet();
        castTarget.genericSections = castTarget.genericSections.FindAll(section =>
        {
            bool allowed = validDimNames.Contains(section.sectionName);
            validDimNames.Remove(section.sectionName);
            return allowed;
        });
        
        // add sections in order
        List<string> presentSectionNames = castTarget.genericSections.Select(gs => gs.sectionName).ToList();
        definition.definedDimensions.ForEach(dim =>
        {
            if (!presentSectionNames.Contains(dim.DimensionName))
            {
                StringSectionMapping ssm = new StringSectionMapping();
                ssm.sectionName = dim.DimensionName;
                ssm.Section = null;
                castTarget.genericSections.Add(ssm);
            }
        });

    }
}
