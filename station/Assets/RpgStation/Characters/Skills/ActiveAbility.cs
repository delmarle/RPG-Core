using System;
using System.Collections.Generic;

using UnityEngine;

namespace Station
{
  [Serializable]
  public class PassiveAbility
  {
    //INFO
    public string Name = "New Ability";
    public string Description;
    public Sprite Icon;

    public int ParentSkillId;

    public bool UseRank = false;
    public List<PassiveAbilityRank> Ranks = new List<PassiveAbilityRank>();
    
  }
  [Serializable]
  public class ActiveAbility
  {
    //INFO
    public string Name = "New Ability";
    public string Description;
    public Sprite Icon;

    public int ParentSkillId;

    public bool UseRank = false;
    public List<AbilityRank> Ranks = new List<AbilityRank>();
  
    //TARGETING
    public Targeting Targeting = new Targeting();
    //CAST REQUIREMENT (some)
    //RANKS
    //VITAL CONSUMED
    //ITEM CONSUMED
    //EFFECTS
    //ANIMATION
    //Sounds
    //EFFECTS
  }

  public enum DriverTarget
  {
    Self,
    Target
  }
  public enum RequireTargetState
  {
    Alive,
    Dead,
    Any
  }

  public enum AbilityTargeting
  {
    None,
    Self,
    SelfOrFriendly,
    Friendly,
    Enemy,
    Any,
    NotSelf
  }
  [Serializable]
  public class Targeting
  {
    

    public RequireTargetState TargetRequiredState;
    public AbilityTargeting UsedAbilityTargeting;
  }
  
  [Serializable]
  public class PassiveAbilityRank
  {
    public List<IdIntegerValue> AttributeBonus = new List<IdIntegerValue>();
    public List<IdIntegerValue> StatBonus = new List<IdIntegerValue>();
    public List<IdIntegerValue> VitalBonus = new List<IdIntegerValue>();
  }
  [Serializable]
  public class AbilityRank
  {
    public float CastDistance;
    public float CoolDown;

    public CastingData Casting = new CastingData();

    public InvokingData Invoking = new InvokingData();
    //CAST REQUIREMENT:
    public List<IdIntegerValue> VitalsUsed = new List<IdIntegerValue>();
    
    //DRIVERS:
    public List<Projectile> ProjectileDrivers = new List<Projectile>();
    public List<Direct> DirectDrivers = new List<Direct>();
  }

  [Serializable]
  public class CastingData
  {
    public bool HasData;
    public ExitMode Option;
    public float Length = 1;
    public int AnimationId;
    public SoundConfig StartSound;
  }
  
  [Serializable]
  public class InvokingData
  {
    public ExitMode Option;
    public float Length = 1;
    public int AnimationId;
    public string InvokeSound;
  }

  public enum ExitMode
  {
    None,
    CanceledByMovement,
    BlockMovement
  }
}