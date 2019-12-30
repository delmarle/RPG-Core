using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Station
{
    [CreateAssetMenu]
    public class AttributesDb : DictGenericDatabase<AttributeModel>
    {
        [Serializable] public class LocalDictionary : SerializableDictionary<string, AttributeModel> {}
        [SerializeField] private LocalDictionary _db = new LocalDictionary();
  
        public override IDictionary<string, AttributeModel> Db
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
            return "Attributes";
        }
    }
}

