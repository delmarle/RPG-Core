using Station;
using UnityEngine;

namespace RPG.Editor
{
  public static class GameConfigTab
  {
    #region [[FIELDS]]
    private static int _toolBarIndex;
    private static string _categoryName;
    private static Vector2 _propertyScrollPos;
    private static object _selectedItem;
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
        toolbarOptions[0] = new GUIContent(RpgEditorStatic.CONFIG_TAB_GAMEPLAY,null, "");
        toolbarOptions[1] = new GUIContent(RpgEditorStatic.CONFIG_TAB_CHARACTER_CREATION,null, "");
        toolbarOptions[2] = new GUIContent(RpgEditorStatic.CONFIG_TAB_DIFFICULTY, null, "");
         
        var height = 40 * toolbarOptions.Length;
        _toolBarIndex = GUILayout.SelectionGrid(_toolBarIndex, toolbarOptions,1,EditorStatic.ToolBarStyle,GUILayout.Height(height));
      }
      GUILayout.EndVertical();
    }
    
    private static void DrawProperties()
    {
      _propertyScrollPos = GUILayout.BeginScrollView(_propertyScrollPos,"box",GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true));
      {
        
        switch (_toolBarIndex)
        {
          case 0:
            GamePlayEditor.Draw();
            break;
          case 1:
            CharacterCreationEditor.Draw();
            break;
        }
        
      }
      GUILayout.EndScrollView();
    }
    #endregion
  }
}

