using System;
using System.Collections.Generic;

namespace Station
{
    public class PlayerGeneralSave : SaveModule<PlayerGeneralData>
    {
        public override void FetchData()
        {
            if (Value == null) return;
            var playerInvSystem = GameInstance.GetSystem<PlayerInventorySystem>();
            Value.CurrenciesStatus = playerInvSystem.GetCurrenciesState();
        }
    }

    [Serializable]
    public class PlayerGeneralData
    {
        public List<IdIntegerValue> CurrenciesStatus = new List<IdIntegerValue>();
    }
}