﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Station
{
    [CreateAssetMenu(menuName = StationConst.BUILD_ASSET_CREATE_PATH+"Db/Race")]
    public class RaceDb : DictGenericDatabase<RaceModel>
    {
        [Serializable] public class LocalDictionary : SerializableDictionary<string, RaceModel> {}
        [SerializeField] private LocalDictionary _db = new LocalDictionary();
  
        public override IDictionary<string, RaceModel> Db
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
            return "Race";
        }
    }
}

