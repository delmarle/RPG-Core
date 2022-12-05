using System.Collections.Generic;


namespace Station
{
    public class CurrencyHandler
    {
        private Dictionary<string, long> currencies = new Dictionary<string, long>();
        public StationEvent<CurrencyChange,CurrencyModel,long, long> OnChanged = new StationEvent<CurrencyChange,CurrencyModel,long, long>();
        public enum CurrencyChange
        {
            Increase,Decrease
        }
        
        #region Save and LOAD

        public void Load(List<IdLongValue> currencyState)
        {
            if (currencyState != null)
            {
                foreach (var curr in currencyState)
                {
                    currencies.Add(curr.Id, curr.Value); 
                }
            }
        }

        public List<IdLongValue> GenerateSaveState()
        {
            var state = new List<IdLongValue>();
            foreach (var c in currencies)
            {
                state.Add(new IdLongValue(c.Key, c.Value));
            }
            return state;
        }
        #endregion
        
        #region Transaction

        public bool HasEnoughCurrency(CurrencyModel model, int requiredAmount)
        {
            if (currencies.ContainsKey(model.name))
            {
                return currencies[model.name] >= requiredAmount;
            }
            return false;
        }

        public void AddCurrency(CurrencyModel model, int amount, bool playerCurrencyEvent = false)
        {
            if (currencies.ContainsKey(model.name) == false)
            {
                currencies.Add(model.name, 0);
            }
            
            currencies[model.name] += amount;
            OnChanged.Invoke(CurrencyChange.Increase, model, currencies[model.name], amount);
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
            
            OnChanged.Invoke(CurrencyChange.Increase, null,0,0);
        }

        public void RemoveAllCurrencies()
        {
            currencies.Clear();
            OnChanged.Invoke(CurrencyChange.Decrease, null, 0, 0);
        }

        public void RemoveCurrency(CurrencyModel model, int removedAmount)
        {
            if (currencies.ContainsKey(model.name) == false)
            {
                //error
                return;
            }
            
            
            if (currencies[model.name] < removedAmount)
            {
                //error
                return;
            }
            
            currencies[model.name] -= removedAmount;
            OnChanged.Invoke(CurrencyChange.Decrease, model, currencies[model.name], removedAmount);
        }

        public long GetCurrencyAmount(CurrencyModel key)
        {
            if (currencies.ContainsKey(key.name))
            {
                return currencies[key.name];
            }

            return 0;
        }
        #endregion
    }

}
