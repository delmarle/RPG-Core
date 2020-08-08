using System;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ScriptableHelper
{
    public static T CreateScriptableObject<T>(string path, string fileName) where T : ScriptableObject
    {
        
        var db = ScriptableObject.CreateInstance<T>();
#if UNITY_EDITOR
        if (Directory.Exists(path) == false)
        {
            Directory.CreateDirectory(path);
        }

        AssetDatabase.CreateAsset(db, path+fileName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
        return db;
    }

    public static ScriptableObject CreateScriptableObject(Type typeToLoad, string path, string fileName)
    {
        var db = ScriptableObject.CreateInstance(typeToLoad);
#if UNITY_EDITOR
        if (Directory.Exists(path) == false)
        {
            Directory.CreateDirectory(path);
        }

        AssetDatabase.CreateAsset(db, path+fileName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
        return db;
    }

    public static void RenameScriptAbleAsset(ScriptableObject so, string newPath)
    {
#if UNITY_EDITOR
        string assetPath =  AssetDatabase.GetAssetPath(so.GetInstanceID());
        AssetDatabase.RenameAsset(assetPath, newPath);
        AssetDatabase.SaveAssets();
#endif
    }


}
