using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Station
{
    [CreateAssetMenu(menuName = StationConst.BUILD_ASSET_CREATE_PATH+"Db/Items")]
    public class ItemsDb : DictGenericDatabase<BaseItemModel>
    {
        [Serializable] public class LocalDictionary : SerializableDictionary<string, BaseItemModel> {}
        [SerializeField] private LocalDictionary _db = new LocalDictionary();
        
        public override IDictionary<string, BaseItemModel> Db
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

