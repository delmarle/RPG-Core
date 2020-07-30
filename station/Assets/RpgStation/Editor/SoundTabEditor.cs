using System;
using RPG.Editor;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Station
{
    public static class SoundTabEditor
    {
        
        private static SoundsDb _db;

        private static int _selectedIndexGroup;
        private static Vector2 _propertyScrollPosLeftList;

       // private static int _selectedIndex = 1;
        private static Vector2 _scrollPos;
        private static string NewGroupName;
        
        public static void DrawContent()
        {
            EditorGUI.BeginChangeCheck();
            {
                GUILayout.BeginHorizontal();
                Draw();
                GUILayout.EndHorizontal();
            }
            if (EditorGUI.EndChangeCheck())
            {
                if (_db != null)
                {
                    _db.ForceRefresh(); 
                }
            }
        }

        private static void Draw()
        {
            if (_db == null)
            {
                _db = (SoundsDb)EditorStatic.GetDb(typeof(SoundsDb));
                EditorGUILayout.HelpBox("Db is missing", MessageType.Warning);
            }
            else
            {
                AddressableAssetSettings settings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>(EditorStatic.EDITOR_ADDRESSABLE_PATH);
                if (settings == null)
                {
                    Debug.LogError("cant find db at: ");
                }

                GUILayout.BeginVertical("box",GUILayout.Width(150), GUILayout.ExpandHeight(true));
                {
                    EditorGUILayout.LabelField("groups: ");
                    DrawSelectionList();

                }
                GUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                EditorStatic.DrawSectionTitle(32,"add a new group: ");
                NewGroupName = EditorGUILayout.TextField(NewGroupName);
                if (EditorStatic.SizeableButton(120,32, "Add", "plus"))
                {
                    if (string.IsNullOrEmpty(NewGroupName))
                    {
                        return;
                    }

                    foreach (var addGroup in settings.groups)
                    {
                        if (addGroup.Name == NewGroupName )
                        {

                            return;
                        }
                    }
                    var newGroup = settings.CreateGroup(NewGroupName, false, false, false, null);
                    _db.Add(new SoundCategory{CategoryName = NewGroupName});
                    NewGroupName = String.Empty;
                }
                
                EditorStatic.DrawLargeLine(5);
                foreach (var group in _db)
                {
                    EditorGUILayout.LabelField(group.CategoryName);
                    if (EditorStatic.ButtonDelete())
                    {
                        var template = settings.FindGroup(group.CategoryName);
                        if (template)
                        {
                            settings.RemoveGroup(template);
                        }
                        _db.Remove(group);
                          
                    }

                    EditorStatic.DrawThinLine();
                }

EditorGUILayout.EndVertical();
            }

        }
        
        public static void DrawSelectionList()
        {

                  var entries = _db.ListEntryNames();
      GUILayout.BeginVertical(GUILayout.Width(EditorStatic.LIST_VIEW_WIDTH),GUILayout.ExpandHeight(true));
      {
          _propertyScrollPosLeftList = GUILayout.BeginScrollView(_propertyScrollPosLeftList,GUILayout.ExpandWidth(true),GUILayout.ExpandHeight(true));
        {
          GUILayout.Label("SELECT "+_db.ObjectName()+": " ,GUILayout.Width(EditorStatic.LIST_VIEW_WIDTH -30));
          var  toolbarOptions = new GUIContent[_db.Count()];
          for (int i = 0; i < _db.Count(); i++)
          {
   

            toolbarOptions[i] = new GUIContent(entries[i],null, "");
          }
         
          var previousIndex = _selectedIndexGroup;

          float lElemH2 = toolbarOptions.Length * 40;
          _selectedIndexGroup = GUILayout.SelectionGrid(_selectedIndexGroup, toolbarOptions,1,EditorStatic.VerticalBarStyle, GUILayout.Height(lElemH2), GUILayout.Width(EditorStatic.LIST_VIEW_WIDTH - 10));
          if(previousIndex != _selectedIndexGroup)EditorStatic.ResetFocus();
        }
        GUILayout.EndScrollView();

      }
      GUILayout.EndVertical();
        }
    }
}