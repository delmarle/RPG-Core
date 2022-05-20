using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class CoreCharacter : MonoBehaviour
    {
        public CharacterUpdate OnDie;
        public delegate void CharacterUpdate(CoreCharacter character);
        #region METADATA

        private Dictionary<string, object> _meta = new Dictionary<string, object>();

        public object GetMeta(string key)
        {
            if (_meta.ContainsKey(key))
            {
                return _meta[key];
            }

            return "";
        }
        
        public T GetMeta<T>(string key)
        {
            if (_meta.ContainsKey(key))
            {
                return (T)_meta[key];
            }

            return default;
        }
        
        public void AddMeta(string key, object value)
        {
            if (_meta.ContainsKey(key))
            {
                _meta[key] = value;
            }
            else
            {
                _meta.Add(key, value);
            }
        }
        #endregion
    }

}
