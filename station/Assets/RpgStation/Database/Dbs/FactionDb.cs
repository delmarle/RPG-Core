using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Station
{
    [CreateAssetMenu(menuName = StationConst.BUILD_ASSET_CREATE_PATH+"Db/Factions")]
    public class FactionDb : DictGenericDatabase<FactionModel>
    {
        [Serializable] public class ElementDictionary : SerializableDictionary<string, FactionModel> {}
        [SerializeField] private ElementDictionary _db = new ElementDictionary();
    
        public override IDictionary<string, FactionModel> Db
        {
            get => _db;
            set => _db.CopyFrom (value);
        }
        
        public override string ObjectName()
        {
            return "Faction";
        }
        
        public override string[] ListEntryNames()
        {
            return _db.Select(entry => entry.Value.Name).ToArray();
        }
    }

}

