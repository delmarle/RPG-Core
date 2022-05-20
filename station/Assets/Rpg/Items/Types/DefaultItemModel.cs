
 
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Station
{
    [Serializable]
    public class DefaultItemModel : BaseItemModel
    {
        public override void OnUse(BaseCharacter user)
        {
           
        }
        #if UNITY_EDITOR
        public override void DrawSpecificEditor()
        {
            EditorGUILayout.HelpBox("default item type, no specific functionality", MessageType.Info);
        }
        #endif
    }
}

