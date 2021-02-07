using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Station
{
    [CreateAssetMenu(menuName = StationConst.BUILD_ASSET_CREATE_PATH+"Db/Skills")]
    public class SkillDb : DictGenericDatabase<SkillData> 
    {
        [Serializable] public class GenericDictionary : SerializableDictionary<string, SkillData> {}
        [SerializeField] private GenericDictionary _db = new GenericDictionary();
    
        public override IDictionary<string, SkillData> Db
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
            return "Skill";
        }
    }
}
