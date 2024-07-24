using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "DirConfiguration", menuName = "ScriptableObjects/DirConfiguration", order = 1)]
public class DirConfiguration : ScriptableObject
{
    private static string _dirConfigPath = "Assets/Config/DirConfig.asset";
    private static DirConfiguration _instance;

    public static DirConfiguration Instance
    {
        get{
            if (_instance == null)
            {
                _instance = AssetDatabase.LoadAssetAtPath<DirConfiguration>(_dirConfigPath);
                if (_instance == null)
                {
                    DirConfiguration dirConfiguration = CreateInstance<DirConfiguration>();
                    AssetDatabase.CreateAsset(dirConfiguration, _dirConfigPath);
                }
            }
            return _instance;
        }
    }
    
    public string pathScriptableObjectDir = "Assets/EffectLogicParts/NewPathLogic/PathObjects/";
    public string vfxDataScriptableObjectDir = "Assets/EffectLogicParts/VFXData/VFXDataObjects/";

    public static string GetPCacheFileNamingTemplate()
    {
        return "{0}" + "_{1}_" + DateTime.Now.ToString("yyyymmdd") + ".pcache";
    }
}


static class MySingletonMenuItems
{
    [MenuItem("DirConfig/Initialize")]
    static void LoadDirConfig()
    {
        DirConfiguration test = DirConfiguration.Instance;
    }
}