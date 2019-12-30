using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Station
{
    [CustomPropertyDrawer(typeof(SimpleAnimation))]
    public class SimpleAnimationDrawer : PropertyDrawer
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
            //scene.intValue = EditorGUI.Popup(amountRect, scene.intValue, Resource.ScenesDatabase.ListEntryNames());
            //spawn.intValue = EditorGUI.Popup(unitRect, spawn.intValue, Resource.ScenesDatabase.GetEntry(scene.intValue).SpawnNames());
            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}

