using System.Collections.Generic;
using UnityEngine;

namespace Station
{
  [CreateAssetMenu]
public class CharacterCalculation : ScriptableObject
{
  #region [[ FIELDS ]]
  protected Dictionary<int, int> _cachedBaseAttributes = new Dictionary<int, int>();
  protected Dictionary<int, float> _cachedBaseStatistics = new Dictionary<int, float>();
  protected Dictionary<int, int> _cachedBaseVitals = new Dictionary<int, int>();
  protected Dictionary<int, float> _cachedBaseVitalsRegen = new Dictionary<int, float>();
  
  protected BaseCharacter _character;

  public delegate void CharacterEvent();

  public CharacterEvent OnMoving;
  private DbSystem _dbSystem;
  protected VitalsDb _vitalDb;
  protected AttributesDb _attributesDb;
  protected StatisticDb _statisticDb;
  protected RaceDb _raceDb;
  #endregion
  public  void Setup(BaseCharacter source)
  {
    _character = source;
    _dbSystem = RpgStation.GetSystemStatic<DbSystem>();
    _vitalDb = _dbSystem.GetDb<VitalsDb>();
    _attributesDb = _dbSystem.GetDb<AttributesDb>();
    _statisticDb = _dbSystem.GetDb<StatisticDb>();
    _raceDb = _dbSystem.GetDb<RaceDb>();
    
     OnSetup();
  }

  public virtual void OnSetup()
  {
  }

  public virtual void UpdateAttributes()
  {
    
  }
  
  public virtual void UpdateStatistics()
  {
   
  }

  public virtual void UpdateVitals()
  {
   
  }

  protected void BuildVital(int vitalId, int raceBonus, int classBonus)
  {
    _dbSystem = RpgStation.GetSystemStatic<DbSystem>();
    _vitalDb =  _dbSystem.GetDb<VitalsDb>();
    _attributesDb = _dbSystem.GetDb<AttributesDb>();
    _statisticDb = _dbSystem.GetDb<StatisticDb>();
    _cachedBaseVitals.Add(vitalId, raceBonus+classBonus);
    //REGEN
    CacheVitalRegen(vitalId);
  }

  private void CacheVitalRegen(int regenId)
  {
    float valueRegenBonus = 1;
    var data = _vitalDb.GetEntry(regenId);
    foreach (var attrBonus in data.AttributesBonuses)
    {
      var attributeTotal = _character.Stats.Attributes[attrBonus.Id].MaximumValue;
      valueRegenBonus += attrBonus.Value * attributeTotal;
    }
    _cachedBaseVitalsRegen.Add(regenId, valueRegenBonus);
  }

  #region [[ Getters ]] 

  public int GetBaseVitalRegen(int id)
  {
    
    return (int) (_cachedBaseVitalsRegen.ContainsKey(id)? _cachedBaseVitalsRegen[id] : 0);
  }

  public int GetBaseAttribute(int id)
  {
    return _cachedBaseAttributes.ContainsKey(id)? _cachedBaseAttributes[id] : 10;
  }

  public float GetBaseStatistic(int id)
  {
    return _cachedBaseStatistics.ContainsKey(id)? _cachedBaseStatistics[id] : 10f;
  }
  
  public int GetBaseVital(int id)
  {
    return _cachedBaseVitals.ContainsKey(id)? _cachedBaseVitals[id] : 100;
  }
  
  public int GetVitalBonusFromAttributes(int vitalId)
  {
    int bonus = 0;

    foreach (var attribute in _character.Stats.Attributes)
    {
      foreach (var vitalBonus in  attribute.Value.StaticData.VitalBonuses)
      {
        
        if (vitalBonus.Id == vitalId)
        {
          bonus += attribute.Value.MaximumValue * vitalBonus.Value;
        }
      }
    }
    
   
    return bonus;
  }
  #endregion
  #region [[ VIRTUAL FUNCTIONS ]] 

  public virtual float GetEvadePower()
  {
    return 0;
  }

  public virtual float GetBlockPower()
  {
    return 0;
  }

  public virtual float GetHitChance(float bonus)
  {
    return 50+bonus;
  }

  public virtual void ReceiveDamage(VitalChangeData data)
  {
    if (_character.IsDead)
    {
      return;
    }

   
    if (_character.Stats.Health.Current > 0)
    {
      _character.Stats.Health.Decrease((int)data.Amount);
      if (_character.OnDamaged != null)
      {
        _character.OnDamaged.Invoke(_character, data);
      }
        
      if(_character.Stats.Health.Current <= 0)
      {
        InvokeDie();
      }
    }
  }

  public virtual void Heal(VitalChangeData data)
  {
    if (_character.Stats != null)
    {
      _character.Stats.Health?.Increase(data.Amount);
    }
    
  }

  public VitalChangeData GetDamageCalculation(string damageType, float baseDamage, float baseCritical)
  {
    var data = new VitalChangeData(0, _character, false);

    switch (damageType)
    {
      case DamageEffect.MeleeDamageType:
        float criticalChance = baseCritical + GetCriticalChance(damageType);
        bool isCritical = Random.Range(0,100)> criticalChance;
      
        //use weapon to change range + speed + damage
        data.Amount = (int)baseDamage;
        
        if (isCritical)
        {
          data.Amount = (int)(data.Amount*1.2f);
        }

        return data;
        
      case "":
        break;
    }
    //skill bonus, equiped, calculation for magic/melee
    return data;
  }

  public float GetCriticalChance(string effectType)
  {
    return 5;
  }

  public float GetActionSpeedMultiplier(string actionType)
  {
    return 1;
  }

  #endregion
  public void InvokeMove()
  {
    OnMoving?.Invoke();
  }

  private void InvokeDie()
  {
    _character.IsDead = true;
  }

  public void Revive(float percent)
  {
    _character.IsDead = false;
    if (_character.Stats != null)
    {
      var value = _character.Stats.Health.MaximumValue * percent;
      _character.Stats.Health?.Increase((int)value);
    }
    _character.OnRevived?.Invoke(_character);
  }
}

}

