 
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
            var equipmentType = (EquipmentTypesDb) BaseDb.GetDbFromEditor(typeof(EquipmentTypesDb));

            if (string.IsNullOrEmpty(EquipmentType))
            {
                EquipmentType = equipmentType.GetEntry(0)?.Name?.Key;
            }
            else
            {
                var listNames = equipmentType.ListEntryNames();
                var listKeys = equipmentType.Db.Keys.ToArray();
                var currentIndex = listKeys.FindIndex(x => x == EquipmentType);
                currentIndex = EditorGUILayout.Popup("Equipment type: ", currentIndex, listNames);
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
                    EditorGUILayout.HelpBox("missing equipments types", MessageType.Info);
                }

              
            }
        }
#endif
    }
}

