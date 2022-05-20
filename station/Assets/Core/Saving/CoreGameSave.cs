using System;
using System.Collections.Generic;
using Station;
using UnityEngine;



namespace Station
{
    [Serializable]
    public partial class CoreGameSave
    {
        public GenericSaveDictionary<bool> BoolMap = new GenericSaveDictionary<bool>();
        public GenericSaveDictionary<string> StringMap = new GenericSaveDictionary<string>();
    }

    [Serializable]
    public class GenericSaveDictionary<T>
    {
        private Dictionary<string, T> _valueMap = new Dictionary<string, T>();

        public void SetValue(string key, T val)
        {
            if (_valueMap.ContainsKey(key))
            {
                _valueMap[key] = val;
            }
            else
            {
                _valueMap.Add(key, val);
            }
        }

        public T GetValue(string key, T defaultValue)
        {
            if (_valueMap.ContainsKey(key))
            {
                return _valueMap[key];
            }

            return defaultValue;
        }
    }
}
