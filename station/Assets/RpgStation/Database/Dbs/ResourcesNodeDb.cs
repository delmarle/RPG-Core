using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Station
{
    [CreateAssetMenu(menuName = StationConst.BUILD_ASSET_CREATE_PATH+"Db/Resources nodes")]
    public class ResourcesNodeDb : DictGenericDatabase<ResourceNodeModel>
    {
        [Serializable] public class LocalDictionary : SerializableDictionary<string, ResourceNodeModel> {}
        [SerializeField] private LocalDictionary _db = new LocalDictionary();
  
        public override IDictionary<string, ResourceNodeModel> Db
        {
            get => _db;
            set => _db.CopyFrom (value);
        }


        public override string[] ListEntryNames()
        {
            return _db.Select(entry => entry.Value.Name.GetValue()).ToArray();
        }
        
        public override string ObjectName()
        {
            return "Resource node";
        }
    }
}

