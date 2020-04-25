using UnityEngine;

public static class PathUtils
{
    private const string EDITOR_SOUNDS_PATH = "Assets/Content/Sounds/";
    private const string EDITOR_ITEMS_PATH = "Assets/Content/Items/";
    
    public static string GetStreamingAssetsPath()
    {
        #if UNITY_STANDALONE_OSX && !UNITY_EDITOR
        return  Application.dataPath + "/Resources/Data/StreamingAssets";
        #endif
        return  Application.dataPath + "/StreamingAssets";
    }

    public static string SavePath()
    {
        return Application.persistentDataPath + "/GameSave/";
    }
    
    public static void CheckContentFolder()
    {
       // if (Directory.Exists(CONTENT_FOLDER) == false)
         //   Directory.CreateDirectory(CONTENT_FOLDER);
        //if (Directory.Exists(DATABASES_PATH) == false)
          //  Directory.CreateDirectory(DATABASES_PATH);
    }

    public static string BuildSoundPath(string category)
    {
        return EDITOR_SOUNDS_PATH + "/" + category+"/"; 
    }

    public static string BuildItemPath()
    {
        return EDITOR_ITEMS_PATH;
    }
}
