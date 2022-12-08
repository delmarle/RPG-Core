using System;
using System.Collections;
using System.Collections.Generic;
using Station;
using UnityEngine;

public class CurrencyModel : ScriptableObject, IStationIcon
{
    public LocalizedText Description = new LocalizedText("description");
    #region BASE currency

    public string Key;
    public LocalizedText Name = new LocalizedText("currency");
    public Sprite Icon;
    public Sprite GetIcon() { return Icon; }

    #endregion
    
    public List<CurrencySubValue> SubValues = new List<CurrencySubValue>();
}

[Serializable]
public class CurrencySubValue
{
    public LocalizedText Name = new LocalizedText("currency");
    public Sprite Icon;


    public int PreviousEquivalent;
}
