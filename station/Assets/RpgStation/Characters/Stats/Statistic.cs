using UnityEngine;

namespace Station
{
  public class Statistic 
  {
    #region [[ FIELDS ]]

    public const string MOVEMENT_SPEED_ID = "movement_speed";
    public Statistic(BaseCharacter character, StatisticModel model)
    {
      _character = character;
      _model = model;
    }

    private BaseCharacter _character;
    private StatisticModel _model;
    private float _modifiedAmount;

    private float _equipmentBonus;
    private float _base;
    
    public float BaseValue 
    {
      get
      {
        _base = _character.Calculator.GetBaseStatistic(_model.Id) + _character.Calculator.GetVitalBonusFromAttributes(_model.Id);
        
        return _base;
      }
    }

    public float ModifiedAmount
    {
      get => _modifiedAmount;
      set
      {
        _modifiedAmount = value;
        DoChangeEvent();
      }
    }

    public float EquipmentBonusAmount
    {
      get => _equipmentBonus;
      set
      {
        _equipmentBonus = value;
        DoChangeEvent();
      }
    }

    public float MaximumValue
    {
      get
      {
        var value = BaseValue + _equipmentBonus + _modifiedAmount;
        return Mathf.Clamp(value ,_model.MinimumValue, _model.MaximumValue);
      }
    }
    #endregion
    
    private void DoChangeEvent()
    {
      if(_character) { _character.OnStatisticUpdated?.Invoke(_character); }
    }
  }
}
