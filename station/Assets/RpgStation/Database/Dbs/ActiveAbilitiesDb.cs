using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Station
{
    [CreateAssetMenu(menuName = StationConst.BUILD_ASSET_CREATE_PATH+"Db/Active abilities")]
    public class ActiveAbilitiesDb : DictGenericDatabase<ActiveAbility>
    {
        [Serializable] public class InteractionDictionary : SerializableDictionary<string, ActiveAbility> {}
        [SerializeField] private InteractionDictionary _db = new InteractionDictionary();
     
        public override IDictionary<string, ActiveAbility> Db
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
            return "Active Ability";
        }
    }
}



