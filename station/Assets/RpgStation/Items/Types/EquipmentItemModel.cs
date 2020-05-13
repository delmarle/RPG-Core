 
using System;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Station
{
    [Serializable]
    public class EquipmentItemModel : BaseItemModel
    {
        public string EquipmentType;
        
        
        public override void OnUse(BaseCharacter user)
        {
           
        }
#if UNITY_EDITOR
        public override void DrawSpecificEditor()
        {
            EditorGUILayout.HelpBox("Equipment", MessageType.Info);
            var itemSettings = (ItemsSettingsDb) BaseDb.GetDbFromEditor(typeof(ItemsSettingsDb));
            var equipmentSlots = itemSettings.Get().EquipmentSlots;
            if (string.IsNullOrEmpty(EquipmentType))
            {
                EquipmentType = equipmentSlots.FirstOrDefault().Key;
            }
            else
            {
                var listNames = equipmentSlots.Values.Select(x => x.Name.GetValue()).ToArray();
                var listKeys = equipmentSlots.Keys.ToArray();
                var currentIndex = listKeys.FindIndex(x => x == EquipmentType);
                currentIndex = EditorGUILayout.Popup(currentIndex, listNames);
                EquipmentType = listKeys[currentIndex];
            }
        }
#endif
    }
}

