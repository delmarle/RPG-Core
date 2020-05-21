using System.Collections.Generic;
using System.Linq;
using RPG.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Station
{ 
  public partial class SpawnPointsEditor
  {  
    private static ScenesDb _scenesDb;
    private static Vector2 _propertyScrollPos;
    private static int _selectedIndex;
    private static int _selectedSpawn;
    private static Vector2 _scrollPos;
    private static SpawnPlacer _spawnPlacer;
    
    public static void DrawContent()
    {
      if (_scenesDb == null)
      {
        _scenesDb = (ScenesDb) EditorStatic.GetDb(typeof(ScenesDb));
      
      }
      else
      {
        if (_scenesDb.Any() == false)
        {
          DrawEmpty();
        }
        else
        {
          EditorGUI.BeginChangeCheck();
          {
            DrawSelectionColumn();
            ListView(_selectedIndex);
          }
          if (EditorGUI.EndChangeCheck())
          {
            _scenesDb.ForceRefresh();
          }
        }
      }

    }
    
    #region [[ DRAW ]]

    private static void DrawEmpty()
    {
      GUILayout.BeginVertical("box",GUILayout.ExpandWidth(true));
      {
        EditorGUILayout.HelpBox("No scenes found ! \n You can add scenes in the previous section", MessageType.Warning);
      }
      GUILayout.EndVertical();
    }

    private static void DrawSelectionColumn()
    {
      var currentScene = _scenesDb.GetEntry(_selectedIndex);
      if (currentScene == null) return;
      if (_selectedSpawn >= currentScene.SpawnPoints.Count)
      {
        _selectedSpawn = currentScene.SpawnPoints.Count-1;
        currentScene = _scenesDb.GetEntry(_selectedIndex);
      }

      if (_selectedSpawn == -1 && currentScene.SpawnPoints.Any()) _selectedSpawn = 0;

      currentScene = _scenesDb.GetEntry(_selectedIndex);
      
      GUILayout.BeginVertical("box",GUILayout.Width(250));
      {
        GUILayout.Label("Select a scene:",GUILayout.Width(90));
        EditorGUILayout.Space();
        EditorStatic.DrawLargeLine();
        GUILayout.BeginHorizontal(EditorStyles.centeredGreyMiniLabel);
        {
          _selectedIndex = EditorGUILayout.Popup(_selectedIndex, _scenesDb.ListEntryNames());
          if (EditorStatic.SizeableButton(100, 16, "OPEN SCENE", ""))
          {
            var path = currentScene.Reference.ScenePath;
            if (string.IsNullOrEmpty(path))
            {
              Debug.LogWarning("scene not assigned");
            }
            else
            {
              EditorSceneManager.OpenScene(AssetDatabase.GetAssetPath(currentScene.Reference.SceneAsset));
            }         
          }
        }
        GUILayout.EndHorizontal();
        
      
        EditorStatic.DrawThinLine(5);
        GUILayout.Label("Select a Spawn-point:",GUILayout.Width(140));
        EditorGUILayout.Space();
        if (currentScene.SpawnPoints.Count > 0)
        {
          List<string> spawnNames = new List<string>();
          foreach (var spawn in currentScene.SpawnPoints)
          {
            spawnNames.Add(spawn.VisualName.GetValue());
          }
          _selectedSpawn = EditorGUILayout.Popup(_selectedSpawn, spawnNames.ToArray());
        }
        
        GUILayout.BeginHorizontal();
        {
          if (EditorStatic.ButtonAdd())
          {
            currentScene.SpawnPoints.Add(new SpawnPoint("spawn_"+currentScene.SpawnPoints.Count));
            _selectedSpawn = currentScene.SpawnPoints.Count - 1;
          }
        
          if (currentScene.SpawnPoints.Count > 0 && _selectedSpawn>=0)
          {
            if (EditorStatic.ButtonDelete())
            {
              currentScene.SpawnPoints.RemoveAt(_selectedSpawn);
              _selectedSpawn = currentScene.SpawnPoints.Count - 1;
              return;
            }
          }
        }
        GUILayout.EndHorizontal();
      }
      GUILayout.EndVertical();
     
    }
    #endregion

    protected static bool IsCorrectScene(string scenePath)
    {
      var scene = SceneManager.GetActiveScene();
      return scene.path == scenePath;
    }

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
          DrawPanel(entry);
        }
        EditorGUILayout.EndScrollView();
      }
      GUILayout.EndHorizontal();
    }

    private static void DrawPanel(Scene scene)
    {
      GUILayout.Label("EDIT Spawn point:",GUILayout.Width(140));
      EditorStatic.DrawLargeLine(5);
    
      if (scene.SpawnPoints.Count == 0)
      {
        EditorGUILayout.HelpBox("No spawn points found", MessageType.Info);
        return;
      }
      
      BuildUtils.BuildScene buildScene = BuildUtils.GetBuildScene(scene.Reference.SceneAsset);
     // int sceneControlId = GUIUtility.GetControlID(FocusType.Passive);
      DrawSceneInfoGui(buildScene,scene);
    }
    
   #region [[HELPER]]
   private static void DrawSceneInfoGui( BuildUtils.BuildScene buildScene,Scene sceneData)
  {
    if (sceneData.SpawnPoints.Count < _selectedSpawn) return;
    if (_selectedSpawn >= sceneData.SpawnPoints.Count) _selectedSpawn = sceneData.SpawnPoints.Count - 1;
    if (sceneData.SpawnPoints.Count <= 0) return;
    if (_selectedSpawn < 0) return;
    
    var scene = SceneManager.GetActiveScene();
    var scenePath = buildScene.AssetPath;
    var spawnPoint = sceneData.SpawnPoints[_selectedSpawn];
   
    EditorGUILayout.HelpBox("Scene Opened: "+scene.path,MessageType.Info);
    EditorStatic.DrawThinLine(10); 
EditorStatic.DrawLocalization(spawnPoint.VisualName, "Visual name");
EditorStatic.DrawLocalization(spawnPoint.Description, "Description");
EditorStatic.DrawThinLine();
spawnPoint.Icon =
  (Sprite)EditorGUILayout.ObjectField(spawnPoint.Icon,typeof(Sprite), false, GUILayout.Width(EditorStatic.EDITOR_BUTTON_SIZE *2),
    GUILayout.Height(EditorStatic.EDITOR_BUTTON_SIZE*2));
EditorStatic.DrawThinLine();
    if (IsCorrectScene(scenePath))
    {
      GUILayout.Space(5);
      
      GUILayout.BeginHorizontal();
      {
      
        if (EditorStatic.ButtonPressed( "Edit "+spawnPoint.VisualName, Color.white))
        {        
          _spawnPlacer = Object.FindObjectOfType<SpawnPlacer>();
          if (_spawnPlacer == null)
          {
            var asset = (SpawnPlacer)AssetDatabase.LoadAssetAtPath(@"Assets/RpgStation/Prefabs/SpawnPlacer.prefab", typeof(SpawnPlacer));
            if (asset == null)
            {
              Debug.LogError("cannot find Assets/RpgStation/Prefabs/SpawnPlacer.prefab");
              return;
            }

            _spawnPlacer = Object.Instantiate(asset);
          }
          _spawnPlacer.Initialize();
          if (SceneView.lastActiveSceneView)
          {
            _spawnPlacer.transform.position = SceneView.lastActiveSceneView.pivot+Vector3.up*1.2f;
          }

          Selection.activeGameObject = _spawnPlacer.gameObject;
        }

        if (_spawnPlacer)
        {
          if (EditorStatic.ButtonPressed("save ("+_spawnPlacer.GetPointsCount()+") points", Color.white))
          {
            spawnPoint.Positions = _spawnPlacer.Positions();
            spawnPoint.Direction = _spawnPlacer.gameObject.transform.rotation.eulerAngles;
            _spawnPlacer.Clean();
            _scenesDb.ForceRefresh();
          }
        }
      }
    
      GUILayout.EndHorizontal();
      GUILayout.Space(5);
      GUILayout.BeginVertical(EditorStyles.helpBox);
      {

        GUILayout.Label("Direction: " + spawnPoint.Direction);
        if (spawnPoint.Positions != null)
        {
          int i = 0;
          foreach (var p in spawnPoint.Positions)
          {
            var text = string.Format("[Point {0}]: x={1}, y={2}, z={3}",i,Mathf.Round(p.x),Mathf.Round(p.y),Mathf.Round(p.z));
            GUILayout.Label(text);
            i++;
          }
        }
      }
      GUILayout.EndVertical();
    }
    else
    {
      EditorGUILayout.HelpBox("Open the scene "+scenePath+" to edit spawn points", MessageType.Warning);
      if (EditorStatic.ButtonPressed( "open scene", Color.white))
      {
        EditorSceneManager.OpenScene(scenePath);
      }
    }

    
    
  }
    #endregion

  }
}
