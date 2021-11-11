using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Station
{
    [CreateAssetMenu(menuName = StationConst.BUILD_ASSET_CREATE_PATH+"Db/Loot tables")]
    public class LootTableDb : DictGenericDatabase<LootTableModel>
    {
        [Serializable] public class LocalDictionary : SerializableDictionary<string, LootTableModel> {}
        [SerializeField] private LocalDictionary _db = new LocalDictionary();
  
        public override IDictionary<string, LootTableModel> Db
        {
            get => _db;
            set => _db.CopyFrom (value);
        }


        public override string[] ListEntryNames()
        {
            return _db.Select(entry => entry.Value.Description).ToArray();
        }
        
        public override string ObjectName()
        {
            return "Loot table";
        }
    }
}