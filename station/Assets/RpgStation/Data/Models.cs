using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Station

{
    [Serializable] public class StringDictionary : SerializableDictionary<string, string> {}
    
    [Serializable]
    public class GameSettingsModel
    {
      public StationMechanics Mechanics;
      public BaseCharacterCreation CharacterCreation;
      
      //PARTY RELATED
      public int CharacterCreatedCount = 2;
      public int MaxTeamSize = 2;
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
      public string Id;
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
      public string Id;
      public string Description;
      public Sprite Icon;
      public int DefaultValue = 10;
    
      #region [[Optional]]
      public List<IdIntegerValue> VitalBonuses = new List<IdIntegerValue>();
      public List<IdFloatValue> StatisticBonuses = new List<IdFloatValue>();
      #endregion
    }

    [Serializable]
    public class GenderModel
    {
      public string Key;
      public LocalizedText Name = new LocalizedText("new gender");
      public LocalizedText Description= new LocalizedText("");
      public Sprite Icon;
    }
    
    [Serializable]
    public class RaceModel
    {
      public LocalizedText Name = new LocalizedText("new race");
      public LocalizedText Description= new LocalizedText("");
      public Sprite Icon;
    
      #region [[OPTIONAL]]
      public List<IdIntegerValue> AttributeBonuses = new List<IdIntegerValue>();
      public List<IdIntegerValue> VitalBonuses = new List<IdIntegerValue>();
    
      //TODO clear from editor
      private Dictionary<string, int> _attributesBonusesMap = new Dictionary<string, int>();
      private Dictionary<string, int> _vitalsBonusesMap = new Dictionary<string, int>();
    
      public int GetAttributeRaceBaseValue(string attributeId)
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
    
      public int GetVitalsBonus(string vitalId)
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

    #region CHARACTERS
    [Serializable]
    public class PlayerClassModel
    {
      public LocalizedText Name = new LocalizedText("new class");
      public LocalizedText Description;
      public Sprite Icon;
      public List<RaceMeta> AllowedRaces = new List<RaceMeta>();
      public bool UseHealth;
      public IdIntegerValue HealthVital = new IdIntegerValue("", 0);
      public bool UseSecondaryHealth;
      public IdIntegerValue SecondaryHealthVital = new IdIntegerValue("", 0);
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
      public FootSoundTemplate FootSoundTemplate;
      #region [[ RUNTIME CACHING ]]

      private Dictionary<string, int> _attributesBonusesMap = new Dictionary<string, int>();
      private Dictionary<string, float> _statisticsBonusesMap = new Dictionary<string, float>();
      private Dictionary<string, int> _vitalsBonusesMap = new Dictionary<string, int>();

      public int GetAttributeBonus(string attributeId)
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

      public int GetVitalsBonus(string vitalId)
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

      public float GetStatsBonus(string statId)
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


      public bool IsRaceAllowed(string raceId)
      {
        foreach (var raceMeta in AllowedRaces)
        {
          if (raceMeta.RaceId == raceId)
          {
            return true;
          }
        }

        return false;
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
      public CharacterBrain Brain;
      
      public bool UseHealth;
      public IdIntegerValue HealthVital = new IdIntegerValue("health", 0);
      public bool UseSecondaryHealth;
      public IdIntegerValue SecondaryHealthVital = new IdIntegerValue("health", 0);
      public List<IdIntegerValue> EnergyVitals = new List<IdIntegerValue>();
      public List<IdIntegerValue> AttributesBonuses = new List<IdIntegerValue>();
      public List<IdFloatValue> StatisticsBonuses = new List<IdFloatValue>();

      //faction
      //Ai
      //loot tables
      public string LootTable;
      //dialogue
      //shop tables
      //counter updates

      public List<ActiveAbility> OwnedAbilities = new List<ActiveAbility>();
    
      public CharacterCalculation StatsCalculator;
      public AttackData Attack = new AttackData();

      #region [[ RUNTIME CACHING ]]

      private Dictionary<string, int> _attributesBonusesMap = new Dictionary<string, int>();
      private Dictionary<string, float> _statisticsBonusesMap = new Dictionary<string, float>();
      private Dictionary<string, int> _vitalsBonusesMap = new Dictionary<string, int>();

      public int GetAttributeBonus(string attributeId)
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

      public int GetVitalsBonus(string vitalId)
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

      public float GetStatsBonus(string statId)
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
    #endregion
    [Serializable]
    public class StatisticModel
    {
        public string Id;
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
        SceneSystem sceneSystem = GameInstance.GetSystem<SceneSystem>();
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
      [FormerlySerializedAs("scene")] [FormerlySerializedAs("Destination")] public DestinationModel destination;
      public override ApplyEffectResult ApplyEffect(BaseCharacter source, BaseCharacter target)
      {
        GameGlobalEvents.OnBeforeLeaveScene?.Invoke();

        GameGlobalEvents.OnTriggerSceneSave?.Invoke();

        
        SceneSystem sceneSystem = GameInstance.GetSystem<SceneSystem>();
        var sceneDb = GameInstance.GetDb<ScenesDb>();
        if (sceneSystem.IsTraveling)
        {
          return ApplyEffectResult.Blocked;
        }
        
        var sceneData = sceneDb.GetEntry(destination.SceneId);
        var model = new TravelModel {SceneName = sceneData.VisualName};
        sceneSystem.InjectDestinationInSave(destination);
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
      FloatingPopupSystem.SpawnObject(ModifierType.ToString().ToLower(), EffectName,target.FloatingPopupAnchor, source, target, EffectIcon);
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
      FloatingPopupSystem.SpawnObject("heal", ((int)value).ToString(),target.FloatingPopupAnchor, source, target);
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
            FloatingPopupSystem.SpawnObject(dmgType, damageData.Amount.ToString(),target.FloatingPopupAnchor, source, target);
            target.Calculator.ReceiveDamage(damageData);
            return ApplyEffectResult.Success;
          }
          else if(hitRoll <= hitPower+targetEvadePower)
          {
            //target evaded
            FloatingPopupSystem.SpawnObject(FloatingPopupSystem.TYPE_EVADE, "Evade",target.FloatingPopupAnchor, source, target);
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
          FloatingPopupSystem.SpawnObject(FloatingPopupSystem.TYPE_MISS, "Miss",target.FloatingPopupAnchor, source, target);
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
    public HideHintOptions HideHintOptions = new HideHintOptions();
    public InteractType TryInteractMode;
    public float InteractionRange = 2;
    public HoverMode HoverMode;

    public ActionFxData actionFxData = new ActionFxData();
    public CancelInteractionMode CancelInteractionMode;
    public float CancelInteractionDistance = 2.5f;
    //NOTIFICATIONS
    public List<ScriptableNotificationChannel> FailNotificationChannels = new List<ScriptableNotificationChannel>();
    public List<ScriptableNotificationChannel> ResultNotificationChannels = new List<ScriptableNotificationChannel>();

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

  public enum CancelInteractionMode
  {
    None,
    ByDistance,
    ByMoving
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
        names.Add(spawn.VisualName.GetValue());
      }
      return names.ToArray();
    }
  }
  
  [Serializable]
  public class SpawnPoint
  {
    public LocalizedText VisualName;
    public LocalizedText Description = new LocalizedText("description");
    public Sprite Icon;
    public Vector3[] Positions;
    public Vector3 Direction;

    public SpawnPoint(string name)
    {
      VisualName = new LocalizedText(name);
    }
  }
  #endregion
  
  #region FACTION
  [Serializable]
  public class FactionModel
  {
    public LocalizedText Name = new LocalizedText("new faction");
    public Sprite Icon;
    public LocalizedText Description = new LocalizedText("");
        
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
    public string Id;
    public int Value;
    
    public IdIntegerValue(string id, int defaultAmount)
    {
      Id = id;
      Value = defaultAmount;
    }
  }
  [Serializable]
  public class RankProgression
  {
    public string Id;
    public int Rank;
    public int Progression;
  }
  
  [Serializable]
  public class RankedIdSave
  {
    public string Id;
    public int Rank;

    public RankedIdSave(string id, int defaultAmount)
    {
      Id = id;
      Rank = defaultAmount;
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
    public string Id;
    public float Value;
    
    public IdFloatValue(string id, float defaultAmount)
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
    public FloatingPopup Prefab;
    public int PoolSize = 1;
    public ShowPopupRule Rule = new ShowPopupRule();
    public bool Follow;
  }

  public enum IdentityType
  {
    MainPlayer,
    TeamMember,
    Npc,
    Pet
  }

  #endregion
  
  #region ITEMS
  [Serializable] public class ItemModelsDictionary : SerializableDictionary<string, BaseItemModel> {}
  [Serializable] public class ItemCategoryDictionary : SerializableDictionary<string, ItemCategory> {}
  
  [Serializable]
  public class ItemsSettingsModel
  {
    public List<string> ItemsTags = new List<string>();
    public ContainerSettings ContainerSettings = new ContainerSettings();
    public CraftSettings CraftSettings = new CraftSettings();
  }
  [Serializable]
  public class ItemRarity
  {
    public LocalizedText Name = new LocalizedText("Common");
    public Color32 Color = UnityEngine.Color.gray;
  }
  [Serializable]
  public class ItemCategory
  {
    public LocalizedText Name = new LocalizedText("category");
  }
  
  [Serializable]
  public class EquipmentSlotModel
  {
    public LocalizedText Name = new LocalizedText("slot");
    public LocalizedText Description = new LocalizedText("description");
    public Sprite Icon;
    public List<string> EquipmentTypeMatching = new List<string>();
  }
  
  [Serializable]
  public class EquipmentTypesModel
  {
    public LocalizedText Name = new LocalizedText("equipment type");
    public LocalizedText Description = new LocalizedText("description");
  }

  [Serializable]
  public class ContainerSettings
  {
    //player related
    public PlayerInventoryType PlayerInventoryType;
    public int InitialPlayerInventorySize = 4;
    //ui prefabs
    public UiPopup ContainerPopup;
    public UiPopup CharacterLootPopup;

    public LootInteractable LootInteractionPrefab;
  }

  public enum PlayerInventoryType
  {
    Shared,
    PerCharacter
  }

  [Serializable]
  public class CraftSettings
  {
  }

  [Serializable]
  public class BaseItemModel: ScriptableObject,IStationIcon
  {
    public LocalizedText Name = new LocalizedText("item");
    public LocalizedText Description = new LocalizedText("description");
    public Sprite Icon;
    public Sprite GetIcon() { return Icon; }
    public string RarityKey;
    public string CategoryKey;
    public int MaxStackSize = 1;

    public bool Stackable => MaxStackSize > 1;
    #region IMPLEMENTATION
    
    public virtual void OnUse(BaseCharacter user)
    {
      
    }

#if UNITY_EDITOR
    public virtual void DrawSpecificEditor()
    {
    }
    #endif
    #endregion
  }

  [Serializable]
  public class ResourceNodeModel: IStationIcon
  {
    public LocalizedText Name = new LocalizedText("resource node");
    public LocalizedText Description = new LocalizedText("contain some resources");
    public Sprite Icon;
    public Sprite GetIcon() => Icon;
    //requirement to collect
    public List<LootModel> Loots = new List<LootModel>();
    //mode
    public float CycleLength = 1;
    public ActionFxData FxData;

  }

  [Serializable]
  public class LootModel
  {
    public string ItemId;
    public int QuantityMin = 1;
    public int QuantityMax = 1;
    public float Chance = 100;
  }
  
  
  [Serializable]
  public class ChestNodeModel: IStationIcon
  {
    public LocalizedText Name = new LocalizedText("Chest");
    public LocalizedText Description = new LocalizedText("contain some items");
    public Sprite Icon;
    public Sprite GetIcon() => Icon;
    //requirement to collect
    public string LootTable;
    public AssetReference ReferencePrefab;
    public ChestNode Prefab;
    public SoundConfig OpenSound;
    public SoundConfig CloseSound;
    
  }
  
  [Serializable]
  public class LootTableModel
  {
    public string Description = "new loot table";
    public List<LootModel> Loots = new List<LootModel>();
  }
  #endregion
  
  #region LOCALIZATION
  [Serializable]
  public class LocalizedText
  {
    public LocalizedText(string defaultName)
    {
      Key = defaultName;
    }

    public string Key;

    public string GetValue()
    {
      return Key;
    }
  }

  #endregion
  #region GENERIC INTERFACE

  public interface IStationIcon
  {
    Sprite GetIcon();
  }

  #endregion
  
  #region SOUNDS
  [Serializable]
  public class SoundCategory
  {
    public string CategoryName;
  }

  [Serializable]
  public class SoundAddress
  {
    public string SoundContainerName;
  }

  [Serializable]
  public class SoundReferenceDrawer
  {
    public bool Enabled = false;
    public string GroupId;
    public string SoundId;
  }

  #endregion
  
}