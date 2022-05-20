using System.Collections;
using System.Collections.Generic;
using RPG.Editor;
using UnityEngine;

namespace Station.Editor
{
    public static class CharactersTab
    {
        private static int _toolBarIndex;
        private static Vector2 _propertyScrollPos;
        
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
                var  toolbarOptions = new GUIContent[4];
                toolbarOptions[0] = new GUIContent(EditorStatic.CHARACTERS_TAB_RACES,null, "");
                toolbarOptions[1] = new GUIContent(EditorStatic.CHARACTERS_TAB_PLAYER_CLASSES,null, "");
                toolbarOptions[2] = new GUIContent(EditorStatic.CHARACTERS_TAB_NPC, null, "");
                toolbarOptions[3] = new GUIContent(EditorStatic.CHARACTERS_TAB_PETS, null, "");
                
                var height = 40 * toolbarOptions.Length;
                _toolBarIndex = GUILayout.SelectionGrid(_toolBarIndex, toolbarOptions,1,EditorStatic.ToolBarStyle,GUILayout.Height(height));
            }
            GUILayout.EndVertical();
        }
    
        private static void DrawProperties()
        {
            _propertyScrollPos = GUILayout.BeginScrollView(_propertyScrollPos,GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true));
            {
        
                switch (_toolBarIndex)
                {
                    case 0:
                        RaceEditor.Draw();
                        break;
                    case 1:
                        PlayerClassEditor.Draw();
                        break;
                    case 2:
                        NpcEditor.Draw();
                        break;
                    case 3:

                        break;
                }
            }
            GUILayout.EndScrollView();
        }
        #endregion
    }
}
