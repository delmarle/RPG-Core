using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Editor;
using UnityEditor;
using UnityEngine;


namespace Station
{
     public static class FieldsPrefabsEditor
    {
        private static FootstepsDb _db;
        private static int _toolBarIndex;
        private static int _selectedIndexGroup;
        private static Vector2 _propertyScrollPosLeftList;
        private static string _currentAssetName;
        private static List<bool> _statesFoldouts = new List<bool>();
        
        public static void DrawContent()
        {
            EditorGUI.BeginChangeCheck();
            {
                if (_db == null)
                {
                    _db = (FootstepsDb)EditorStatic.GetDb(typeof(FootstepsDb));
                    EditorGUILayout.HelpBox("Db is missing", MessageType.Warning);
                }
                else
                {
                    _toolBarIndex = GUILayout.Toolbar(_toolBarIndex, EditorStatic.FieldsPrefabsToolbarOptions, EditorStatic.ToolBarStyle, EditorStatic.ToolbarHeight);
                    switch (_toolBarIndex)
                    {
                        case 0:
                            FieldsEditor.DrawList();
                            break;
                        case 1:
                            UiPrefabsEditor.DrawPrefabList();
                            break;
                        case 2:
                            PooledPrefabsEditor.DrawList();
                            break;
                    }
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                if (_db != null)
                {
                    _db.ForceRefresh(); 
                }
            }
        }

     
        
       
        
      
        
        public static void DrawSelectionList()
        {
            var entries = _db.ListEntryNames();
            GUILayout.BeginVertical(GUILayout.Width(EditorStatic.LIST_VIEW_WIDTH), GUILayout.ExpandHeight(true));
            {
                _propertyScrollPosLeftList = GUILayout.BeginScrollView(_propertyScrollPosLeftList,
                    GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                {
                    GUILayout.Label("SELECT " + _db.ObjectName() + ": ",
                        GUILayout.Width(EditorStatic.LIST_VIEW_WIDTH - 30));
                    var toolbarOptions = new GUIContent[_db.Count()];
                    for (int i = 0; i < _db.Count(); i++)
                    {
                        toolbarOptions[i] = new GUIContent(entries[i], null, "");
                    }

                    var previousIndex = _selectedIndexGroup;

                    float lElemH2 = toolbarOptions.Length * 40;
                    _selectedIndexGroup = GUILayout.SelectionGrid(_selectedIndexGroup, toolbarOptions, 1,
                        EditorStatic.VerticalBarStyle, GUILayout.Height(lElemH2),
                        GUILayout.Width(EditorStatic.LIST_VIEW_WIDTH - 10));
                    if (previousIndex != _selectedIndexGroup) EditorStatic.ResetFocus();
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
        }
    }
}
