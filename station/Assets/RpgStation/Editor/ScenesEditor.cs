
using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{ 
  public static class ScenesEditor
  {
    private static ScenesDb _scenesDb;
    private static Vector2 _propertyScrollPos;
    private static int _selectedIndex = 1;
    
    public static void DrawContent()
    {
      if (_scenesDb == null)
      {
        _scenesDb = (ScenesDb) EditorStatic.GetDb(typeof(ScenesDb));
      }
      else
      {
        EditorGUI.BeginChangeCheck();
        {
          DrawAttributesList();
          ListView(_selectedIndex);
        }
        if (EditorGUI.EndChangeCheck())
        {
          _scenesDb.ForceRefresh(); 
        }
      }
      
    }
    
    #region [[ DRAW ]]

    private static void DrawAttributesList()
    {
      _selectedIndex = EditorStatic.DrawGenericSelectionList(_scenesDb, _selectedIndex, _propertyScrollPos,out _propertyScrollPos,"BuildSettings.SelectedIcon",true);
    }
    #endregion
    
    private static Vector2 _scrollPos;
    private static Texture2D _selectedTexture;
    private static bool _pickingTexture;
    
    private static void ListView(int selectedEntry)
    {
      var entriesCount = _scenesDb.Count();
      if (selectedEntry == -1) return;
      if (entriesCount == 0) return;
      if (entriesCount < selectedEntry+1) selectedEntry = 0;
      
      var entry = _scenesDb.GetEntry(selectedEntry);
      GUILayout.BeginHorizontal("box");
      {
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true));
        {
          DrawPanel(entry,selectedEntry);
        }
        EditorGUILayout.EndScrollView();
      }
      GUILayout.EndHorizontal();
    }

    private static void DrawPanel(Scene scene,int selectedEntry)
    {
      GUILayout.Label("EDIT Scene:",GUILayout.Width(90));
      EditorStatic.DrawLargeLine(5);
      GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
      {
        if (scene.Icon)
          _selectedTexture = scene.Icon.texture;
        else
          _selectedTexture = null;

        if (GUILayout.Button(_selectedTexture, GUILayout.Width(EditorStatic.EDITOR_BUTTON_SIZE),GUILayout.Height(EditorStatic.EDITOR_BUTTON_SIZE)))
        {
          int controllerId = GUIUtility.GetControlID(FocusType.Passive);
          EditorGUIUtility.ShowObjectPicker<Sprite>(null,false,null,controllerId);
          _pickingTexture = true;
        }
        string commandName = Event.current.commandName;
        if (_pickingTexture && commandName == EditorStatic.EDITOR_OBJECT_PICKER_COMMAND_NAME)
        {
          _scenesDb.GetEntry(selectedEntry).Icon = EditorGUIUtility.GetObjectPickerObject() as Sprite;
          _pickingTexture = false;
        }
        
        GUILayout.BeginVertical();
        {
          GUILayout.Space(5);
          GUILayout.BeginHorizontal();
          {
            GUILayout.Label("Name",GUILayout.Width(70));
            scene.VisualName = GUILayout.TextField(scene.VisualName);
          }
          GUILayout.EndHorizontal();
          
          GUILayout.Space(5);
          GUILayout.BeginHorizontal();
          {
            GUILayout.Label("Reference",GUILayout.Width(70));
            if (scene.Reference == null)
            {
              scene.Reference = new SceneReference();
            }

            scene.Reference.SceneAsset = EditorGUILayout.ObjectField( scene.Reference.SceneAsset,typeof(SceneAsset), true);
          }
          GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        if (EditorStatic.ButtonDelete())
        {
          if (EditorUtility.DisplayDialog("Delete confirmation",
            "Do you want to delete the scene?","Delete","Cancel"))
          {
            _scenesDb.Remove(scene);
            EditorStatic.ResetFocus();
            return;
          }
        }
      }
      GUILayout.EndHorizontal();
      BuildUtils.BuildScene buildScene = BuildUtils.GetBuildScene(scene.Reference.SceneAsset);
      DrawSceneInfoGui(buildScene);

    }
    
   #region [[HELPER]]
     private static void DrawSceneInfoGui( BuildUtils.BuildScene buildScene)
  {
    EditorStatic.DrawThinLine(10);
    bool readOnly = BuildUtils.IsReadOnly();
    string readOnlyWarning = readOnly ? 
      "\n\nWARNING: Build Settings is not checked out and so cannot be modified." : "";
    string textToolTip;
    GUILayout.BeginHorizontal();
    {
      Texture2D iconStatus;
      string textStatus;
      
      // Missing from build scenes
      if (buildScene.BuildIndex == -1)
      {
        iconStatus = EditorGUIUtility.FindTexture("d_winbtn_mac_close");
        textStatus = "Not in build";
        textToolTip = "This scene is NOT in build settings.\nIt will be NOT included in builds.";
      }
      // In build scenes and enabled
      else if (buildScene.Scene.enabled)
      {
        iconStatus = EditorGUIUtility.FindTexture("d_winbtn_mac_max");
        textStatus = "BuildIndex: " + buildScene.BuildIndex;
        textToolTip = "This scene is in build settings and ENABLED.\nIt will be included in builds."+readOnlyWarning;
      }
      // In build scenes and disabled
      else
      {
        iconStatus = EditorGUIUtility.FindTexture("d_winbtn_mac_min");
        textStatus = "BuildIndex: " + buildScene.BuildIndex;
        textToolTip = "This scene is in build settings and DISABLED.\nIt will be NOT included in builds.";
      }
      GUILayout.Label(new GUIContent(textStatus,iconStatus));
      if (buildScene.BuildIndex == -1)
      {
        if (EditorStatic.ButtonPressed("Add (buildIndex " +EditorBuildSettings.scenes.Length + ")", Color.white))
        {
          BuildUtils.AddBuildScene(buildScene);
        }
      }
      else
      {
        bool isEnabled = buildScene.Scene.enabled;
        string stateString = isEnabled ? "Disable" : "Enable";
        if (EditorStatic.ButtonPressed( stateString + " In Build", Color.white))
        {
          BuildUtils.SetBuildSceneState(buildScene, !isEnabled);
        }
        if (EditorStatic.ButtonPressed( "Remove from Build", Color.white))
        {
          BuildUtils.RemoveBuildScene(buildScene);
        }
      }
    }
    
    GUILayout.EndHorizontal();
    GUILayout.Space(5);
    EditorGUILayout.HelpBox(textToolTip, MessageType.Info);
    GUILayout.Space(5);
    if (EditorStatic.ButtonPressed( "Build Settings", Color.white))
    {
      BuildUtils.OpenBuildSettings();
    }
  }
    #endregion
  }
    
  
}
