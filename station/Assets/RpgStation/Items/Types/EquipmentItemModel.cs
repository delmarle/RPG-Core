 
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
            var equipmentSlotsDb = (EquipmentSlotsDb) BaseDb.GetDbFromEditor(typeof(EquipmentSlotsDb));

            if (string.IsNullOrEmpty(EquipmentType))
            {
                EquipmentType = equipmentSlotsDb.GetEntry(0)?.Name?.Key;
            }
            else
            {
                var listNames = equipmentSlotsDb.ListEntryNames();
                var listKeys = equipmentSlotsDb.Db.Keys.ToArray();
                var currentIndex = listKeys.FindIndex(x => x == EquipmentType);
                currentIndex = EditorGUILayout.Popup("Equipment slot used: ", currentIndex, listNames);
                if (listNames.Length > 0)
                {
                    if (currentIndex == -1)
                    {
                        currentIndex = 0;
                    }

                    EquipmentType = listKeys[currentIndex];
                }
                else
                {
                    EditorGUILayout.HelpBox("missing equipments slots", MessageType.Info);
                }

              
            }
        }
#endif
    }
}

