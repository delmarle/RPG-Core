using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Station
{
    [CreateAssetMenu]
    public class StatisticDb  : DictGenericDatabase<StatisticModel>
    {
        [Serializable] public class StatisticDictionary : SerializableDictionary<string, StatisticModel> {}
        [SerializeField] private StatisticDictionary _db = new StatisticDictionary();
  
        public override IDictionary<string, StatisticModel> Db
        {
            get { return _db; }
            set { _db.CopyFrom (value); }
        }
    
        public override string[] ListEntryNames()
        {
            return _db.Select(entry => entry.Value.Name).ToArray();
        }
        

        
        public override string ObjectName()
        {
            return "satistic";
        }
    }
}

