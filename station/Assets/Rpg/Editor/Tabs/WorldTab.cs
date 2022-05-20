using RPG.Editor;
using UnityEngine;

namespace Station
{
  public class WorldTab
  {
    #region [[FIELDS]]
    private static int _toolBarIndex;
    private string _categoryName;
    private object _selecteditem;
    #endregion
    
    public static void DrawTab()
    {
      GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
      {
        DrawLeftBar();
        DrawProperties();
      }
      GUILayout.EndHorizontal();
    }


    #region [[ DRAW SECTIONS ]]
    private static void DrawLeftBar()
    {
      GUILayout.BeginVertical("box",GUILayout.Width(EditorStatic.LIST_VIEW_WIDTH),GUILayout.ExpandHeight(true));
      {
        var  toolbarOptions = new GUIContent[3];
        toolbarOptions[0] = new GUIContent(EditorStatic.WORLD_TAB_CONFIGURATION,null, "");
        toolbarOptions[1] = new GUIContent(EditorStatic.WORLD_TAB_SCENES, null, "");
        toolbarOptions[2] = new GUIContent(EditorStatic.WORLD_TAB_SPAWN_POINTS, null, "");
        var previousIndex = _toolBarIndex;
        var height = 40 * toolbarOptions.Length;
        _toolBarIndex = GUILayout.SelectionGrid(_toolBarIndex, toolbarOptions,1,EditorStatic.ToolBarStyle,GUILayout.Height(height));
        if(_toolBarIndex != previousIndex)EditorStatic.ResetFocus();
      }
      GUILayout.EndVertical();
    }
    
    private static void DrawProperties()
    {
      switch (_toolBarIndex)
      {
        case 0:
          //TODO
          break;
        case 1:
          ScenesEditor.DrawContent();
          break;
        case 2:
          SpawnPointsEditor.DrawContent();
          break;
                
      }
    }


    #endregion
  }
}

