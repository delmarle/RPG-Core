using System;
using System.Collections.Generic;

namespace Station
{
    public class BiDirectionalDictionary<T0, T1>
    {
        #region Public constructors

        public BiDirectionalDictionary()
        {
        }

        #endregion

        #region Override operator

        public T1 this[T0 i]
        {
            get
            {
                if (!_forwarDictionary.ContainsKey(i))
                {
                    throw new Exception("the key : " + i + " is not present in the dictionary");
                }
                else
                {
                    return _forwarDictionary[i];
                }
            }
        }

        #endregion

        #region Private properties

        IDictionary<T0, T1> _forwarDictionary = new Dictionary<T0, T1>();
        IDictionary<T1, T0> _backwardDictionary = new Dictionary<T1, T0>();

        #endregion

        #region Public properties

        public int Count
        {
            get { return _forwarDictionary.Count; }
        }

        public bool IsReadOnly { get; private set; }

        public ICollection<T0> Keys => _forwarDictionary.Keys;

        public ICollection<T1> Values => _forwarDictionary.Values;

        #endregion

        #region API

        public T1 GetValue(T0 k)
        {
            if (!_forwarDictionary.ContainsKey(k))
            {
                throw new Exception("the key : " + k + " is not present in the dictionary");
            }
            else
            {
                return _forwarDictionary[k];
            }
        }

        public T0 GetKey(T1 v)
        {
            if (!_backwardDictionary.ContainsKey(v))
            {
                throw new Exception("the value : " + v + " is not present in the dictionary");
            }
            else
            {
                return _backwardDictionary[v];
            }
        }

        public void Add(T0 key, T1 value)
        {
            if (_forwarDictionary.ContainsKey(key))
            {
                throw new Exception("The key : " + key + " exist already!");
            }
            else if (_backwardDictionary.ContainsKey(value))
            {
                throw new Exception("The value : " + value + " exist already!");
            }
            else
            {
                _forwarDictionary.Add(key, value);
                _backwardDictionary.Add(value, key);
            }
        }

        public void Add(KeyValuePair<T0, T1> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _forwarDictionary.Clear();
            _backwardDictionary.Clear();
        }

        public void CopyTo(KeyValuePair<T0, T1>[] array, int arrayIndex)
        {
            _forwarDictionary.CopyTo(array, arrayIndex);
        }

        public bool RemoveKey(T0 key)
        {
            if (_forwarDictionary.ContainsKey(key))
            {
                T1 value = _forwarDictionary[key];
                _forwarDictionary.Remove(key);
                return _backwardDictionary.Remove(value);
            }
            else
            {
                throw new Exception("the key : " + key + " is not present in the dictionary");
            }
        }

        public bool RemoveValue(T1 key)
        {
            if (_backwardDictionary.ContainsKey(key))
            {
                T0 value = _backwardDictionary[key];
                _forwarDictionary.Remove(value);
                return _backwardDictionary.Remove(key);
            }
            else
            {
                throw new Exception("the value : " + key + " is not present in the dictionary");
            }
        }

        public bool Remove(KeyValuePair<T0, T1> item)
        {
            if (_forwarDictionary.Contains(item))
            {
                _forwarDictionary.Remove(item);
                KeyValuePair<T1, T0> item2 = new KeyValuePair<T1, T0>(item.Value, item.Key);
                return _backwardDictionary.Remove(item2);
            }
            else
            {
                throw new Exception("the item : " + item.Key + " , " + item.Value +
                                    " is not present in the dictionary");
            }
        }

        public bool Contains(KeyValuePair<T0, T1> item)
        {
            return _forwarDictionary.Contains(item);
        }

        public bool ContainsKey(T0 key)
        {
            return _forwarDictionary.ContainsKey(key);
        }

        public bool ContainsValue(T1 key)
        {
            return _backwardDictionary.ContainsKey(key);
        }

        public bool TryGetValue(T0 key, out T1 value)
        {
            value = default(T1);
            if (_forwarDictionary.ContainsKey(key))
            {
                value = _forwarDictionary[key];
            }

            return false;
        }

        #endregion
    }
}