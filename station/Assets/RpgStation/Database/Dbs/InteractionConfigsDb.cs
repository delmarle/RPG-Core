using System;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    [CreateAssetMenu]
    public class InteractionConfigsDb: DictGenericDatabase<InteractionConfig>
    {
        [Serializable] public class InteractionDictionary : SerializableDictionary<string, InteractionConfig> {}
        [SerializeField] private InteractionDictionary _db = new InteractionDictionary();
  
        public override IDictionary<string, InteractionConfig> Db
        {
            get { return _db; }
            set { _db.CopyFrom (value); }
        }
    
        public override string[] ListEntryNames()
        {
            var names = new List<string>();
            foreach (var entry in _db) { names.Add(entry.Value.InteractibleType.Type.Name); }
    
            return names.ToArray();
        }

        public override string ObjectName()
        {
            return "Interaction Config";
        }
    }
}

