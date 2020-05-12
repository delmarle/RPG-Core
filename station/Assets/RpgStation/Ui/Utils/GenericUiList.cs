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
        private readonly List<T2> _components;

        public List<T2> GetEntries() => _components;
        public GenericUiList(GameObject prefab, LayoutGroup list, int preWarmed = 1)
        {
            _prefab = prefab;
            _list = list;
            _items = new List<GameObject> {prefab};
            _components = new List<T2>{prefab.GetComponent<T2>()};
            prefab.SetActive(false);
            if (preWarmed > 1)
            {
                for (int i = 0; i < (preWarmed-1); i++)
                {
                    AddInstance();
                }
            }
        }

        private void AddInstance()
        {
            var instance = Object.Instantiate(_prefab, _list.transform, false);
            _items.Add(instance);
            _components.Add(instance.GetComponent<T2>());
            instance.SetActive(false);
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T2">Component to retrieve</typeparam>
        /// <param name="expression"></param>
        public void Iterate(Action<T2> expression)
        {
            foreach (var component in _components)
                expression.Invoke(component);
        }

        public T2 FindComponent(Func<T2, bool> condition)
        {
            return _components
                .Select(item => item)
                .FirstOrDefault(condition);
        }

        public void Generate(IEnumerable<T> items, Action<T, T2> transformer)
        {
            var index = 0;

            foreach (var item in items)
            {
                GameObject listItem;
                T2 listComponent;
                if (_items.Count > index)
                {
                    // We can use an item from pool
                    listItem = _items[index];
                    listComponent = _components[index];
                }
                else
                {
                    // We need to create a new item and add it to the pool
                    listItem = Object.Instantiate(_prefab, _list.transform, false);
                    listComponent = listItem.GetComponent<T2>();
                    _components.Add(listComponent);
                    _items.Add(listItem);
                    _components.Add(listComponent);
                }

                if (typeof(T2) == typeof(GameObject))
                    transformer.Invoke(item, listItem as T2);
                else
                    transformer.Invoke(item, listComponent);
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

        public GameObject GetObjectAt(int index)
        {
            if (_items.Count <= index)
                return null;

            return _items.ElementAt(index);
        }

        public T2 GetComponentAt(int index)
        {
            if (_components.Count <= index)
                return null;

            return _components.ElementAt(index);
        }
    }
}