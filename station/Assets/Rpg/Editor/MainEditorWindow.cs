using RPG.Editor;
using Station.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
  public class MainEditorWindow : EditorWindow
  {
    int _toolBarIndex;
    
    [MenuItem("Tools/Core RPG")]
    private static void ShowWindow()
    {
      EditorStatic.LoadStyles();
      MainEditorWindow myWindow = GetWindow<MainEditorWindow>("Core RPG");
      myWindow.minSize = new Vector2(1024, 600);
      myWindow.maxSize = myWindow.minSize*2;
      myWindow.titleContent = new GUIContent("Core RPG");
      myWindow.Show();
    }

    private void OnDestroy()
    {
      Debug.Log("saving all items entries");
      ItemsEditor.SaveAllItems();
    }

    private void OnGUI()
    {
      if (EditorApplication.isCompiling) return;
      if (EditorStatic.PublisherNameStyle == null)
      {
        EditorStatic.LoadStyles();
        return;
      }

      
      DrawHeader();
      DrawBody();
      DrawFooter();
    }

    private void DrawHeader()
    { 
      EditorStatic.Space();
      
      GUILayout.Label(new GUIContent(EditorStatic.WINDOW_TITLE, EditorStatic.GetEditorTexture("core_small")), EditorStatic.PublisherNameStyle);
      EditorStatic.Space();
      _toolBarIndex = GUILayout.Toolbar(_toolBarIndex, EditorStatic.ToolbarOptions, EditorStatic.ToolBarStyle,
        EditorStatic.ToolbarHeight);
    }

    private void DrawBody()
    {
      switch (_toolBarIndex)
      {
        case 0:
          GameConfigTab.DrawTab();
          break;
        case 1:
          StatsTab.DrawTab();
          break;
        case 2:
          CharactersTab.DrawTab();
          break;
        case 3:
          FactionTab.DrawTab();
          break;
        case 4:
          //ProgressionTab.DrawTab();
          break;
        case 5:
          SkillTab.DrawTab();
          break;
        case 6:
          ItemsEditor.DrawTab();
          break;
        case 7:
          WorldTab.DrawTab();
          break;
        case 8:
          InteractionTab.DrawTab();
          break;
      }
    }

    private void DrawFooter()
    {
      EditorGUILayout.LabelField(new GUIContent("version 0.6"), EditorStatic.CenteredVersionLabel);
    }
  }
}