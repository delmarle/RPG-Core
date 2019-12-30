using UnityEngine;

namespace Station
{
  [CreateAssetMenu]
    public class NpcCalculation : CharacterCalculation
    {
        private NpcModel _data;
        [SerializeField] private float _baseHitChance = 0;
        
        public void PreSetup(NpcModel classModel)
        {
            _data = classModel;
        }
        
         public override void OnSetup()
    {
      
      for (int i = 0; i < _attributesDb.Count(); i++)
      {
        _cachedBaseAttributes.Add(i, 0);
      }
    
      for (int i = 0; i < _statisticDb.Count(); i++)
      {
        _cachedBaseStatistics.Add(i, 0);
      }

      if (_data.UseHealth)
      {
        _cachedBaseVitals.Add(_data.HealthVital.Id, 100);
        _cachedBaseVitalsRegen.Add(_data.HealthVital.Id, 0f);
      }

      if (_data.UseSecondaryHealth)
      {
        _cachedBaseVitals.Add(_data.SecondaryHealthVital.Id, 0);
        _cachedBaseVitalsRegen.Add(_data.SecondaryHealthVital.Id, 0f);
      }

      foreach (var energyData in _data.EnergyVitals)
      {
        _cachedBaseVitals.Add(energyData.Id, 0);
        _cachedBaseVitalsRegen.Add(energyData.Id, 0f);
      }
    }

    #region [[ OVERRIDEN FUNCTIONS ]]

    public override void UpdateAttributes()
    {
      for (int i = 0; i < _cachedBaseAttributes.Count; i++)
      {
        //DEFAULT VALUE
        int value = 0;
    
        //RACE BONUS
        value  += _raceDb.GetEntry(_character.GetRace()).GetAttributeRaceBaseValue(i);
     
        //CLASS BONUS
        value += _data.GetAttributeBonus(i);
        
        _cachedBaseAttributes[i] = value;
      } 
    }

    public override void UpdateStatistics()
    {
      for (int i = 0; i < _cachedBaseStatistics.Count; i++)
      {
        //DEFAULT VALUE
        float value = 0;
        foreach (var attribute in _character.Stats.Attributes)
        {
          foreach (var statBonus in _attributesDb.GetEntry(attribute.Key).StatisticBonuses)
          {
            if (statBonus.Id == i)
            {
              value += statBonus.Value *attribute.Value.MaximumValue;
            }
          }
        }
        //RACE BONUS
        value  += _raceDb.GetEntry(_character.GetRace()).GetAttributeRaceBaseValue(i);
     
        //CLASS BONUS
        value += _data.GetStatsBonus(i);
     
        _cachedBaseStatistics[i] = value;
      } 
    }

    public override void UpdateVitals()
    {
      _cachedBaseVitals.Clear();
      _cachedBaseVitalsRegen.Clear();
      var raceData = _raceDb.GetEntry(_character.GetRace());
    
      if (_data.UseHealth) { BuildVital(_data.HealthVital.Id, raceData.GetVitalsBonus(_data.HealthVital.Id),_data.HealthVital.Value); }
      if (_data.UseSecondaryHealth) { BuildVital(_data.SecondaryHealthVital.Id, raceData.GetVitalsBonus(_data.SecondaryHealthVital.Id),_data.SecondaryHealthVital.Value); }
      foreach (var vital in _data.EnergyVitals) { BuildVital(vital.Id, raceData.GetVitalsBonus(vital.Id),vital.Value); }
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

