using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif
namespace Station
{
  [System.Serializable]
public class SceneReference : ISerializationCallbackReceiver
{
#if UNITY_EDITOR
  // What we use in editor to select the scene
  public Object SceneAsset;
  bool IsValidSceneAsset
  {
    get
    {
      if (SceneAsset == null)
        return false;
      return SceneAsset.GetType().Equals(typeof(SceneAsset));
    }
  }
#endif

  // This should only ever be set during serialization/deserialization!
  [SerializeField] private string _scenePath = string.Empty;

  // Use this when you want to actually have the scene path
  public string ScenePath
  {
    get
    {
#if UNITY_EDITOR
      // In editor we always use the asset's path
      return GetScenePathFromAsset();
#else
// At runtime we rely on the stored path value which we assume was serialized correctly at build time.
// See OnBeforeSerialize and OnAfterDeserialize
            return _scenePath;
#endif
    }
    set
    {
      _scenePath = value;
#if UNITY_EDITOR
      SceneAsset = GetSceneAssetFromPath();
#endif
    }
  }

  public static implicit operator string(SceneReference sceneReference)
  {
    return sceneReference.ScenePath;
  }

  // Called to prepare this data for serialization. Stubbed out when not in editor.
  public void OnBeforeSerialize()
  {
#if UNITY_EDITOR
    HandleBeforeSerialize();
#endif
  }

  // Called to set up data for deserialization. Stubbed out when not in editor.
  public void OnAfterDeserialize()
  {
#if UNITY_EDITOR
    // We sadly cannot touch assetdatabase during serialization, so defer by a bit.
    EditorApplication.update += HandleAfterDeserialize;
#endif
  }


#if UNITY_EDITOR
  private SceneAsset GetSceneAssetFromPath()
  {
    if (string.IsNullOrEmpty(_scenePath))
      return null;
    return AssetDatabase.LoadAssetAtPath<SceneAsset>(_scenePath);
  }

  private string GetScenePathFromAsset()
  {
    if (SceneAsset == null)
      return string.Empty;
    return SceneAsset.name;
  }

  private void HandleBeforeSerialize()
  {
    // Asset is invalid but have Path to try and recover from
    if (IsValidSceneAsset == false && string.IsNullOrEmpty(_scenePath) == false)
    {
      SceneAsset = GetSceneAssetFromPath();
      if (SceneAsset == null)
        _scenePath = string.Empty;

      UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
    }
    // Asset takes precendence and overwrites Path
    else
    {
      _scenePath = GetScenePathFromAsset();
    }
  }

  private void HandleAfterDeserialize()
  {
    EditorApplication.update -= HandleAfterDeserialize;
    // Asset is valid, don't do anything - Path will always be set based on it when it matters
    if (IsValidSceneAsset)
      return;

    // Asset is invalid but have path to try and recover from
    if (string.IsNullOrEmpty(_scenePath) == false)
    {
      SceneAsset = GetSceneAssetFromPath();
      // No asset found, path was invalid. Make sure we don't carry over the old invalid path
      if (SceneAsset == null)
        _scenePath = string.Empty;

      if (Application.isPlaying == false)
        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
    }
  }
#endif
}

#if UNITY_EDITOR
/// <summary>
/// Display a Scene Reference object in the editor.
/// If scene is valid, provides basic buttons to interact with the scene's role in Build Settings.
/// </summary>
[CustomPropertyDrawer(typeof(SceneReference))]
public class SceneReferencePropertyDrawer : PropertyDrawer
{
  // The exact name of the asset Object variable in the SceneReference object
  const string SCENE_ASSET_PROPERTY_STRING = "SceneAsset";

  // The exact name of  the scene Path variable in the SceneReference object
  const string SCENE_PATH_PROPERTY_STRING = "_scenePath";

  static readonly RectOffset BoxPadding = EditorStyles.helpBox.padding;
  static readonly float padSize = 2f;
  static readonly float LineHeight = EditorGUIUtility.singleLineHeight;
  static readonly float PaddedLine = LineHeight + padSize;
  static readonly float footerHeight = 10f;

  /// <summary>
  /// Drawing the 'SceneReference' property
  /// </summary>
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
  {
    var sceneAssetProperty = GetSceneAssetProperty(property);

    // Draw the Box Background
    position.height -= footerHeight;
    GUI.Box(EditorGUI.IndentedRect(position), GUIContent.none, EditorStyles.helpBox);
    position = BoxPadding.Remove(position);
    position.height = LineHeight;

    // Draw the main Object field
    label.tooltip = "The actual Scene Asset reference.\nOn serialize this is also stored as the asset's path.";

    EditorGUI.BeginProperty(position, GUIContent.none, property);
    EditorGUI.BeginChangeCheck();
    int sceneControlId = GUIUtility.GetControlID(FocusType.Passive);
    var selectedObject = EditorGUI.ObjectField(position, label, sceneAssetProperty.objectReferenceValue,
      typeof(SceneAsset), false);
    BuildUtils.BuildScene buildScene = BuildUtils.GetBuildScene(selectedObject);

    if (EditorGUI.EndChangeCheck())
    {
      sceneAssetProperty.objectReferenceValue = selectedObject;

      // If no valid scene asset was selected, reset the stored path accordingly
      if (buildScene.Scene == null)
        GetScenePathProperty(property).stringValue = string.Empty;
    }

    position.y += PaddedLine;

    if (buildScene.AssetGuid.Empty() == false)
    {
      // Draw the Build Settings Info of the selected Scene
      DrawSceneInfoGUI(position, buildScene, sceneControlId + 1);
    }

    EditorGUI.EndProperty();
  }

  /// <summary>
  /// Ensure that what we draw in OnGUI always has the room it needs
  /// </summary>
  public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
  {
    int lines = 2;
    SerializedProperty sceneAssetProperty = GetSceneAssetProperty(property);
    if (sceneAssetProperty.objectReferenceValue == null)
      lines = 1;

    return BoxPadding.vertical + LineHeight * lines + padSize * (lines - 1) + footerHeight;
  }

  /// <summary>
  /// Draws info box of the provided scene
  /// </summary>
  private void DrawSceneInfoGUI(Rect position, BuildUtils.BuildScene buildScene, int sceneControlId)
  {
    bool readOnly = BuildUtils.IsReadOnly();
    string readOnlyWarning =
      readOnly ? "\n\nWARNING: Build Settings is not checked out and so cannot be modified." : "";

    // Label Prefix
    GUIContent iconContent;
    GUIContent labelContent = new GUIContent();

    // Missing from build scenes
    if (buildScene.BuildIndex == -1)
    {
      iconContent = EditorGUIUtility.IconContent("d_winbtn_mac_close");
      labelContent.text = "NOT In Build";
      labelContent.tooltip = "This scene is NOT in build settings.\nIt will be NOT included in builds.";
    }
    // In build scenes and enabled
    else if (buildScene.Scene.enabled)
    {
      iconContent = EditorGUIUtility.IconContent("d_winbtn_mac_max");
      labelContent.text = "BuildIndex: " + buildScene.BuildIndex;
      labelContent.tooltip = "This scene is in build settings and ENABLED.\nIt will be included in builds." +
                             readOnlyWarning;
    }
    // In build scenes and disabled
    else
    {
      iconContent = EditorGUIUtility.IconContent("d_winbtn_mac_min");
      labelContent.text = "BuildIndex: " + buildScene.BuildIndex;
      labelContent.tooltip = "This scene is in build settings and DISABLED.\nIt will be NOT included in builds.";
    }

    // Left status label
    using (new EditorGUI.DisabledScope(readOnly))
    {
      Rect labelRect = DrawUtils.GetLabelRect(position);
      Rect iconRect = labelRect;
      iconRect.width = iconContent.image.width + padSize;
      labelRect.width -= iconRect.width;
      labelRect.x += iconRect.width;
      EditorGUI.PrefixLabel(iconRect, sceneControlId, iconContent);
      EditorGUI.PrefixLabel(labelRect, sceneControlId, labelContent);
    }

    // Right context buttons
    Rect buttonRect = DrawUtils.GetFieldRect(position);
    buttonRect.width = (buttonRect.width) / 3;

    string tooltipMsg;
    using (new EditorGUI.DisabledScope(readOnly))
    {
      // NOT in build settings
      if (buildScene.BuildIndex == -1)
      {
        buttonRect.width *= 2;
        int addIndex = EditorBuildSettings.scenes.Length;
        tooltipMsg =
          "Add this scene to build settings. It will be appended to the end of the build scenes as buildIndex: " +
          addIndex + "." + readOnlyWarning;
        if (DrawUtils.ButtonHelper(buttonRect, "Add...", "Add (buildIndex " + addIndex + ")",
          EditorStyles.miniButtonLeft, tooltipMsg))
          BuildUtils.AddBuildScene(buildScene);
        buttonRect.width /= 2;
        buttonRect.x += buttonRect.width;
      }
      // In build settings
      else
      {
        bool isEnabled = buildScene.Scene.enabled;
        string stateString = isEnabled ? "Disable" : "Enable";
        tooltipMsg = stateString + " this scene in build settings.\n" +
                     (isEnabled ? "It will no longer be included in builds" : "It will be included in builds") + "." +
                     readOnlyWarning;

        if (DrawUtils.ButtonHelper(buttonRect, stateString, stateString + " In Build", EditorStyles.miniButtonLeft,
          tooltipMsg))
          BuildUtils.SetBuildSceneState(buildScene, !isEnabled);
        buttonRect.x += buttonRect.width;

        tooltipMsg =
          "Completely remove this scene from build settings.\nYou will need to add it again for it to be included in builds!" +
          readOnlyWarning;
        if (DrawUtils.ButtonHelper(buttonRect, "Remove...", "Remove from Build", EditorStyles.miniButtonMid,
          tooltipMsg))
          BuildUtils.RemoveBuildScene(buildScene);
      }
    }

    buttonRect.x += buttonRect.width;

    tooltipMsg = "Open the 'Build Settings' Window for managing scenes." + readOnlyWarning;
    if (DrawUtils.ButtonHelper(buttonRect, "Settings", "Build Settings", EditorStyles.miniButtonRight, tooltipMsg))
    {
      BuildUtils.OpenBuildSettings();
    }
  }

  static SerializedProperty GetSceneAssetProperty(SerializedProperty property)
  {
    return property.FindPropertyRelative(SCENE_ASSET_PROPERTY_STRING);
  }

  static SerializedProperty GetScenePathProperty(SerializedProperty property)
  {
    return property.FindPropertyRelative(SCENE_PATH_PROPERTY_STRING);
  }


}

#endif

}
