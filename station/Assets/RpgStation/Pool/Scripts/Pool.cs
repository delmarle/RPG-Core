

using System.Collections.Generic;
using UnityEngine;

namespace Station
{
	public class Pool : MonoBehaviour
	{
		private Transform _root;

		private Dictionary<GameObject, ObjectPool<GameObject>> _prefabsPools;
		private Dictionary<GameObject, ObjectPool<GameObject>> _instancePools;
		private const string OnSpawn = "OnSpawn";
		private const string OnDespawn = "OnDespawn";
		#region Mono

		private void Awake()
		{
			_root = transform;
			_prefabsPools = new Dictionary<GameObject, ObjectPool<GameObject>>();
			_instancePools = new Dictionary<GameObject, ObjectPool<GameObject>>();
		}

		#endregion

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
				clone.transform.SetParent(parent);
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
	}
}




