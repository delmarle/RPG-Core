using System;
using UnityEditor;

namespace Station
{
  [Serializable]
  public class Direct : BaseAbilityDriver
  {
    public EffectHolder Effects = new EffectHolder();
    public DriverTarget Target;
    
#if UNITY_EDITOR
    protected override void DrawFields()
    {
      Target = (DriverTarget)EditorGUILayout.EnumPopup("Target: ", Target);
    }
#endif
  }
}

