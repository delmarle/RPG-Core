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
        var  toolbarOptions = new GUIContent[8];
        toolbarOptions[0] = new GUIContent(EditorStatic.CONFIG_TAB_GAMEPLAY,null, "");
        toolbarOptions[1] = new GUIContent(EditorStatic.CONFIG_TAB_CHARACTER_CREATION,null, "");
        toolbarOptions[2] = new GUIContent(EditorStatic.CONFIG_TAB_DIFFICULTY, null, "");
        toolbarOptions[3] = new GUIContent(EditorStatic.CONFIG_TAB_OPTIONS, null, "");
        toolbarOptions[4] = new GUIContent(EditorStatic.CONFIG_TAB_PLATFORMS, null, "");
        toolbarOptions[5] = new GUIContent(EditorStatic.CONFIG_TAB_INPUT_EVENTS, null, "");
        toolbarOptions[6] = new GUIContent(EditorStatic.CONFIG_TAB_FLOATING_POPUPS, null, "");
        toolbarOptions[7] = new GUIContent(EditorStatic.CONFIG_TAB_UI_CHANNELS, null, "");
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
          case 2:

            break;
          case 3:

            break;
          case 4:
          //  PlatformsConfigEditor.DrawContent();
            break;
          case 5:
           // InputEventEditor.DrawContent();
            break;
          case 6:
              FloatingPopupEditor.DrawContent();
            break;
          case 7:
            UiNotificationChannelsEditor.DrawContent();
            break;
        }
        
      }
      GUILayout.EndScrollView();
    }
    #endregion
  }
}

