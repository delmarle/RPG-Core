using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
    public static class UiNotificationChannelsEditor 
    {
         private static UiNotificationChannelsDb _db;

        private static Vector2 _propertyScrollPos;
        private static int _selectedIndex = 1;
        private static Vector2 _scrollPos;

        public static void DrawContent()
        {
            EditorGUI.BeginChangeCheck();
            {
                Draw();
            }
            if (EditorGUI.EndChangeCheck())
            {
                if (_db != null)
                {
                    _db.ForceRefresh(); 
                }
            }
        }
    
        #region [[ DRAW ]]

        private static void Draw()
        {
            if (_db == null)
            {
                _db = (UiNotificationChannelsDb)EditorStatic.GetDb(typeof(UiNotificationChannelsDb));
                EditorGUILayout.HelpBox("Db is missing", MessageType.Warning);
            }
            else
            {
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
                {
                    DrawBar();
                     ListView(_selectedIndex);
                }
                GUILayout.EndHorizontal();
            }

           
        }

        private static void DrawBar()
        {
            _selectedIndex = EditorStatic.DrawGenericSelectionList(_db, _selectedIndex, _propertyScrollPos,out _propertyScrollPos,"bullet_yellow",false);
        }
        
        private static void ListView(int selected)
        {
            var raceCount = _db.Count();
            if (selected == -1) return;
            if (raceCount == 0) return;
            if (raceCount < selected+1) selected = 0;
      
            var entry = _db.GetEntry(selected);
            GUILayout.BeginHorizontal("box");
            {
                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true));
                {
                    DrawPanel(entry,selected);
                }
                EditorGUILayout.EndScrollView();
            }
            GUILayout.EndHorizontal();
        }
        
          private static void DrawPanel(UiChannelModel staticData,int selected)
    {
      GUILayout.Label("EDIT Channel:",GUILayout.Width(75));
      EditorStatic.DrawLargeLine(5);
      GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
      {
 
        
        GUILayout.BeginVertical();
        {
          GUILayout.Space(5);
 
          
            GUILayout.Label("Name: ",GUILayout.Width(70));
            staticData.Name = GUILayout.TextField(staticData.Name);
          

        }
        GUILayout.EndVertical();
        if (EditorStatic.ButtonDelete())
        {
          if (EditorUtility.DisplayDialog("Delete data?",
            "Do you want to delete: "+staticData.Name,"Delete","Cancel"))
          {
            _db.Remove(staticData);
            EditorStatic.ResetFocus();
            return;
          }
        }
      }
      GUILayout.EndHorizontal();
      
      GUILayout.Space(3);
      EditorStatic.DrawSectionTitle(28, "Elements");
      EditorGUILayout.HelpBox("Elements sets as widget require the channel to be set on the prefab", MessageType.Info);
      EditorStatic.DrawThinLine(4);
      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.HelpBox("Elements in this list will be instanced and persisted between scenes" +
                              "\n make sure they are unique. ", MessageType.Info);
      if (EditorStatic.SizeableButton(250, 32, "Add new element", "plus"))
      {
          staticData.Elements.Add(null);
      }
      EditorGUILayout.EndHorizontal();
      for (int i = 0; i < staticData.Elements.Count; i++)
      {
          EditorGUILayout.BeginHorizontal();
          staticData.Elements[i] = (UiNotificationElement)EditorGUILayout.ObjectField(staticData.Elements[i], typeof(UiNotificationElement), false);
          if (EditorStatic.SizeableButton(100, 20, "delete", "cross"))
          {
              staticData.Elements.Remove(staticData.Elements[i]);
              GUIUtility.ExitGUI();
          }
          EditorGUILayout.EndHorizontal();
      }

    }
        #endregion
    }

}

