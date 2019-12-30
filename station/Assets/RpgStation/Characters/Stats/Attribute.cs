namespace Station
{
  public class Attribute 
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
    
    private AttributeModel _staticData;
    public AttributeModel StaticData
    {
      get { return _staticData; }
      set { _staticData = value; }
    }

    private int _modifiedAmount;
    private int _statBonus;
    private int _equipmentBonus;
    private int _base;
    
    public int BaseValue 
    {
      get
      {
        _base = _character.Calculator.GetBaseAttribute(_staticData.Id);
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
    
    public int StatBonusAmount
    {
      get { return _statBonus; }
      set
      {
        _statBonus = value;
        DoChangeEvent();
      }
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
      get { return BaseValue + _statBonus + _equipmentBonus + _modifiedAmount; }
    }
    #endregion
    
    private void DoChangeEvent()
    {
      if(_character)
      {
        if(_character.OnAttributesUpdated != null) { _character.OnAttributesUpdated.Invoke(_character); }
        if(_character.OnVitalsUpdated != null) { _character.OnVitalsUpdated.Invoke(_character); }
        if(_character.OnStatisticUpdated != null) { _character.OnStatisticUpdated.Invoke(_character); }
      }
    }
  }
}
