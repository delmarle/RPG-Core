
using UnityEngine;

namespace Station
{
    public class PoolSystem : BaseSystem
    {
        private static PoolSystem Instance;
        private Pool _pool;
        
        protected override void OnInit()
        {
            Instance = this;
            var canvas = transform.Find("pool");
            _pool = canvas.gameObject.AddComponent<Pool>();
        }

        protected override void OnDispose()
        {
    
        }

        protected override void OnDataBaseReady()
        {
     
        }

        #region POOLEDITEM
        public static void PopulatePool(PooledItem prefab, int size)
        {
            Instance._pool.PopulatePool(prefab, size);
        }

        public static T Spawn<T>(PooledItem prefab)
        {
            var item = Spawn(prefab);
            var component = item.FindComponent(typeof(T));
            var casted = (T) component;
            return casted;
        }
        public static T Spawn<T>(PooledItem prefab, Vector3 position, Quaternion rotation)
        {
            var item = Spawn(prefab, position, rotation);
            var component = item.FindComponent(typeof(T));
            var casted = (T) component;
            return casted;
        }

        public static PooledItem Spawn(PooledItem prefab)
        {
            return Instance == null ? null : Instance._pool.SpawnObject(prefab);
        }
        
        public static PooledItem Spawn(PooledItem prefab, Vector3 position, Quaternion rotation)
        {
            return Instance == null ? null : Instance._pool.SpawnObject(prefab, position, rotation);
        }
        
        public static PooledItem Spawn(PooledItem prefab, Vector3 position, Quaternion rotation,Transform parent)
        {
            if (Instance == null) return null;
            return  Instance._pool.SpawnObject(prefab, position, rotation, parent);
        }

        public static void Despawn(PooledItem clone)
        {
            if (Instance == null) return;
            Instance._pool.DespawnObject(clone);
        }

        public static void Despawn(PooledItem clone,float time)
        {
            if (Instance == null) return;
            Timer.Register(time, () => Instance._pool.DespawnObject(clone));
        }

        #endregion
#region GAMEOBJECT
        public static void PopulatePool(GameObject prefab, int size)
        {
            Instance._pool.PopulatePool(prefab, size);
        }

        public static GameObject Spawn(GameObject prefab)
        {
            return Instance == null ? null : Instance._pool.SpawnObject(prefab);
        }

        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return Instance == null ? null : Instance._pool.SpawnObject(prefab, position, rotation);
        }
        
        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation,Transform parent)
        {
            if (Instance == null) return null;
            return  Instance._pool.SpawnObject(prefab, position, rotation, parent);
        }

        public static void Despawn(GameObject clone)
        {
            if (Instance == null) return;
            Instance._pool.DespawnObject(clone);
        }

        public static void Despawn(GameObject clone,float time)
        {
            if (Instance == null) return;
            Timer.Register(time, () => Instance._pool.DespawnObject(clone));
        }
        
        #endregion
    }
}

