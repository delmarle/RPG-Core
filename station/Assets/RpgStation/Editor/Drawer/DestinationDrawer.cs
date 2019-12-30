
using System.Linq;
using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
    [CustomPropertyDrawer(typeof(DestinationModel))]
    public class DestinationDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            var amountRect = new Rect(position.x, position.y, 100, position.height);
            var unitRect = new Rect(position.x + 105, position.y, 120, position.height);

            var scene = property.FindPropertyRelative("SceneId");
            var spawn = property.FindPropertyRelative("SpawnId");
            var sceneDb = (ScenesDb) EditorStatic.GetDb(typeof(ScenesDb));
           
           

            if (sceneDb.Any() == false)
            {
                EditorGUI.HelpBox(unitRect, "no scenes in the database found", MessageType.Warning);
            }
            else
            {
                int index = sceneDb.GetIndex(scene.stringValue);
                index = EditorGUI.Popup(amountRect, index, sceneDb.ListEntryNames());
                if (index == -1)
                {
                    index = 0;
                }
                scene.stringValue = sceneDb.GetKey(index);
                var spawnsName = sceneDb.GetEntry(scene.stringValue).SpawnNames();
                spawn.intValue = EditorGUI.Popup(unitRect, spawn.intValue,spawnsName );
            }

          
            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}

