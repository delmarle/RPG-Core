using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Station
{
    [CreateAssetMenu(menuName = StationConst.BUILD_ASSET_CREATE_PATH+"Db/Npc")]
    public class NpcDb : DictGenericDatabase<NpcModel>
    {
        [Serializable] public class ElementDictionary : SerializableDictionary<string, NpcModel> {}
        [SerializeField] private ElementDictionary _db = new ElementDictionary();
    
        public override IDictionary<string, NpcModel> Db
        {
            get => _db;
            set => _db.CopyFrom (value);
        }
        
        public override string ObjectName()
        {
            return "Npc";
        }
        
        public override string[] ListEntryNames()
        {
            return _db.Select(entry => entry.Value.Name).ToArray();
        }
    }

}

