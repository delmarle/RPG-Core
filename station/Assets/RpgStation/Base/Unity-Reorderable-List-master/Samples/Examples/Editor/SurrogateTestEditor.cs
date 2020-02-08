using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Malee.Editor;

[CustomEditor(typeof(SurrogateTest))]
public class SurrogateTestEditor : Editor {

	private ReorderableList list;
	private SerializedProperty myClassArray;

	private void OnEnable() {

		//custom list with more complex surrogate functionalty

		list = new ReorderableList(serializedObject.FindProperty("objects"));
		list.surrogate = new ReorderableList.Surrogate(typeof(GameObject), AppendObject);

		//myClassArray uses an auto surrogate property on the "ReorderableAttribute"
		//it's limited to only setting a property field to the dragged object reference. Still handy!

		myClassArray = serializedObject.FindProperty("myClassArray");
	}

	public override void OnInspectorGUI() {

		GUILayout.Label("Drag a GameObject onto the lists. Even though the list type is not a GameObject!");

		serializedObject.Update();

		list.DoLayoutList();
		EditorGUILayout.PropertyField(myClassArray);

		serializedObject.ApplyModifiedProperties();
	}

	private void AppendObject(SerializedProperty element, Object objectReference, ReorderableList list) {

		//we can do more with a custom surrogate delegate :)

		element.FindPropertyRelative("gameObject").objectReferenceValue = objectReference;
		element.FindPropertyRelative("name").stringValue = objectReference.name;
	}
}
