using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(NestedExample.NestedChildCustomDrawer))]
public class NestedChildDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

		Rect r1 = position;
		r1.width = 20;

		Rect r2 = position;
		r2.xMin = r1.xMax + 10;

		EditorGUI.BeginProperty(position, label, property);

		EditorGUI.PropertyField(r1, property.FindPropertyRelative("myBool"), GUIContent.none);
		EditorGUI.PropertyField(r2, property.FindPropertyRelative("myGameObject"), GUIContent.none);

		EditorGUI.EndProperty();
	}
}
