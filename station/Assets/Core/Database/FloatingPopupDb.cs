using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Station
{
    [CreateAssetMenu(menuName = StationConst.BUILD_ASSET_CREATE_PATH+"Db/Floating popups")]
    public class FloatingPopupDb : DictGenericDatabase<FloatingPopupModel>
    {
        [Serializable] public class LocalDictionary : SerializableDictionary<string, FloatingPopupModel> {}
        [SerializeField] private LocalDictionary _db = new LocalDictionary();
        private Dictionary<string, FloatingPopupModel> _modelMap = new Dictionary<string, FloatingPopupModel>();
        public override IDictionary<string, FloatingPopupModel> Db
        {
            get => _db;
            set => _db.CopyFrom (value);
        }

        private void OnEnable()
        {
            _modelMap.Clear();
            foreach (var entry in _db.Values)
            {
                _modelMap.Add(entry.Name, entry);
            }
        }

        public FloatingPopupModel GetEntryByName(string entryName)
        {
            return _modelMap.ContainsKey(entryName)? _modelMap[entryName] : null;
        }

        public override string[] ListEntryNames()
        {
            return _db.Select(entry => entry.Value.Name).ToArray();
        }
        
        public override string ObjectName()
        {
            return "Popup";
        }
    }
}

