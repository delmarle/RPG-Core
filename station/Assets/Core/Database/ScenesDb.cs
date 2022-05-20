using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Station
{
    [CreateAssetMenu(menuName = StationConst.BUILD_ASSET_CREATE_PATH+"Db/Scenes")]
    public class ScenesDb : DictGenericDatabase<Scene>
    {
        [Serializable] public class LocalDictionary : SerializableDictionary<string, Scene> {}
        [SerializeField] private LocalDictionary _db = new LocalDictionary();
  
        public override IDictionary<string, Scene> Db
        {
            get => _db;
            set => _db.CopyFrom (value);
        }


        public override string[] ListEntryNames()
        {
            return _db.Select(entry => entry.Value.VisualName).ToArray();
        }
        
        public override string ObjectName()
        {
            return "Scene";
        }
    }
}

