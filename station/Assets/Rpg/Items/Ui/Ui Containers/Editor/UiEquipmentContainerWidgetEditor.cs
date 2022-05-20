using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Station;

namespace RPG.Editor
{
    [CustomEditor(typeof(UiEquipmentContainerWidget))]
    public class UiEquipmentContainerWidgetEditor : UnityEditor.Editor
    {
        private EquipmentSlotsDb _eqSlotDb;
        SerializedProperty dragDummy;

        private void OnEnable()
        {
            _eqSlotDb = (EquipmentSlotsDb) EditorStatic.GetDb(typeof(EquipmentSlotsDb));
            dragDummy = serializedObject.FindProperty("_dragDummy");
        }

        public override void OnInspectorGUI()
        {
            UiEquipmentContainerWidget component = (UiEquipmentContainerWidget) target;
            if (component == null)
            {
                return;
            }
       
            EditorGUILayout.Space();
            EditorStatic.DrawSectionTitle(22, "Equipment container");
            EditorGUILayout.Space();
            if (EditorStatic.Button(true, 64, "Cache all components", "arrow_refresh"))
            {
                foreach (var slotTypeId in _eqSlotDb.Db)
                {
                    if (component.Slots.ContainsKey(slotTypeId.Key) == false)
                    {
                        component.Slots.Add(slotTypeId.Key, null);
                    }
                }
            }

            if (component.Slots.Count != _eqSlotDb.Count())
            {
                EditorGUILayout.HelpBox("this component is not initialized correctly", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.BeginVertical("box");

                Dictionary<string, UiContainerSlot> tempDict = new Dictionary<string, UiContainerSlot>();
                foreach (var entry in component.Slots)
                {
                    var slotName = _eqSlotDb.GetEntry(entry.Key).Name.GetValue();
                    var bgSprite = _eqSlotDb.GetEntry(entry.Key).Icon;
                    var obj = (UiContainerSlot)EditorGUILayout.ObjectField(slotName, component.Slots[entry.Key], typeof(UiContainerSlot),true);
                    if (obj)
                    {
                        obj.name = $"slot_{slotName}";
                        obj.SetBg(bgSprite);
                    }

                    tempDict.Add(entry.Key, obj);
                    EditorStatic.DrawThinLine();
                }

                component.Slots = tempDict;
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.Space();
            EditorStatic.DrawSectionTitle(22, "Other");
            EditorGUILayout.Space();
            serializedObject.Update();
            EditorGUILayout.PropertyField(dragDummy);
            serializedObject.ApplyModifiedProperties();
         
        }

    }
}

