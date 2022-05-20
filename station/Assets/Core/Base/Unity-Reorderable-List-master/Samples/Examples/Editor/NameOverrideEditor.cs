using UnityEditor;
using Malee.Editor;
using System;

[CanEditMultipleObjects]
[CustomEditor(typeof(NameOverride))]
public class NameOverrideEditor : Editor {

	private SerializedProperty autoList;
	private SerializedProperty dynamicList;
	private SerializedProperty nameOverride;
	private SerializedProperty nestedNameOverride;	

	private void OnEnable() {

		//get references to the properties. Could also create the ReorderableList directly here which would avoid the lookup in ReorderableDrawer.GetList
		//but just wanted to highlight the usage of the [Reorderable] attribute

		autoList = serializedObject.FindProperty("autoNameList");
		dynamicList = serializedObject.FindProperty("dynamicNameList");
		nameOverride = serializedObject.FindProperty("nameOverride");
		nestedNameOverride = serializedObject.FindProperty("nestedNameOverride");
	}

	public override void OnInspectorGUI() {

		serializedObject.Update();

		EditorGUILayout.PropertyField(nameOverride);
		EditorGUILayout.PropertyField(nestedNameOverride);

		EditorGUILayout.PropertyField(autoList);
		EditorGUILayout.PropertyField(dynamicList);

		//dynamically change the names of the elements

		UpdateElementNames(dynamicList, nameOverride);
		UpdateNestedElementNames(dynamicList.FindPropertyRelative("array"), nestedNameOverride);

		serializedObject.ApplyModifiedProperties();
	}

	private void UpdateNestedElementNames(SerializedProperty array, SerializedProperty nameOverride) {

		for (int i = 0; i < array.arraySize; i++) {

			UpdateElementNames(array.GetArrayElementAtIndex(i).FindPropertyRelative("nested"), nameOverride);
		}
	}

	private void UpdateElementNames(SerializedProperty listProperty, SerializedProperty nameOverride) {

		ReorderableList list = ReorderableDrawer.GetList(listProperty, ReorderableDrawer.ARRAY_PROPERTY_NAME);

		if (list != null) {

			list.elementNameOverride = nameOverride.stringValue;
		}
	}
}
