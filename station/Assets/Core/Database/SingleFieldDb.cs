#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
namespace Station
{
    public abstract class SingleFieldDb<T> : BaseDb where T : class, new()
    {
        [SerializeField] protected T Database = new T();

        public T Get()
        {
            return Database;
        }
    }
}