using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Station
{
    [CreateAssetMenu(menuName = StationConst.BUILD_ASSET_CREATE_PATH+"Db/Vitals")]
    public class VitalsDb : DictGenericDatabase<VitalModel>
    {
        [Serializable] public class LocalDictionary : SerializableDictionary<string, VitalModel> {}
        [SerializeField] private LocalDictionary _db = new LocalDictionary();
  
        public override IDictionary<string, VitalModel> Db
        {
            get => _db;
            set => _db.CopyFrom (value);
        }


        public override string[] ListEntryNames()
        {
            return _db.Select(entry => entry.Value.Name).ToArray();
        }
        
        public override string ObjectName()
        {
            return "vital";
        }
    }
}

