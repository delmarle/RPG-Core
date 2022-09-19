using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Station
{
    [CreateAssetMenu(menuName = StationConst.BUILD_ASSET_CREATE_PATH+"Db/Currencies")]
    public class CurrenciesDb : DictGenericDatabase<CurrencyModel>
    {
        [Serializable] public class LocalDictionary : SerializableDictionary<string, CurrencyModel> {}
        [SerializeField] private LocalDictionary _db = new LocalDictionary();
        
        public override IDictionary<string, CurrencyModel> Db
        {
            get => _db;
            set => _db.CopyFrom (value);
        }


        public override string[] ListEntryNames()
        {
            return _db.Select(entry => entry.Value?.Name?.GetValue()).ToArray();
        }

        protected override void OnBeforeDelete(string key)
        {
            base.OnBeforeDelete(key);
#if UNITY_EDITOR
            var objToDelete = GetEntry(key);
            string pathToDelete = AssetDatabase.GetAssetPath(objToDelete);      
            AssetDatabase.DeleteAsset(pathToDelete);
#endif
        }
    }
}