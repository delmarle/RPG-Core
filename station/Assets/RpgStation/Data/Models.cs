using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Station

{
    [Serializable] public class StringDictionary : SerializableDictionary<string, string> {}
    
    [Serializable]
    public class GameSettingsModel
    {
      public string CharacterCreationType;
      public StationMechanics Mechanics;
      public int MaxTeamSize = 1;
    }


    [Serializable]
    public class DestinationModel
    {
        public string SceneId;
        public int SpawnId;
    }

    [Serializable]
    public class VitalModel
    {
      public string Name = "new vital";
      public int Id;
      public Color Color = Color.white;
      public string Description;
      public Sprite Icon;
      public int DefaultValue = 100;
      public float RegenCycle = 4;
      public int DefaultRegenValue = 1;
      public RegenType RegenMode;
      public VitalType UsageType;
      public List<IdFloatValue> AttributesBonuses = new List<IdFloatValue>();
      public enum RegenType
      {
        PercentBased,
        NumericBased
      }

      public enum VitalType
      {
        PrimaryHealth,
        SecondaryHealth,
        Energy
      }
    }
    
    [Serializable]
    public class AttributeModel
    {
      public string Name = "new Attribute";
      public int Id;
      public string Description;
      public Sprite Icon;
      public int DefaultValue = 10;
    
      #region [[Optional]]
      public List<IdIntegerValue> VitalBonuses = new List<IdIntegerValue>();
      public List<IdFloatValue> StatisticBonuses = new List<IdFloatValue>();
      #endregion
    }
    
    [Serializable]
    public class RaceModel
    {
      public string Name = "New Race";
      public string Description;
      public Sprite Icon;
    
      #region [[OPTIONAL]]
      public List<IdIntegerValue> AttributeBonuses = new List<IdIntegerValue>();
      public List<IdIntegerValue> VitalBonuses = new List<IdIntegerValue>();
    
      //TODO clear from editor
      private Dictionary<int, int> _attributesBonusesMap = new Dictionary<int, int>();
      private Dictionary<int, int> _vitalsBonusesMap = new Dictionary<int, int>();
    
      public int GetAttributeRaceBaseValue(int attributeId)
      {
        if (_attributesBonusesMap.Count != AttributeBonuses.Count)
        {
          _attributesBonusesMap.Clear();
          foreach (var bonus in AttributeBonuses)
          {
            _attributesBonusesMap.Add(bonus.Id, bonus.Value);
          }
        }
        return _attributesBonusesMap.ContainsKey(attributeId) ? _attributesBonusesMap[attributeId] : 0;
      }
    
      public int GetVitalsBonus(int vitalId)
      {
        if (_vitalsBonusesMap.Count != VitalBonuses.Count)
        {
          _vitalsBonusesMap.Clear();
          foreach (var bonus in VitalBonuses)
          {
            _vitalsBonusesMap.Add(bonus.Id, bonus.Value);
          }
        }

        return _vitalsBonusesMap.ContainsKey(vitalId) ? _vitalsBonusesMap[vitalId] : 0;
      }

      #endregion
    }

    [Serializable]
    public class PlayerClassModel
    {
      public string Name = "New class";
      [Multiline] public string Description;
      public Sprite Icon;
      public List<RaceMeta> AllowedRaces = new List<RaceMeta>();
      public bool UseHealth;
      public IdIntegerValue HealthVital = new IdIntegerValue(0, 0);
      public bool UseSecondaryHealth;
      public IdIntegerValue SecondaryHealthVital = new IdIntegerValue(0, 0);
      public List<IdIntegerValue> EnergyVitals = new List<IdIntegerValue>();
      public List<IdIntegerValue> AttributesBonuses = new List<IdIntegerValue>();
      public List<IdFloatValue> StatisticsBonuses = new List<IdFloatValue>();


      public List<RankedTimeIdSave> OwnedAbilities = new List<RankedTimeIdSave>();
      public List<RankedIdSave> OwnedSkills = new List<RankedIdSave>();
      public List<RankedIdSave> OwnedPassiveAbilities = new List<RankedIdSave>();
      public bool CanUpgradeClass;
      public List<string> UpgradeClasses = new List<string>();
      public CharacterCalculation StatsCalculator;
      public AttackData Attack = new AttackData();

      #region [[ RUNTIME CACHING ]]

      private Dictionary<int, int> _attributesBonusesMap = new Dictionary<int, int>();
      private Dictionary<int, float> _statisticsBonusesMap = new Dictionary<int, float>();
      private Dictionary<int, int> _vitalsBonusesMap = new Dictionary<int, int>();

      public int GetAttributeBonus(int attributeId)
      {
        if (_attributesBonusesMap.Count != AttributesBonuses.Count)
        {
          _attributesBonusesMap.Clear();
          foreach (var bonus in AttributesBonuses)
          {
            _attributesBonusesMap.Add(bonus.Id, bonus.Value);
          }
        }

        return _attributesBonusesMap.ContainsKey(attributeId) ? _attributesBonusesMap[attributeId] : 0;
      }

      public int GetVitalsBonus(int vitalId)
      {
        if (_vitalsBonusesMap.Count != EnergyVitals.Count)
        {
          _vitalsBonusesMap.Clear();
          foreach (var bonus in EnergyVitals)
          {
            _vitalsBonusesMap.Add(bonus.Id, bonus.Value);
          }
        }

        return _vitalsBonusesMap.ContainsKey(vitalId) ? _vitalsBonusesMap[vitalId] : 0;
      }

      public float GetStatsBonus(int statId)
      {
        if (_statisticsBonusesMap.Count != StatisticsBonuses.Count)
        {
          _statisticsBonusesMap.Clear();
          foreach (var bonus in StatisticsBonuses)
          {
            _statisticsBonusesMap.Add(bonus.Id, bonus.Value);
          }
        }

        return _statisticsBonusesMap.ContainsKey(statId) ? _statisticsBonusesMap[statId] : 0f;
      }

      #endregion
    }

    [Serializable]
    public class NpcModel
    {
      public string Name = "New NPC";
      [Multiline] public string Description;
      public Sprite Icon;
      public string RaceId;
      public string FactionId;
      public string PrefabId;
      
      public bool UseHealth;
      public IdIntegerValue HealthVital = new IdIntegerValue(0, 0);
      public bool UseSecondaryHealth;
      public IdIntegerValue SecondaryHealthVital = new IdIntegerValue(0, 0);
      public List<IdIntegerValue> EnergyVitals = new List<IdIntegerValue>();
      public List<IdIntegerValue> AttributesBonuses = new List<IdIntegerValue>();
      public List<IdFloatValue> StatisticsBonuses = new List<IdFloatValue>();

      //faction
      //Ai
      //loot tables
      //dialogue
      //shop tables
      //counter updates

      public List<ActiveAbility> OwnedAbilities = new List<ActiveAbility>();
    
      public CharacterCalculation StatsCalculator;
      public AttackData Attack = new AttackData();

      #region [[ RUNTIME CACHING ]]

      private Dictionary<int, int> _attributesBonusesMap = new Dictionary<int, int>();
      private Dictionary<int, float> _statisticsBonusesMap = new Dictionary<int, float>();
      private Dictionary<int, int> _vitalsBonusesMap = new Dictionary<int, int>();

      public int GetAttributeBonus(int attributeId)
      {
        if (_attributesBonusesMap.Count != AttributesBonuses.Count)
        {
          _attributesBonusesMap.Clear();
          foreach (var bonus in AttributesBonuses)
          {
            _attributesBonusesMap.Add(bonus.Id, bonus.Value);
          }
        }

        return _attributesBonusesMap.ContainsKey(attributeId) ? _attributesBonusesMap[attributeId] : 0;
      }

      public int GetVitalsBonus(int vitalId)
      {
        if (_vitalsBonusesMap.Count != EnergyVitals.Count)
        {
          _vitalsBonusesMap.Clear();
          foreach (var bonus in EnergyVitals)
          {
            _vitalsBonusesMap.Add(bonus.Id, bonus.Value);
          }
        }

        return _vitalsBonusesMap.ContainsKey(vitalId) ? _vitalsBonusesMap[vitalId] : 0;
      }

      public float GetStatsBonus(int statId)
      {
        if (_statisticsBonusesMap.Count != StatisticsBonuses.Count)
        {
          _statisticsBonusesMap.Clear();
          foreach (var bonus in StatisticsBonuses)
          {
            _statisticsBonusesMap.Add(bonus.Id, bonus.Value);
          }
        }

        return _statisticsBonusesMap.ContainsKey(statId) ? _statisticsBonusesMap[statId] : 0f;
      }

      #endregion
    }

    [Serializable]
    public class RaceMeta
    {
        public string RaceId;
        public string MaleAddressPrefab;
        public string FemaleAddressPrefab;

        public RaceMeta(string raceId)
        {
            RaceId = raceId;
        }
    }
    
    [Serializable]
    public class StatisticModel
    {
        public int Id;
        public string Name = "new statistic";
        public Color Color = Color.white;
        public string Description;
        public Sprite Icon;
        public float MinimumValue;
        public float MaximumValue;
    }

    public enum ModifierType
    {
      Buff,
      Debuff
    }

    public class RuntimeModifier
    {
      public ModifierEffect Modifier;
      public float TimeLeft;
      public int CurrentStack;

      public RuntimeModifier(ModifierEffect effect, int stack)
      {
        Modifier = effect;
        TimeLeft = Modifier.Length;
        CurrentStack = stack;
      }

      public void DecreaseTimer()
      {
        TimeLeft -= Time.deltaTime;
      }

      public bool IsOutOfTime()
      {
        return TimeLeft <= 0;
      }
    }

    #region EFFECTS
    [Serializable]
    public class ResurrectEffect : BaseEffect
    {
      public float percentRefreshed = 0.1f;
      
      public override ApplyEffectResult ApplyEffect(BaseCharacter source, BaseCharacter target)
      {
        if (target == null)
        {
          return ApplyEffectResult.MissingTarget;
        }
        SceneSystem sceneSystem = RpgStation.GetSystemStatic<SceneSystem>();
        if (sceneSystem.IsTraveling)
        {
          return ApplyEffectResult.Blocked;
        }

        if (target.IsDead == false)
        {
          return ApplyEffectResult.TargetIsDead;
        }

        GameGlobalEvents.OnBeforeLeaveScene?.Invoke();
        GameGlobalEvents.OnTriggerSceneSave?.Invoke();

        target.Calculator.Revive(percentRefreshed);
        return ApplyEffectResult.Success;
      }
    }
    
    [Serializable]
    public class TeleportEffect : BaseEffect
    {
      public DestinationModel Destination;
      public override ApplyEffectResult ApplyEffect(BaseCharacter source, BaseCharacter target)
      {
        GameGlobalEvents.OnBeforeLeaveScene?.Invoke();

        GameGlobalEvents.OnTriggerSceneSave?.Invoke();

        
        SceneSystem sceneSystem = RpgStation.GetSystemStatic<SceneSystem>();
        var dbSystem =  RpgStation.GetSystemStatic<DbSystem>();
        var sceneDb = dbSystem.GetDb<ScenesDb>();
        if (sceneSystem.IsTraveling)
        {
          return ApplyEffectResult.Blocked;
        }
        
        var sceneData = sceneDb.GetEntry(Destination.SceneId);
        var model = new TravelModel {SceneName = sceneData.VisualName};
        sceneSystem.InjectDestinationInSave(Destination);
        sceneSystem.TravelToZone(model);
        return ApplyEffectResult.Success;
      }
    }

    [Serializable]
  public class ModifierEffect : BaseEffect
  {
    public ModifierType ModifierType;
    public List<IdIntegerValue> AttributesBuffs = new List<IdIntegerValue>();
    public List<IdIntegerValue> VitalsBuffs = new List<IdIntegerValue>();
    public List<IdFloatValue> StatisticsBuffs = new List<IdFloatValue>();
    public int MaxStack = 1;
    public float Length = 10;
    public string Identifier;

    public override ApplyEffectResult ApplyEffect(BaseCharacter source, BaseCharacter target)
    {
      FloatingPopupSystem.SpawnObject(ModifierType.ToString().ToLower(), EffectName,target.GetTop(), source, target, EffectIcon);
      target.Stats.ReceiveModifier(this, source);
      return ApplyEffectResult.Success;
    }

    public void AddBonusToHandler(StatsHandler handler, int stacks)
    {
      foreach (var att in AttributesBuffs)
      {
        handler.Attributes[att.Id].ModifiedAmount += att.Value * stacks;
      }

      foreach (var vt in VitalsBuffs)
      {
        foreach (var vital in handler.Vitals)
        {
          if (vital.Key == vt.Id)
          {
            vital.Value.ModifiedAmount += vt.Value * stacks;
          }
        }
      }

      foreach (var stt in StatisticsBuffs)
      {
        handler.Statistics[stt.Id].ModifiedAmount += stt.Value * stacks;
      }
    }
    
    public void RemoveBonusToHandler(StatsHandler handler, int stacks)
    {
      foreach (var att in AttributesBuffs)
      {
        handler.Attributes[att.Id].ModifiedAmount -= att.Value * stacks;
      }

      foreach (var vt in VitalsBuffs)
      {
        foreach (var vital in handler.Vitals)
        {
          if (vital.Key == vt.Id)
          {
            vital.Value.ModifiedAmount -= vt.Value * stacks;
          }
        }
      }

      foreach (var stt in StatisticsBuffs)
      {
        handler.Statistics[stt.Id].ModifiedAmount -= stt.Value * stacks;
      }
    }
    
  }
  
  [Serializable]
  public class HealEffect : BaseEffect
  {
    public float MinValue;
    public float MaxValue;
    
    public override ApplyEffectResult ApplyEffect(BaseCharacter source, BaseCharacter target)
    {
      var calculation = source.Calculator;
      float value = Random.Range(MinValue, MaxValue);
      FloatingPopupSystem.SpawnObject("heal", ((int)value).ToString(),target.GetTop(), source, target);
      target.Calculator.Heal(new VitalChangeData((int)value,source));
      return ApplyEffectResult.Success;
    }
  }

  [Serializable]
  public class DamageEffect : BaseEffect
  {
    public float MinValue = 5;
    public float MaxValue = 22;
    public float CriticalBonus;
    public const string MeleeDamageType = "Melee";

    public override ApplyEffectResult ApplyEffect(BaseCharacter source, BaseCharacter target)
    {
      if (target)
      {
        var calculation = source.Calculator;
        
        float hitPower = calculation.GetHitChance(90);
        float targetEvadePower = target.Calculator.GetEvadePower();
        float targetBlockPower = target.Calculator.GetBlockPower();
        float total = hitPower + targetEvadePower + targetBlockPower;
        var hitRoll = Random.Range(0, total);

        var playerHit = Random.value * 100 <= hitPower;
        if (playerHit)
        {
          if (hitRoll <= hitPower)
          {
            //we touch
                  
            var damageData = calculation.GetDamageCalculation(MeleeDamageType,  Random.Range(MinValue, MaxValue), CriticalBonus);
            var dmgType = damageData.IsCritical ? FloatingPopupSystem.TYPE_DAMAGE_CRITICAL : FloatingPopupSystem.TYPE_DAMAGE;
            FloatingPopupSystem.SpawnObject(dmgType, damageData.Amount.ToString(),target.GetTop(), source, target);
            target.Calculator.ReceiveDamage(damageData);
            return ApplyEffectResult.Success;
          }
          else if(hitRoll <= hitPower+targetEvadePower)
          {
            //target evaded
            FloatingPopupSystem.SpawnObject(FloatingPopupSystem.TYPE_EVADE, string.Empty,target.GetTop(), source, target);
            return ApplyEffectResult.Evade;
          }
          else
          {
            //target blocked
            return ApplyEffectResult.Blocked;
          }
        }
        else
        {
          //we miss
          FloatingPopupSystem.SpawnObject(FloatingPopupSystem.TYPE_MISS, string.Empty,target.GetTop(), source, target);
          return ApplyEffectResult.Miss;
        }
      }
      else
      {
        return ApplyEffectResult.MissingTarget;
      }
    }
  }
#endregion

#region STATUS EFFECTS

public enum StatusEffectType
{
  Stun,
  Mesmerized,
  Enraged
}

#endregion

#region INTERACTIONS 
  [Serializable]
  public class InteractionConfig
  {
    public int Layer;
    public ClassTypeReference InteractibleType;
    public ShowHintType ShowHintMode;
    public HideHintOptions HideHintOptions;
    public InteractType TryInteractMode;
    public HoverMode HoverMode;
    public float InteractionTime;
    public CastingData _CastingData;
  }

  [Serializable]
  public class HideHintOptions
  {
    public bool UseDelay;
    public float Delay = 5;
    public bool UseDistance;
    public float Distance = 10;
    public bool UseCameraAngle;
    public float CameraAngle = 90;
  }

  public enum HoverMode
  {
    UseMouse,
    Fps
  }

  public enum ShowHintType 
  {
    None,                            //the hint are never shown
    Tap,                             //when object is tapped = > require HideHint mode
    Collision,                       //when leader character collide => require HideHint mode
    WhileInRange,                    //when leader character is in range 
    Hover                            //when cursor is hover (cursor is center screen for fps )
  }
  
  public enum InteractType
  {
    None,
    Tap,
    EnterDistance,
    Collide,
    HoverAndInput,
    UiInput
  }
  #endregion
  
  #region FACTION
  public interface IFactionHandler
  {
    int ResolveStance(string sourceID, string targetID);
  }
  #endregion
  #region SCENES
  [Serializable]
  public class Scene
  {
    public SceneReference Reference;
    public Sprite Icon;
    public string VisualName = "zone_xxx";
    public List<SpawnPoint> SpawnPoints = new List<SpawnPoint>();

    public string[] SpawnNames()
    {
      List<string> names = new List<string>();
      foreach (var spawn in SpawnPoints)
      {
        names.Add(spawn.VisualName);
      }
      return names.ToArray();
    }
  }
  
  [Serializable]
  public class SpawnPoint
  {
    public string VisualName;
    public string Description;
    public Sprite Icon;
    public Vector3[] Positions;
    public Vector3 Direction;

    public SpawnPoint(string spawnName)
    {
      VisualName = spawnName;
    }
  }
  #endregion
  
  #region FACTION
  [Serializable]
  public class FactionModel
  {
    public string Name = "New Faction";
    public Sprite Icon;
    public string Description;
        
    public int DefaultStance;
    public int StanceToSelf;
    [Serializable] public class ElementDictionary : SerializableDictionary<string, int> {}
    [SerializeField] public ElementDictionary Relations = new ElementDictionary();

   

    public int GetStance(string otherFactionId)
    {
      if (Relations.Contains(otherFactionId))
      {
        return Relations[otherFactionId];
      }

      return DefaultStance;
    }
  }
  
  [Serializable]
  public class FactionSettingModel
  {
    public List<FactionRank> Ranks = new List<FactionRank>();
    public string DefaultPlayerFaction;
      

    public bool MoveEntryUp(int index)
    { 
      if (index == 0) return false;

      var item = Ranks[index];
      Ranks.RemoveAt(index);
      Ranks.Insert(index - 1, item);
      return true;
    }
    
    public bool MoveEntryDown(int index)
    {
      if (index+1 == Ranks.Count)return false;

      var item = Ranks[index];
      Ranks.RemoveAt(index);
      Ranks.Insert(index + 1, item);
   
      return true;
    }
      
  }

  [Serializable]
  public class FactionRank
  {
    public string Name;
    public int Value;
  }
  #endregion
  
  #region SKILLS & ABILITIES
  [Serializable]
  public class SkillData
  {
    //INFO
    public string Name = "New Skill";
    public string Description;
    public Sprite Icon;

    //REQUIREMENTS
    public Requirement LearningRequirement;

    //RANKS
    public List<SkillLevel> Levels;

    public SkillData()
    {
      LearningRequirement= new Requirement();
      Levels= new List<SkillLevel>();
    }
  }

  #region [OTHER CLASSES]
  [Serializable]
  public class SkillLevel
  {
    public int PointToNext;
    public List<IdIntegerValue> AttributesBonuses = new List<IdIntegerValue>();
    public List<IdFloatValue> StatisticBonuses = new List<IdFloatValue>();
    public List<IdIntegerValue> VitalBonuses = new List<IdIntegerValue>();
    public List<int> AbilitiesGranted = new List<int>();
  }

  [Serializable]
  public class Requirement
  {
    public IntegerRestriction RaceRestriction;
    public IntegerRestriction ClassesRestriction;
    //minimum level
    public List<IdIntegerValue> AttributeRequirement;
    public List<IdIntegerValue> VitalRequirement;
    public List<IdFloatValue> StatRequirement;
    public List<IdIntegerValue> SkillRequirement;
  }

  [Serializable]
  public class IntegerRestriction
  {
    public enum RestrictionMode
    {
      Required,
      Restricted
    }

    public RestrictionMode Mode = RestrictionMode.Required;
    public List<int> Saved = new List<int>();

    public bool IsAllowed(int testedValue)
    {
      bool contain = Saved.Contains(testedValue);
      switch (Mode)
      {
        case RestrictionMode.Required:
          return contain;
        case RestrictionMode.Restricted:
          return !contain;
      }

      return false;
    }
  }

  [Serializable]
  public class IdIntegerValue
  {
    public int Id;
    public int Value;
    
    public IdIntegerValue(int id, int defaultAmount)
    {
      Id = id;
      Value = defaultAmount;
    }
  }
  
  [Serializable]
  public class RankedIdSave
  {
    public string Id;
    public int Rename;

    public RankedIdSave(string id, int defaultAmount)
    {
      Id = id;
      Rename = defaultAmount;
    }
  }
  
  [Serializable]
  public class RankedTimeIdSave
  {
    public string Id;
    public int Rank;
    public float CoolDown;
    
    public RankedTimeIdSave(string id, int rank, float cooldown)
    {
      Id = id;
      Rank = rank;
      CoolDown = cooldown;
    }
  }
  
  [Serializable]
  public class IdFloatValue
  {
    public int Id;
    public float Value;
    
    public IdFloatValue(int id, float defaultAmount)
    {
      Id = id;
      Value = defaultAmount;
    }
  }
  

  #endregion
  #endregion
  
  #region OTHER
  [Serializable]
  public class FloatingPopupModel
  {
    public string Name;
    public GameObject Prefab;
    public int PoolSize = 1;
    public ShowPopupRule Rule = new ShowPopupRule();
  }

  public enum IdentityType
  {
    MainPlayer,
    TeamMember,
    Npc,
    Pet
  }

  #endregion
  
  
}