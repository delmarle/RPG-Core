
using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Editor;
using UnityEditor;
using UnityEngine;

namespace Station
{
    public static class ItemModelEditorView
    {
        private static Dictionary<Type, ItemEditorViewer> _itemActionMap = new Dictionary<Type, ItemEditorViewer>();

        public static void Register()
        {
            var listFound = ReflectionUtils.FindAllClassFromInterface(typeof(ItemEditorViewer));

            foreach (var typeFound in listFound)
            {
                var instanceView = (ItemEditorViewer)Activator.CreateInstance(typeFound);
            
                if (_itemActionMap.ContainsKey(instanceView.TypeId()))
                {
                    _itemActionMap[instanceView.TypeId()] = instanceView;
                }
                else
                {
                    _itemActionMap.Add(instanceView.TypeId(), instanceView);
                }
            }
        }

        public static void DrawSpecificView(BaseItemModel model)
        {
            var target = model.GetType();
            if (_itemActionMap.ContainsKey(target))
            {
                var instance = _itemActionMap[target];
                instance?.DrawView(model);
            }
        }
    }

    public interface ItemEditorViewer
    {
        Type TypeId();
        void DrawView(BaseItemModel model);
    }

    public class ConsumableItemEditor: ItemEditorViewer
    {
        public Type TypeId()
        {
            return typeof(ConsumableItemModel);
        }

        public void DrawView(BaseItemModel model)
        {
            var consumable = (ConsumableItemModel)model;
            if (consumable)
            {
                AbilityEffectEditor.DrawEffectStack(consumable.Effects);
            }
        }
    }
    
    public class EquipmentItemEditor: ItemEditorViewer
    {
        public Type TypeId()
        {
            return typeof(EquipmentItemModel);
        }

        public void DrawView(BaseItemModel model)
        {
            var equipment = (EquipmentItemModel)model;
            if (equipment)
            {
                EditorGUILayout.HelpBox("Equipment", MessageType.Info);
                var equipmentType = (EquipmentTypesDb) BaseDb.GetDbFromEditor(typeof(EquipmentTypesDb));

                if (string.IsNullOrEmpty(equipment.EquipmentType))
                {
                    equipment.EquipmentType = equipmentType.GetEntry(0)?.Name?.Key;
                }
                else
                {
                    var listNames = equipmentType.ListEntryNames();
                    var listKeys = equipmentType.Db.Keys.ToArray();
                    var currentIndex = listKeys.FindIndex(x => x == equipment.EquipmentType);
                    currentIndex = EditorGUILayout.Popup("Equipment type: ", currentIndex, listNames);
                    if (listNames.Length > 0)
                    {
                        if (currentIndex == -1)
                        {
                            currentIndex = 0;
                        }

                        equipment.EquipmentType = listKeys[currentIndex];
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("missing equipments types", MessageType.Info);
                    }
                }
                
                EditorStatic.DrawSectionTitle("bonuses", 0);
                
                EditorStatic.DrawThinLine(10);
                var _statisticsDb = (StatisticDb) BaseDb.GetDbFromEditor(typeof(StatisticDb));
                RpgEditorStatic.DrawBonusWidget(equipment.StatisticsBonuses, "Statistic Bonus:", _statisticsDb);
                var _attributesDb = (AttributesDb) BaseDb.GetDbFromEditor(typeof(AttributesDb));
                RpgEditorStatic.DrawBonusWidget(equipment.AttributesBonuses, "Attributes Bonus:", _attributesDb);
                VitalBonusSection(equipment);
            }
        }
        
        private static void VitalBonusSection(EquipmentItemModel current)
        {
            EditorGUILayout.LabelField("bonuses Vitals:");
            var _vitalsDb = (VitalsDb) BaseDb.GetDbFromEditor(typeof(VitalsDb));
            var vitalNames = _vitalsDb.ListEntryNames();
            //draw list
            for (var index = 0; index < current.VitalBonuses.Count; index++)
            {
                var vtlBonus = current.VitalBonuses[index];
                GUILayout.BeginHorizontal("box");
                {
                    EditorGUILayout.LabelField("vital " + index + ":", GUILayout.Width(145));
                    int energyIndex = _vitalsDb.GetIndex(vtlBonus.Id);
                    
                    energyIndex = EditorGUILayout.Popup("vital:", energyIndex, vitalNames, GUILayout.Width(250));
                    if (energyIndex < 0)
                    {
                        energyIndex = 0;
                    }
                    
                    vtlBonus.Id =_vitalsDb.GetKey(energyIndex); 
                    GUILayout.Space(5);
                    vtlBonus.Value = EditorGUILayout.IntField("Bonus: ", vtlBonus.Value, GUILayout.Width(250));
                    GUILayout.Space(5);

                    if (EditorStatic.SizeableButton(65, 16, "DELETE", ""))
                    {
                        current.VitalBonuses.Remove(vtlBonus);
                        return;
                    }
                    
                }

                GUILayout.EndHorizontal();
            }
            //add button

            if (EditorStatic.SizeableButton(90, 30, "Add", "plus"))
            {
                current.VitalBonuses.Add(new IdIntegerValue(_vitalsDb.GetKey(0), 5));
            }
        }
    }
}
