using UnityEngine;

namespace Station
{
  public class Vital
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

    private VitalModel _staticData;
    public VitalModel StaticData
    {
      get { return _staticData; }
      set
      {
        _staticData = value;
      }
    }

    #region VALUE
    private int _current;
    private int _modifiedAmount;
    private int AttributesBonus
    {
      get { return _character.Calculator.GetVitalBonusFromAttributes(_staticData.Id); }
    }
    private int _equipmentBonus;
    private int _base;

    public int Current
    {
      get
      {
        return _current;
      }
    }

    public int BaseValue 
    {
      get
      {
        _base = _character.Calculator.GetBaseVital(_staticData.Id);
        return _base;
      }
    }

    public int ModifiedAmount
    {
      get { return _modifiedAmount; }
      set
      {
        _modifiedAmount = value;
        DoChangeEvent();
      }
    }
    
    public int AttributesBonusAmount
    {
      get { return AttributesBonus; }
    }
    
    public int EquipmentBonusAmount
    {
      get { return _equipmentBonus; }
      set
      {
        _equipmentBonus = value;
        DoChangeEvent();
      }
    }

    public int MaximumValue
    {
      get
      {
       // if(_staticData.Id ==0)
        //Debug.Log("BaseValue:"+BaseValue+" / AttributesBonus:"+AttributesBonus+" / _modifiedAmount:"+_modifiedAmount);

        return BaseValue + AttributesBonus + _equipmentBonus + _modifiedAmount;
      }
    }
    #endregion
    #region REGEN

    private int _baseRegenValue {  get { return _character.Calculator.GetBaseVitalRegen(_staticData.Id); }}
    private int _attributeRegenBonus= 0;
    private int _modifiedRegenBonus = 0;
    
    public int RegenValue
    {
      get { return _baseRegenValue + _attributeRegenBonus + _modifiedRegenBonus; }
    }

    #endregion
    #endregion

    public void SetCurrent(int value)
    {
      _current = value == -1? MaximumValue:value;
    }

    public void Increase(int value)
    {
      var maxVal = MaximumValue;
      if (_current != maxVal)
      {
        _current = Mathf.Clamp(_current + value, 0, maxVal);
        DoChangeEvent();
      }
    }

    public void Decrease(int value)
    {
      var maxVal = MaximumValue;
      _current = Mathf.Clamp(_current - value, 0, maxVal);
      DoChangeEvent();
    }

    private void DoChangeEvent()
    {
      if(_character && _character.OnVitalsUpdated != null) { _character.OnVitalsUpdated.Invoke(_character); }
    }

    public void RegenCycle()
    {
      Increase(RegenValue);
    }
  }
}
