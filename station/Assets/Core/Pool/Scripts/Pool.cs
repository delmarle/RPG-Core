

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
	public class PooledItem : MonoBehaviour
	{
		private Dictionary<Type, object> _cachedComponent = new Dictionary<Type, object>();

		public object FindComponent(Type componentType)
		{
			if (_cachedComponent.ContainsKey(componentType))
			{
				return _cachedComponent[componentType];
			}
			else
			{
				var component = GetComponent(componentType);
				if (component != null)
				{
					_cachedComponent.Add(componentType, component);
					return component;
				}

				return null;
			}
		}
	}

	public class Pool: MonoBehaviour
	{
		private Transform _root;
		private Dictionary<PooledItem, ObjectPool<PooledItem>> _componentPools;
		private Dictionary<PooledItem, ObjectPool<PooledItem>> _instanceComponentPools;
		private Dictionary<GameObject, ObjectPool<GameObject>> _prefabsPools;
		private Dictionary<GameObject, ObjectPool<GameObject>> _instancePools;
	
		private const string OnSpawn = "OnSpawn";
		private const string OnDespawn = "OnDespawn";
		#region Mono

		private void Awake()
		{
			_root = transform;
			
			_componentPools = new Dictionary<PooledItem, ObjectPool<PooledItem>>();
			_instanceComponentPools = new Dictionary<PooledItem, ObjectPool<PooledItem>>();
			
			_prefabsPools = new Dictionary<GameObject, ObjectPool<GameObject>>();
			_instancePools = new Dictionary<GameObject, ObjectPool<GameObject>>();
		}

		#endregion
		#region POOLEDITEM VERSION
		private PooledItem InstantiatePrefab(PooledItem prefab)
		{
			var go = Instantiate(prefab);
			if (_root != null) go.transform.SetParent(_root);
			go.gameObject.SetActive(false);
			return go;
		}
		
		public void PopulatePool(PooledItem prefab, int size)
		{
			if (_componentPools.ContainsKey(prefab))
			{
				//already populated
			}

			var pool = new ObjectPool<PooledItem>(() => InstantiatePrefab(prefab), size);
			_componentPools[prefab] = pool;
		}

		
		public PooledItem SpawnObject(PooledItem prefab)
		{
			return SpawnObject(prefab, Vector3.zero, Quaternion.identity);
		}
		
		public PooledItem SpawnObject(PooledItem prefab, Vector3 position, Quaternion rotation, Transform parent = null)
		{
			if (!_componentPools.ContainsKey(prefab))
			{
				PopulatePool(prefab, 1);
			}

			var pool = _componentPools[prefab];

			var clone = pool.Create();
			if (parent != null)
				clone.transform.SetParent(parent);
			clone.transform.SetPositionAndRotation(position, rotation);

			clone.gameObject.SetActive(true);

			_instanceComponentPools.Add(clone, pool);
			clone.SendMessage(OnSpawn, SendMessageOptions.DontRequireReceiver);
			return clone;
		}

		public void DespawnObject(PooledItem clone)
		{
			if(clone.gameObject.activeSelf)
				clone.SendMessage(OnDespawn, SendMessageOptions.DontRequireReceiver);
			
			clone.gameObject.SetActive(false);
			if (clone.transform.parent != transform)
				clone.transform.SetParent(transform);

			if (_instanceComponentPools.ContainsKey(clone))
			{
				_instanceComponentPools[clone].Recycle(clone);
				_instanceComponentPools.Remove(clone);
			}
			else
			{
				Debug.LogWarning("No pool contains: " + clone.name);
			}
		}

		#endregion
#region GAMEOBJECT VERSION
		public void PopulatePool(GameObject prefab, int size)
		{
			if (_prefabsPools.ContainsKey(prefab))
			{
				//already populated
			}

			var pool = new ObjectPool<GameObject>(() => InstantiatePrefab(prefab), size);
			_prefabsPools[prefab] = pool;
		}

		public GameObject SpawnObject(GameObject prefab)
		{
			return SpawnObject(prefab, Vector3.zero, Quaternion.identity);
		}

		public GameObject SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
		{
			if (!_prefabsPools.ContainsKey(prefab))
			{
				PopulatePool(prefab, 1);
			}

			var pool = _prefabsPools[prefab];

			var clone = pool.Create();
			if (parent != null)
				clone.transform.SetParent(parent, false);
			
			clone.transform.SetPositionAndRotation(position, rotation);

			clone.SetActive(true);

			_instancePools.Add(clone, pool);
			clone.SendMessage(OnSpawn, SendMessageOptions.DontRequireReceiver);
			return clone;
		}

		public void DespawnObject(GameObject clone)
		{
			if(clone.activeSelf)
				clone.SendMessage(OnDespawn, SendMessageOptions.DontRequireReceiver);
			
			clone.SetActive(false);
			if (clone.transform.parent != transform)
				clone.transform.SetParent(transform);

			if (_instancePools.ContainsKey(clone))
			{
				_instancePools[clone].Recycle(clone);
				_instancePools.Remove(clone);
			}
			else
			{
				Debug.LogWarning("No pool contains: " + clone.name);
			}
		}


		private GameObject InstantiatePrefab(GameObject prefab)
		{
			var go = Instantiate(prefab);
			if (_root != null) go.transform.SetParent(_root);
			go.SetActive(false);
			return go;
		}
		
		#endregion
	}
}




