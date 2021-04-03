using System.Collections.Generic;
using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
    public static class UiNotificationChannelsEditor 
    {
         private static UiNotificationChannelsDb _db;
         private static bool _isEditingChannelName;
         private static string _editedChannelName;
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
          if (staticData.Identifier == null)
          {
              _editedChannelName = GUILayout.TextField(_editedChannelName);
              if (EditorStatic.SizeableButton(150, 32, "Create asset", "cog")&& string.IsNullOrEmpty(_editedChannelName) == false)
              {
                  string name = _editedChannelName.Contains(".asset") ? _editedChannelName : _editedChannelName + ".asset";
                  staticData.Identifier = (ScriptableNotificationChannel)ScriptableHelper.CreateScriptableObject(typeof(ScriptableNotificationChannel), EditorStatic.EDITOR_CHANNEL_ASSETS_PATH, name);
                  _editedChannelName = string.Empty;
              }
          }
          else
          {
              if (_isEditingChannelName)
              {
                  _editedChannelName = GUILayout.TextField(_editedChannelName);
                  if (EditorStatic.SizeableButton(150, 32, "Save", ""))
                  {
                      staticData.Identifier.name = _editedChannelName;
                      ScriptableHelper.RenameScriptAbleAsset( staticData.Identifier, _editedChannelName);
                      _editedChannelName = string.Empty;
                      _isEditingChannelName = false;
                  }
              }
              else
              {
                  GUILayout.Label(staticData.Identifier.name, EditorStatic.SectionTitle);
                  if (EditorStatic.SizeableButton(150, 32, "Edit", ""))
                  {
                      staticData.Identifier.name = _editedChannelName;
                      _isEditingChannelName = true;
                  }
              }
          }

        }
        GUILayout.EndVertical();
        if (EditorStatic.ButtonDelete())
        {
          if (EditorUtility.DisplayDialog("Delete data?",
            "Do you want to delete: "+staticData.Identifier,"Delete","Cancel"))
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

          public static void DrawNotificationList(ref List<ScriptableNotificationChannel> list, string listName)
          {
              EditorGUILayout.BeginVertical("box");
              EditorStatic.DrawSectionTitle(28, $"Notification: {listName}");

              for (int i = 0; i < list.Count; i++)
              {
                 
                  EditorGUILayout.BeginHorizontal();
                  list[i] = (ScriptableNotificationChannel)EditorGUILayout.ObjectField(list[i], typeof(ScriptableNotificationChannel), false);
                  if (EditorStatic.SizeableButton(32,18, "X", ""))
                  {
                      list.RemoveAt(i);
                      GUIUtility.ExitGUI();
                  }
                  EditorGUILayout.EndHorizontal();
              }


              if (EditorStatic.ButtonAdd())
              {
                  list.Add(null);
              }
              EditorGUILayout.EndVertical();
          }

          #endregion
    }
}

