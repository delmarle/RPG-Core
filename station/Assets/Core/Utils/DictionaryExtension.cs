using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    
    public static class DictionaryExtension 
    {
        public static TU Get<T, TU>(this IDictionary<T, TU> dictionary, T key)
        {
            TU value = default(TU);
            dictionary.TryGetValue(key, out value);
            return value;
        }
    }
}

