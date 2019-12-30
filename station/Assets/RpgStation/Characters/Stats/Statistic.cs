
using UnityEngine;

namespace Station
{
  public class Statistic 
  {
    #region [[ FIELDS ]]
    private BaseCharacter _character;
    public BaseCharacter Character
    {
      get { return _character; }
      set { _character = value; }
    }
    
    private StatsHandler _stats;
    public StatsHandler Stats
    {
      get { return _stats; }
      set { _stats = value; }
    }
    
    private StatisticModel _model;
    public StatisticModel Model
    {
      get { return _model; }
      set { _model = value; }
    }

    private float _modifiedAmount;
    private float _attributesBonus;
    private float _equipmentBonus;
    private float _base;
    
    public float BaseValue 
    {
      get
      {
        _base = _character.Calculator.GetBaseStatistic(_model.Id);
        return _base;
      }
    }

    public float ModifiedAmount
    {
      get { return _modifiedAmount; }
      set
      {
        _modifiedAmount = value;
        DoChangeEvent();
      }
    }
    
    public float AttributesBonusAmount
    {
      get { return _attributesBonus; }
      set
      {
        _attributesBonus = value;
        DoChangeEvent();
      }
    }
    
    public float EquipmentBonusAmount
    {
      get { return _equipmentBonus; }
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
        float value = BaseValue + _attributesBonus + _equipmentBonus + _modifiedAmount;
        return Mathf.Clamp(value ,_model.MinimumValue, _model.MaximumValue);
      }
    }
    #endregion
    
    private void DoChangeEvent()
    {
      if(_character && _character.OnStatisticUpdated != null) { _character.OnStatisticUpdated.Invoke(_character); }
    }
  }
}
