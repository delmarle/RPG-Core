

using UnityEngine;

namespace Station
{
  public class BaseEffect
  {
    public string EffectName;
    public Sprite EffectIcon;

    public virtual void ApplyEffect(BaseCharacter source, BaseCharacter target)
    {
    }
  }
}


