using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Station
{
  [Serializable]
  public class BaseAbilityDriver
  {
    public float Delay;
    
    
    #if UNITY_EDITOR
    public void DrawEditor()
    {
      #region BASE FIELDS
      Delay =  EditorGUILayout.FloatField("Delay: ", Delay);
      #endregion
      DrawFields();
    }

    protected virtual void DrawFields() { }
    #endif
  }
}

