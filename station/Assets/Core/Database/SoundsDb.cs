using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Station
{
    [CreateAssetMenu(menuName = StationConst.BUILD_ASSET_CREATE_PATH+"Db/Sounds")]
    public class SoundsDb : DictGenericDatabase<SoundCategory>
    {
        public List<SoundContainer> PersistentContainers = new List<SoundContainer>();
        [Serializable] public class ElementDictionary : SerializableDictionary<string, SoundCategory> {}
        [SerializeField] private ElementDictionary _db = new ElementDictionary();
    
        public override IDictionary<string, SoundCategory> Db
        {
            get => _db;
            set => _db.CopyFrom (value);
        }
        
        public override string ObjectName()
        {
            return "Sounds";
        }
        
        public override string[] ListEntryNames()
        {
            return _db.Select(entry => entry.Value.CategoryName).ToArray();
        }
    }
}