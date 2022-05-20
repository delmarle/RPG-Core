using System;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
	public class ObjectPool<T>
	{
		private readonly List<ObjectPoolContainer<T>> _list;
		private readonly Dictionary<T, ObjectPoolContainer<T>> _lookup;
		private readonly Func<T> _factoryFunc;
		private int _lastIndex;

		public ObjectPool(Func<T> factoryFunc, int initialSize)
		{
			_factoryFunc = factoryFunc;

			_list = new List<ObjectPoolContainer<T>>(initialSize);
			_lookup = new Dictionary<T, ObjectPoolContainer<T>>(initialSize);

			Warm(initialSize);
		}

		private void Warm(int capacity)
		{
			for (int i = 0; i < capacity; i++)
			{
				BuildPool();
			}
		}

		private ObjectPoolContainer<T> BuildPool()
		{
			var pool = new ObjectPoolContainer<T> {Item = _factoryFunc()};
			_list.Add(pool);
			return pool;
		}

		public T Create()
		{
			ObjectPoolContainer<T> container = null;
			for (int i = 0; i < _list.Count; i++)
			{
				_lastIndex++;
				if (_lastIndex > _list.Count - 1) _lastIndex = 0;
				
				if (_list[_lastIndex].Used == false)
				{
					container = _list[_lastIndex];
					break;
				}
			}

			if (container == null)
			{
				container = BuildPool();
			}

			container.Consume();
			_lookup.Add(container.Item, container);
			return container.Item;
		}

		public void Recycle(object item)
		{
			Recycle((T) item);
		}

		public void Recycle(T item)
		{
			if (_lookup.ContainsKey(item))
			{
				var container = _lookup[item];
				container.Release();
				_lookup.Remove(item);
			}
			else
			{
				Debug.LogWarning("pool is missing: " + item);
			}
		}

		public int Count
		{
			get { return _list.Count; }
		}

		public int CountUsedItems
		{
			get { return _lookup.Count; }
		}
	}
}
