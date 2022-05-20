using System.Collections;
using System.Collections.Generic;
using RPG.Editor;
using UnityEngine;

namespace Station
{
    public class FactionTab : MonoBehaviour
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
                toolbarOptions[0] = new GUIContent(EditorStatic.FACTION_TAB_SETTINGS,null, "");
                toolbarOptions[1] = new GUIContent(EditorStatic.FACTION_TAB_CONFIGURATION,null, "");
                toolbarOptions[2] = new GUIContent(EditorStatic.FACTION_TAB_FACTIONS, null, "");
               
                var height = 40 * toolbarOptions.Length;
                _toolBarIndex = GUILayout.SelectionGrid(_toolBarIndex, toolbarOptions,1,EditorStatic.ToolBarStyle,GUILayout.Height(height));
            }
            GUILayout.EndVertical();
        }
    
        private static void DrawProperties()
        {
            switch (_toolBarIndex)
            {
                case 0:
                    FactionSettingsEditor.DrawSettings();
                    break;
                case 1:
                    FactionSettingsEditor.DrawContent();
                    break;
                case 2:
                    FactionEditor.DrawContent();
                    break;
                
            }
        }

        #endregion
    }

}

