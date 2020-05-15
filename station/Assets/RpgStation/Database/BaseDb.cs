
using System;
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

        public static BaseDb GetDbFromEditor(Type typeDb)
        {
#if UNITY_EDITOR
            string dbPath = "Assets/Content/Databases/" +  typeDb.Name + @".asset";
            BaseDb found = AssetDatabase.LoadAssetAtPath<BaseDb>(dbPath);
            if (found == null)
            {
                Debug.LogError("cant find db at: " + dbPath);
            }

            return found;
#endif
            return null;
        }
    }
}