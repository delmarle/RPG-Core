using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Station
{
    /// <summary>
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    /// <typeparam name="T2">Item component type</typeparam>
    public class GenericUiList<T, T2> where T2 : class
    {
        private readonly LayoutGroup _list;
        private readonly GameObject _prefab;

        private readonly List<GameObject> _items;

        public GenericUiList(GameObject prefab, LayoutGroup list)
        {
            _prefab = prefab;
            _list = list;
            _items = new List<GameObject> {prefab};

            // Disable a prefab
            prefab.SetActive(false);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T2">Component to retrieve</typeparam>
        /// <param name="expression"></param>
        public void Iterate(Action<T2> expression)
        {
            foreach (var item in _items)
                expression.Invoke(item.GetComponent<T2>());
        }

        public T2 FindComponent(Func<T2, bool> condition)
        {
            return _items
                .Select(item => item.GetComponent<T2>())
                .FirstOrDefault(condition);
        }

        public void Generate(IEnumerable<T> items, Action<T, T2> transformer)
        {
            var index = 0;

            foreach (var item in items)
            {
                GameObject listItem;

                if (_items.Count > index)
                {
                    // We can use an item from pool
                    listItem = _items[index];
                }
                else
                {
                    // We need to create a new item and add it to the pool
                    listItem = Object.Instantiate(_prefab);
                    listItem.transform.SetParent(_list.transform, false);
                    _items.Add(listItem);
                }

                if (typeof(T2) == typeof(GameObject))
                    transformer.Invoke(item, listItem as T2);
                else
                    transformer.Invoke(item, listItem.GetComponent<T2>());
                listItem.SetActive(true);
                index++;
            }

            while (_items.Count > index)
            {
                // Disable any unnecessary objects from pool
                _items[index].gameObject.SetActive(false);
                index++;
            }
        }

        //public void Generate(IEnumerable<T> items, Action<T, GameObject> transformer)
        //{
        //    Generate<GameObject>(items, transformer);
        //}

        public GameObject GetObjectAt(int index)
        {
            if (_items.Count <= index)
                return null;

            return _items.ElementAt(index);
        }
    }


    /// <summary>
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    public class GenericUiList<T>
    {
        private readonly LayoutGroup _list;
        private readonly GameObject _prefab;

        private readonly List<GameObject> _items;
     
        public GenericUiList(GameObject prefab, LayoutGroup list)
        {
            _prefab = prefab;
            _list = list;
            _items = new List<GameObject> {prefab};

            // Disable a prefab
            prefab.SetActive(false);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T2">Component to retrieve</typeparam>
        /// <param name="expression"></param>
        public void Iterate<T2>(Action<T2> expression) where T2 : class
        {
            foreach (var item in _items)
                expression.Invoke(item.GetComponent<T2>());
        }

        public T2 FindObject<T2>(Func<T2, bool> condition)
        {
            foreach (var item in _items)
            {
                var component = item.GetComponent<T2>();
                if (condition(component))
                    return component;
            }
            return default(T2);
        }

        public void Generate<T2>(IEnumerable<T> items, Action<T, T2> transformer) where T2 : class
        {
            var index = 0;

            foreach (var item in items)
			{
                GameObject listItem;
                if (_items.Count > index)
                {
                    // We can use an item from pool
                    listItem = _items[index];
                }
                else
                {
                    // We need to create a new item and add it to the pool
                    listItem = Object.Instantiate(_prefab, _list.transform);
                
                    _items.Add(listItem);
                }
                
                if (typeof(T2) == typeof(GameObject))
                    transformer.Invoke(item, listItem as T2);
                else
                    transformer.Invoke(item, listItem.GetComponent<T2>());
                listItem.SetActive(true);
                index++;
            }

            while (_items.Count > index)
            {
                // Disable any unnecessary objects from pool
                _items[index].gameObject.SetActive(false);
                index++;
            }
        }

        public void Generate(IEnumerable<T> items, Action<T, GameObject> transformer)
        {
            Generate<GameObject>(items, transformer);
        }

        public GameObject GetObjectAt(int index)
        {
            return _items.ElementAt(index);
        }
    }
}