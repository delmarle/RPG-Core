using System;
using System.Collections.Generic;

namespace Station
{
  [Serializable]
  public class EffectHolder
  {
    public string EffectName;
    public List<DamageEffect> Damages = new List<DamageEffect>();
    public List<HealEffect> Heals = new List<HealEffect>();
    public List<ModifierEffect> Modifiers = new List<ModifierEffect>();
    public List<TeleportEffect> Teleports = new List<TeleportEffect>();
    public List<ResurrectEffect> Resurects = new List<ResurrectEffect>();
    //OVER TIME effects

    public void ApplyEffects(BaseCharacter source, BaseCharacter target)
    {
      foreach (var buff in Modifiers)
      {
        buff.ApplyEffect(source, target);
      }
      
      foreach (var dmg in Damages)
      {
        dmg.ApplyEffect(source, target);
      }
      
      foreach (var heal in Heals)
      {
        heal.ApplyEffect(source, target);
      }
      
      foreach (var tp in Teleports)
      {
        tp.ApplyEffect(source, target);
      }

      foreach (var rez in Resurects)
      {
        rez.ApplyEffect(source, target);
      }
    }
  }
}