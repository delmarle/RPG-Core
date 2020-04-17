using System.Collections.Generic;
using UnityEngine;

namespace Station
{
  public class StatsHandler : MonoBehaviour 
  {
    #region [[ FIELDS ]

    private DbSystem _dbSystem;
    private AttributesDb _attributesDb;
    private StatisticDb _statisticDb;
    private VitalsDb _vitalsDb;
    private string _movementSpeedId = Statistic.MOVEMENT_SPEED_ID;
    
    private List<RuntimeModifier> _modifierToRecycle = new List<RuntimeModifier>();
    private BaseCharacter _character;
   
    private Dictionary<string, Attribute> _attributes = new Dictionary<string, Attribute>();
    
    public Dictionary<string, Attribute> Attributes => _attributes;

    private Dictionary<string, Statistic> _statistics = new Dictionary<string, Statistic>();
    
    public Dictionary<string, Statistic> Statistics => _statistics;

    public Statistic MovementSpeed => _statistics[_movementSpeedId];

    #region VITALS
    
    private string _healthId;

    public Vital Health => _vitals[_healthId];

    private string _secondaryHealthId;

    public Vital SecondaryHealth => _vitals[_secondaryHealthId];

    private Dictionary<string, Vital> _vitals = new Dictionary<string, Vital>();
    
    public Dictionary<string, Vital> Vitals => _vitals;

    private List<Timer> _regenTimers = new List<Timer>();
    
    public Dictionary<string, RuntimeModifier>  Modifiers => _currentModifier;
    private Dictionary<string, RuntimeModifier> _currentModifier = new Dictionary<string, RuntimeModifier>();
    public delegate void OnModifierUpdated(RuntimeModifier modifier, BaseCharacter source);
    public OnModifierUpdated OnModifierAdded;
    public OnModifierUpdated OnModifierRemoved;
    
    #endregion

    #endregion
    #region [[ SETUP ]]

    public void Setup(BaseCharacter source, IdIntegerValue health, IdIntegerValue secondaryHealth, IdIntegerValue[] energies)
    {
      _dbSystem = RpgStation.GetSystemStatic<DbSystem>();
      _attributesDb = _dbSystem.GetDb<AttributesDb>();
      _statisticDb = _dbSystem.GetDb<StatisticDb>();
      _vitalsDb = _dbSystem.GetDb<VitalsDb>();
      _character = source;
      BuildAttributes(source);
      BuildStatistics(source);
      BuildVitals(source, health, secondaryHealth, energies);
      _character.Calculator.UpdateAttributes();
      _character.Calculator.UpdateVitals();
      _character.Calculator.UpdateStatistics();
      source.OnDie+= OnDie;
    }

    private void BuildAttributes(BaseCharacter source)
    {
   
      foreach (var attr in _attributesDb.Db)
      {
        var attributeInstance = new Attribute(source, _attributesDb.GetEntry(attr.Key));
        _attributes.Add(attr.Value.Id, attributeInstance);
      }
    }

    private void BuildStatistics(BaseCharacter source)
    {
      foreach (var staticStat in _statisticDb)
      {
        var stat = new Statistic(source, _statisticDb.GetEntry(staticStat.Id));

        _statistics.Add(staticStat.Id, stat);
      }
    }

    public void BuildVitals(BaseCharacter source,IdIntegerValue healthVital, IdIntegerValue secondaryHealthVital, IdIntegerValue[] energyVitals)
    {
      //set primary health
      if (healthVital!= null)
      {
        var healthAmount  = new Vital(source, _vitalsDb.GetEntry(healthVital.Id));
        SetupVitalEntry(VitalModel.VitalType.PrimaryHealth, healthAmount , healthVital.Id);
        
        
        if (secondaryHealthVital!= null)
        {
          var secondaryHealthAmount  =  new Vital(source, _vitalsDb.GetEntry(secondaryHealthVital.Id));
          SetupVitalEntry(VitalModel.VitalType.PrimaryHealth, secondaryHealthAmount , secondaryHealthVital.Id);
        }
      }
      
      foreach (var energyData in energyVitals)
      {
        var vital = new Vital(source, _vitalsDb.GetEntry(energyData.Id));
        SetupVitalEntry(VitalModel.VitalType.Energy,vital, energyData.Id);
      }
    }

    private void SetupVitalEntry(VitalModel.VitalType type,Vital vital, string id)
    {
      switch (type)
      {
        case VitalModel.VitalType.PrimaryHealth:
          _healthId = id;
          break;
        case VitalModel.VitalType.SecondaryHealth:
          _secondaryHealthId = id;
          break;
      }
      _vitals.Add(id,vital);
      vital.RegenCycle();
      var timer = Timer.Register(vital.Model.RegenCycle, () => 
      {
        vital.RegenCycle();
      }, null, true);
      _regenTimers.Add(timer);
    }

    public void SetVitalsValue(List<IdIntegerValue> vitalsValues)
    {
      if (vitalsValues == null) return;
      Dictionary<string,int> vitalsMap = new Dictionary<string, int>();
      foreach (var vitals in vitalsValues)
      {
        vitalsMap.Add(vitals.Id,vitals.Value);
      }

      foreach (var vital in _vitals)
      {
        if (vitalsMap.ContainsKey(vital.Key))
        {
          int vitalValue = vitalsMap[vital.Key];
          vital.Value.SetCurrent(vitalValue);
        }
      }
    }

    public void SetVitalsFull()
    {
      foreach (var v in _vitals)
      {
        v.Value.SetFull();
      }
    }

    #endregion

    private void OnDestroy()
    {
      if(_character == null) return;
      _character.OnDie -= OnDie;
      Destroy(_character.Calculator);
    }

    #region MODIFIER
    public void ReceiveModifier(ModifierEffect modifier, BaseCharacter source)
    {
      //TODO handle aggro from source
      RuntimeModifier currentEffect;
      if (_currentModifier.ContainsKey(modifier.Identifier))
      {
         currentEffect = _currentModifier[modifier.Identifier];
        if (currentEffect.CurrentStack >= modifier.MaxStack)
        {
          //refresh effect
          currentEffect = new RuntimeModifier(modifier,modifier.MaxStack);
        }
        else
        {
          //increase stack
          currentEffect.CurrentStack++;
          currentEffect.TimeLeft = modifier.Length;
        }

        _currentModifier[modifier.Identifier] = currentEffect;
        
      }
      else
      {
        //add effect
        currentEffect = new RuntimeModifier(modifier, 1);
        _currentModifier.Add(modifier.Identifier, currentEffect);
      }
      ModifierQueue.AddCharacter(this);
      ResetModifiedStats();
      ApplyModifiedStats();
      OnModifierAdded?.Invoke(currentEffect, source);
    }
   
    public void CycleModifier()
    {
      _modifierToRecycle.Clear();
   
      foreach (var buff in _currentModifier)
      {
        buff.Value.DecreaseTimer();
        if (buff.Value.IsOutOfTime())
        {
          _modifierToRecycle.Add(buff.Value);
        }
      }
      
      //delete all cached modifier
      foreach (var buff in _modifierToRecycle)
      {
        buff.Modifier.RemoveBonusToHandler(this, buff.CurrentStack);
        _currentModifier.Remove(buff.Modifier.Identifier);
        OnModifierRemoved?.Invoke(buff, null);
      }
      
      //remove if no buffs left
      if (_currentModifier.Count == 0)
      {
        ModifierQueue.RemoveCharacter(this);
      }
    }

    private void ResetModifiedStats()
    {
      //reset values of all stats
      foreach (var att in _attributes)
      {
        att.Value.ModifiedAmount = 0;
      }

      foreach (var vital in _vitals)
      {
        vital.Value.ModifiedAmount = 0;
      }

      foreach (var stat in _statistics)
      {
        stat.Value.ModifiedAmount = 0;
      }
    }

    private void ApplyModifiedStats()
    {
      foreach (var buff in _currentModifier)
      {
        buff.Value.Modifier.AddBonusToHandler(this, buff.Value.CurrentStack);
      }

      //invoke attribute event to update stats & vitals too
      if(_character.OnAttributesUpdated != null)_character.OnAttributesUpdated.Invoke(_character);
    }
    //on reduce damaged
    //on die
    //on reduce energy
    //on stat changed
    private void OnDie(BaseCharacter character)
    {
      foreach (var timer in _regenTimers)
      {
        timer.Pause();
      }
    }

  }
  #endregion
  
  
}