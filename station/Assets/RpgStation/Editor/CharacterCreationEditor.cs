using System.Collections.Generic;
using RPG.Editor;
using UnityEditor;

namespace Station
{
    public static class CharacterCreationEditor
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
            var instanceFound = EditorStatic.GetAllScriptables<BaseCharacterCreation>();
            if (instanceFound.Length > 0)
            {
                List<string> objectsNames = new List<string>();
                int selectedObjectId = 0;
                for (var index = 0; index < instanceFound.Length; index++)
                {
                    var scriptable = instanceFound[index];
                    objectsNames.Add(scriptable.name);
                    if (_settingsDb.Get()?.CharacterCreation == scriptable)
                    {
                        selectedObjectId = index;
                    }
                }

                selectedObjectId = EditorGUILayout.Popup("CharacterCreation: ",selectedObjectId,objectsNames.ToArray());
                _settingsDb.Get().CharacterCreation = instanceFound[selectedObjectId];
                if (_settingsDb.Get().Mechanics)
                {
                    EditorGUILayout.HelpBox(_settingsDb.Get().Mechanics.Description(), MessageType.Info);
                }

              


            }
            else
            {
                EditorGUILayout.HelpBox("No Character creation scriptables found", MessageType.Warning);
            }
        }
    }
}

