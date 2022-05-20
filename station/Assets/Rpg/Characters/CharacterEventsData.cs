namespace Station
{

  public struct VitalChangeData
  {
    public int Amount;
    public BaseCharacter Source;
    public bool IsCritical;

    public VitalChangeData(int amount, BaseCharacter source, bool isCritical = false)
    {
      Amount = amount;
      Source = source;
      IsCritical = isCritical;
    }
  }
}

