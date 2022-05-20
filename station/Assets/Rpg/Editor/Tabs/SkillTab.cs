
using RPG.Editor;
using UnityEngine;

namespace Station
{
    public class SkillTab
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
                toolbarOptions[0] = new GUIContent(EditorStatic.SKILLS_TAB_SKILLS,null, "");
                toolbarOptions[1] = new GUIContent(EditorStatic.SKILLS_TAB_ACTIVE_ABILITIES, null, "");
                toolbarOptions[2] = new GUIContent(EditorStatic.SKILLS_TAB_PASSIVE_ABILITIES, null, "");
       
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
                    SkillEditor.DrawContent();
                    break;
                case 1:
                    ActiveAbilityEditor.DrawContent();
                    break;
                case 2:
                    //PassiveEditor.DrawContent();
                    break;
                
            }
        }


        #endregion
    }

}

