namespace Station
{
  public class Attribute 
  {
    public Attribute(BaseCharacter character, AttributeModel model)
    {
      _character = character;
      _model = model;
    }
    
    #region [[ FIELDS ]]
    private readonly BaseCharacter _character;
    private readonly AttributeModel _model;
    private int _modifiedAmount;
    private int _statBonus;
    private int _equipmentBonus;
    private int _base;

    public AttributeModel Model => _model;

    public int BaseValue => _character.Calculator.GetBaseAttribute(_model.Id);

    public int ModifiedAmount
    {
      get => _modifiedAmount;
      set
      {
        _modifiedAmount = value;
        DoChangeEvent();
      }
    }
    
    public int StatBonusAmount
    {
      get => _statBonus;
      set
      {
        _statBonus = value;
        DoChangeEvent();
      }
    }
    
    public int EquipmentBonusAmount
    {
      get => _equipmentBonus;
      set
      {
        _equipmentBonus = value;
        DoChangeEvent();
      }
    }

    public int MaximumValue => BaseValue + _statBonus + _equipmentBonus + _modifiedAmount;

    #endregion
    
    private void DoChangeEvent()
    {
      if(_character)
      {
        _character.OnAttributesUpdated?.Invoke(_character);
        _character.OnVitalsUpdated?.Invoke(_character);
        _character.OnStatisticUpdated?.Invoke(_character);
      }
    }
  }
}
