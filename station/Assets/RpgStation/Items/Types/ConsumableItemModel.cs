
 
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Station
{
    [Serializable]
    public class ConsumableItemModel : BaseItemModel
    {
        public override void OnUse(BaseCharacter user)
        {
           
        }
        #if UNITY_EDITOR
        public override void DrawSpecificEditor()
        {
            EditorGUILayout.HelpBox("consume", MessageType.Info);
        }
        #endif
    }
}

