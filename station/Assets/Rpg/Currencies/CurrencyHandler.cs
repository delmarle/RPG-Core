using System.Collections.Generic;


namespace Station
{
    public class CurrencyHandler
    {
        private Dictionary<string, int> currencies = new Dictionary<string, int>();
        
        
        
        #region Save and LOAD

        public void Load(List<IdIntegerValue> currencyState)
        {
            if (currencyState != null)
            {
                foreach (var curr in currencyState)
                {
                    currencies.Add(curr.Id, curr.Value); 
                }
            }
        }

        public List<IdIntegerValue> GenerateSaveState()
        {
            var state = new List<IdIntegerValue>();
            foreach (var c in currencies)
            {
                state.Add(new IdIntegerValue(c.Key, c.Value));
            }
            return state;
        }
        #endregion
        
        #region Transaction

        public bool HasEnoughCurrency(string model, int requiredAmount)
        {
            if (currencies.ContainsKey(model))
            {
                return currencies[model] >= requiredAmount;
            }
            return false;
        }

        public void AddCurrency(string model, int amount, bool playerCurrencyEvent = false)
        {
            if (currencies.ContainsKey(model) == false)
            {
                currencies.Add(model, 0);
            }
            
            currencies[model] += amount;
        }
        
        public void AddCurrencies(Dictionary<string, int> addedCurrencies, bool playerCurrencyEvent = false)
        {
            foreach (var ac in addedCurrencies)
            {
                if (currencies.ContainsKey(ac.Key) == false)
                {
                    currencies.Add(ac.Key, 0);
                }
            
                currencies[ac.Key] += ac.Value;
            }
        }

        public void RemoveAllCurrencies()
        {
            currencies.Clear();
        }

        public void RemoveCurrency(string model, int removedAmount)
        {
            if (currencies.ContainsKey(model) == false)
            {
                //error
                return;
            }
            
            
            if (currencies[model] < removedAmount)
            {
                //error
                return;
            }
            
            currencies[model] -= removedAmount;
        }
        #endregion
    }

}
