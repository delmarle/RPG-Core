using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
    public static class UiPrefabsEditor
    {
        private static GameSettingsDb _db;
        private static int _toolBarIndex;
        private static int _selectedIndexGroup;
        private static Vector2 _propertyScrollPosLeftList;
        
        public static void DrawPrefabList()
        {
            if (_db == null)
            {
                _db = (GameSettingsDb)EditorStatic.GetDb(typeof(GameSettingsDb));
                EditorGUILayout.HelpBox("Db is missing", MessageType.Warning);
            }
            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.HelpBox("add the ui popup that have an unique ID, they will be cached when the game start.", MessageType.Info);

                if (EditorStatic.SizeableButton(100, 32, "Add", "plus"))
                {
                    _db.Get()._cachedUniquePopup.Add(null);

                }
                EditorStatic.DrawLargeLine();
           
                for (var index = 0; index <  _db.Get()._cachedUniquePopup.Count; index++)
                {
                    var popup =  _db.Get()._cachedUniquePopup[index];
                    EditorGUILayout.BeginHorizontal();
                    _db.Get()._cachedUniquePopup[index] =
                        (UiPopup)EditorGUILayout.ObjectField(_db.Get()._cachedUniquePopup[index], typeof(UiPopup), false);
                    if (popup != null)
                    {

                        EditorGUI.BeginChangeCheck();
                        {
                            _db.Get()._cachedUniquePopup[index].PopupUniqueId = EditorGUILayout.TextField(_db.Get()._cachedUniquePopup[index].PopupUniqueId);
                        }
                        if (EditorGUI.EndChangeCheck())
                        {
                            EditorUtility.SetDirty(_db.Get()._cachedUniquePopup[index]);
                        }
                        

                        if (EditorStatic.SizeableButton(64, 18, "delete", "cross"))
                        {
                            _db.Get()._cachedUniquePopup.Remove(popup);
                            return;
                        }
                    }
             
                    EditorGUILayout.EndHorizontal();
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                _db.ForceRefresh(); 
            }
            
        }
    }
    
    public static class FieldsEditor
    {
        private static GameSettingsDb _db;
        private static int _toolBarIndex;
        private static int _selectedIndexGroup;
        private static Vector2 _propertyScrollPosLeftList;
        
        public static void DrawList()
        {
            if (_db == null)
            {
                _db = (GameSettingsDb)EditorStatic.GetDb(typeof(GameSettingsDb));
                EditorGUILayout.HelpBox("Db is missing", MessageType.Warning);
            }
            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.HelpBox("ID should not be empty", MessageType.Warning);
                if (EditorStatic.SizeableButton(100, 32, "Add", "plus"))
                {
                    _db.Get()._cachedFields.Add(new FieldVariable());

                }
                EditorStatic.DrawLargeLine();
           
                for (var index = 0; index <  _db.Get()._cachedFields.Count; index++)
                {
                    var field =  _db.Get()._cachedFields[index];
                    EditorGUILayout.BeginHorizontal();
            
                    field.Id = EditorGUILayout.TextField("ID:", field.Id);
                    field.TypeField = (FieldType)EditorGUILayout.EnumPopup(field.TypeField);
                    switch (field.TypeField)
                    {
                        case FieldType.Int:
                            field.ValueInt = EditorGUILayout.IntField(field.ValueInt);
                            break;
                        case FieldType.Float:
                            field.ValueFloat = EditorGUILayout.FloatField(field.ValueFloat);
                            break;
                        case FieldType.String:
                            field.ValueString = EditorGUILayout.TextField(field.ValueString);
                            break;
                        case FieldType.Bool:
                            field.ValueBool = EditorGUILayout.Toggle(field.ValueBool);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    _db.Get()._cachedFields[index] = field;
                    if (EditorStatic.SizeableButton(64, 18, "delete", "cross"))
                    {
                        _db.Get()._cachedFields.Remove(field);
                        return;
                    }
             
                    EditorGUILayout.EndHorizontal();
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                _db.ForceRefresh(); 
            }
            
        }
    }
    
     public static class PooledPrefabsEditor
    {
        private static GameSettingsDb _db;
        private static int _toolBarIndex;
        private static int _selectedIndexGroup;
        private static Vector2 _propertyScrollPosLeftList;
        
        public static void DrawList()
        {
            if (_db == null)
            {
                _db = (GameSettingsDb)EditorStatic.GetDb(typeof(GameSettingsDb));
                EditorGUILayout.HelpBox("Db is missing", MessageType.Warning);
            }
            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.HelpBox("ID should not be empty", MessageType.Warning);
                if (EditorStatic.SizeableButton(100, 32, "Add", "plus"))
                {
                    _db.Get()._poolPrefabs.Add(new PrefabReference());

                }
                EditorStatic.DrawLargeLine();
           
                for (var index = 0; index <  _db.Get()._poolPrefabs.Count; index++)
                {
                    var field =  _db.Get()._poolPrefabs[index];
                    EditorGUILayout.BeginHorizontal();
                    field.Id = EditorGUILayout.TextField("ID:", field.Id);
                    field.Prefab = (GameObject)EditorGUILayout.ObjectField(field.Prefab, typeof(GameObject), false);
                    field.InitialAmount = EditorGUILayout.IntField("Amount: ", field.InitialAmount);
                    _db.Get()._poolPrefabs[index] = field;
                    if (EditorStatic.SizeableButton(64, 18, "delete", "cross"))
                    {
                        _db.Get()._poolPrefabs.Remove(field);
                        return;
                    }
             
                    EditorGUILayout.EndHorizontal();
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                _db.ForceRefresh(); 
            }
            
        }
    }
}



