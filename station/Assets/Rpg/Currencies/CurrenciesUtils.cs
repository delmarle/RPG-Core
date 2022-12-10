using System;
using System.Collections.Generic;
using Station;

public static class CurrenciesUtils 
{
    public static List<long> GetVisualRepresentation(CurrencyModel model,long amount)
    {
        List<long>tempValues = new List<long>();
        if (model == null)
        {
            return tempValues;
        }
        int subValuesCount = model.SubValues.Count;
       
        tempValues.Add(amount);
        foreach (var subValue in model.SubValues)
        {
            tempValues.Add(0);
        }

        int currentCurrencyIndex = 0;
        if (amount > 0)
        {
            for (int i = 0; i < subValuesCount; i++)
            {
                long initialNumberLength = tempValues[currentCurrencyIndex].Digits_IfChain();
                long dividedNumber = tempValues[currentCurrencyIndex] / model.SubValues[currentCurrencyIndex].PreviousEquivalent;
                if (dividedNumber > 0)
                {
                    long dividedNumberLength = dividedNumber.Digits_IfChain();
                    var numberFull = tempValues[currentCurrencyIndex];
                    var digits = GetDigits(numberFull, initialNumberLength - dividedNumberLength, initialNumberLength - dividedNumberLength);

                    tempValues[currentCurrencyIndex] = digits;
                    tempValues[currentCurrencyIndex + 1] = dividedNumber;
                    //string initialString = tempValues[currentCurrencyIndex].ToString();
                    //string str = initialString.Substring(initialString.Length-(int)initialNumberLength,(int)dividedNumberLength);
                    // Debug.Log($"difference {initialNumberLength - dividedNumberLength} digits {digits} removed = { str}");
                    // Debug.Log($"from {tempValues[currentCurrencyIndex]} divide {tempValues[currentCurrencyIndex] /  model.SubValues[currentCurrencyIndex].PreviousEquivalent}");
                }


                /*
                 //this was the original version that worked but was way too slow
                 while (tempValues[currentCurrencyIndex] >= model.SubValues[currentCurrencyIndex].PreviousEquivalent)
                 {
                     tempValues[currentCurrencyIndex] -= model.SubValues[currentCurrencyIndex].PreviousEquivalent;
                     tempValues[currentCurrencyIndex + 1]++;
                 } 
                 */
                //go to upper sub currency
                currentCurrencyIndex++;
            }
        }

        return tempValues;
    }
    
    public static long GetDigits( long number, long highestDigit, long numDigits)
    {
        return (number / (int)Math.Pow(10, highestDigit - numDigits)) % (int)Math.Pow(10, numDigits);
    }
    
    public static int GetDigits( int number, int highestDigit, int numDigits)
    {
        return (number / (int)Math.Pow(10, highestDigit - numDigits)) % (int)Math.Pow(10, numDigits);
    }

    public static void TransferCurrencies(BaseItemContainer containerToEmpty, BaseItemContainer containerToAdd)
    {
        var currencies = containerToEmpty.GetCurrencies;
        containerToAdd.AddCurrencies(currencies);
        containerToEmpty.RemoveAllCurrencies();
        
    }
}
