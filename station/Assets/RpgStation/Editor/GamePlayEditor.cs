using System.Collections.Generic;
using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
    public static class GamePlayEditor
    {
        private static GameSettingsDb _settingsDb;
        
        public static void Draw()
        {
            if (_settingsDb == null)
            {
                _settingsDb = (GameSettingsDb)EditorStatic.GetDb(typeof(GameSettingsDb));
            }
            
            EditorGUI.BeginChangeCheck();
            {
                DrawContent();
            }
            if (EditorGUI.EndChangeCheck())
            {
                _settingsDb.ForceRefresh(); 
            }
        }

        public static void DrawContent()
        {
            GUILayout.Label(new GUIContent(EditorStatic.GetEditorTexture("core_full")), EditorStatic.PublisherNameStyle);
            GUILayout.Space(15);
            var mechanicsFound = EditorStatic.GetAllScriptables<StationMechanics>();
            if (mechanicsFound.Length > 0)
            {
                EditorGUILayout.BeginHorizontal();
                
                List<string> objectsNames = new List<string>();
                int selectedObjectId = 0;
                for (var index = 0; index < mechanicsFound.Length; index++)
                {
                    var scriptable = mechanicsFound[index];
                    objectsNames.Add(scriptable.name);
                    if (_settingsDb.Get().Mechanics == scriptable) selectedObjectId = index;
                }
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical();
                selectedObjectId = EditorGUILayout.Popup("Runtime mechanics used: ",selectedObjectId,objectsNames.ToArray());
                _settingsDb.Get().Mechanics = mechanicsFound[selectedObjectId];
                if (_settingsDb.Get().Mechanics)
                {
                    EditorGUILayout.HelpBox(_settingsDb.Get().Mechanics.Description(), MessageType.Info);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical();
                if (EditorStatic.SizeableButton(200, 32, "open save folder", ""))
                {
                    EditorStatic.OpenDataFolder();
                }
                if (EditorStatic.SizeableButton(200, 32, "clear save", ""))
                {
                    EditorStatic.ClearSaveFolder();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("No Mechanics scriptables found", MessageType.Warning);
            }
        }
    }
}

