

using UnityEngine;

namespace Station
{
  public class BaseEffect
  {
    public string EffectName;
    public Sprite EffectIcon;

    public virtual ApplyEffectResult ApplyEffect(BaseCharacter source, BaseCharacter target)
    {
      return ApplyEffectResult.Success;
    }
  }

  public enum ApplyEffectResult
  {
    Success,
    Miss,
    Evade,
    Resist,
    MissingTarget,
    TargetIsDead,
    Blocked
  }
}


