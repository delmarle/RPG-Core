using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiCurrencyWidget : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] _values;
    [SerializeField] private Image[] _icons;
    
    public void DisplayAmount(CurrencyModel model, long amount, bool showAll)
    {
        var vals = CurrenciesUtils.GetVisualRepresentation(model, amount);
        for (var index = 0; index < _values.Length; index++)
        {
            var text = _values[index];
            if (vals.Count < _values.Length)
            {
                text.text = String.Empty;
            }
            else
            {
                text.text = vals[index].ToString();
            }
        }
        
        for (var index = 0; index < _icons.Length; index++)
        {
            var icon = _icons[index];
            if (vals.Count < _icons.Length)
            {
                icon.gameObject.SetActive(false);
            }
            else
            {
                icon.sprite = index == 0 ? model.Icon : model.SubValues[index - 1].Icon;
                icon.gameObject.SetActive(true);
            }
        }
    }
    
    public void DisplayCost(CurrencyModel model, int amount, int cost)
    {
        
    }
}
