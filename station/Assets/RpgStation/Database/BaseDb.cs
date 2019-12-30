
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Station
{
    public abstract class BaseDb : ScriptableObject
    {
        public virtual string ObjectName()
        {
            return string.Empty;
        }

        public void ForceRefresh()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }
}