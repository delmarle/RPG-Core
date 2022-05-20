using System.Linq;
using UnityEngine;


namespace Station
{
  [CreateAssetMenu]
  public class PlayerCalculations : CharacterCalculation
  {
    #region [[ FIELDS ]]

    private PlayerClassModel _data;
    [SerializeField] private float _baseHitChance = 0;
    #endregion

    public void PreSetup(PlayerClassModel classModel)
    {
      _data = classModel;
    }

    public override void OnSetup()
    {
      var raceData = _raceDb.GetEntry(_character.GetRaceID());
      _cachedBaseVitals.Clear();
      _cachedBaseVitalsRegen.Clear();

      foreach (var attribute in _attributesDb.Db)
      {
        _cachedBaseAttributes.Add(attribute.Key, 0);
      }

      foreach (var stat in _statisticDb.Db)
      {
        _cachedBaseStatistics.Add(stat.Key, 0);
      }

      if (_data.UseHealth)
      {
        BuildVital(_data.HealthVital.Id, raceData.GetVitalsBonus(_data.HealthVital.Id),_data.HealthVital.Value);
      }

      if (_data.UseSecondaryHealth)
      {
        BuildVital(_data.SecondaryHealthVital.Id, raceData.GetVitalsBonus(_data.SecondaryHealthVital.Id),_data.SecondaryHealthVital.Value);
      }

      foreach (var energyData in _data.EnergyVitals)
      {
        BuildVital(energyData.Id, raceData.GetVitalsBonus(energyData.Id),energyData.Value);
      }
    }
    
    
    
    #region [[ OVERRIDEN FUNCTIONS ]]

    public override void UpdateAttributes()
    {
      for (int i = 0; i < _cachedBaseAttributes.Count; i++)
      {
        string key = _cachedBaseAttributes.ElementAt(i).Key;
        //DEFAULT VALUE
        int value = 0;
    
        //RACE BONUS
        value  += _raceDb.GetEntry(_character.GetRaceID()).GetAttributeRaceBaseValue(key);
     
        //CLASS BONUS
        value += _data.GetAttributeBonus(key);
        
        //EQUIPMENT BONUS
        value += _character.GetEquipment.GetAttributeBonus(key);
        
        _cachedBaseAttributes[key] = value;
      }
    }



    public override void UpdateStatistics()
    {
      for (int i = 0; i < _cachedBaseStatistics.Count; i++)
      {
        string key = _cachedBaseStatistics.ElementAt(i).Key;
        //DEFAULT VALUE
        float value = 0;
        foreach (var attribute in _character.Stats.Attributes)
        {
          foreach (var statBonus in _attributesDb.GetEntry(attribute.Key).StatisticBonuses)
          {
            if (statBonus.Id.Equals(key))
            {
              value += statBonus.Value *attribute.Value.MaximumValue;
            }
          }
        }
        //RACE BONUS
        value  += _raceDb.GetEntry(_character.GetRaceID()).GetAttributeRaceBaseValue(key);
     
        //CLASS BONUS
        value += _data.GetStatsBonus(key);
        
        //EQUIPMENT BONUS
        value += _character.GetEquipment.GetStatsBonus(key);
     
        _cachedBaseStatistics[key] = value;
      }
    }


    public override void UpdateVitals()
    {
      _cachedBaseVitals.Clear();
      _cachedBaseVitalsRegen.Clear();
      var raceData = _raceDb.GetEntry(_character.GetRaceID());

      if (_data.UseHealth)
      {
        string healthId = _data.HealthVital.Id;
        var bonusHealth = _data.HealthVital.Value;
          
          ////EQUIPMENT BONUS
          bonusHealth += _character.GetEquipment.GetVitalBonus(healthId);
        BuildVital(healthId, raceData.GetVitalsBonus(healthId),bonusHealth);
      }

      if (_data.UseSecondaryHealth)
      {
        string secondHealthId = _data.SecondaryHealthVital.Id;
        var secondBonusHealth = _data.SecondaryHealthVital.Value;
        ////EQUIPMENT BONUS
        secondBonusHealth += _character.GetEquipment.GetVitalBonus(secondHealthId);
        BuildVital(secondHealthId, raceData.GetVitalsBonus(secondHealthId),secondBonusHealth);
      }

      foreach (var vital in _data.EnergyVitals)
      {
        var vitalBonus = vital.Value;
        ////EQUIPMENT BONUS
        vitalBonus += _character.GetEquipment.GetVitalBonus(vital.Id);

        BuildVital(vital.Id, raceData.GetVitalsBonus(vital.Id),vitalBonus);
      }
    }

    public override float GetHitChance(float bonus)
    {
      float hitChance = _baseHitChance+bonus;

      return hitChance;
    }

    public override float GetBlockPower()
    {
      return 1;
    }


    public override float GetEvadePower()
    {
      return 5f;
    }

    #endregion

  }

}

