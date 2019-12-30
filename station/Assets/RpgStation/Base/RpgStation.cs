using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Station
{
    public class RpgStation : MonoBehaviour
    {
        #region FIELDS
        public static GameObject RootGameObject;
        public static ThreadQueueSystem Aqm;
    
        private readonly Dictionary<Type, Component> _systemsMap = new Dictionary<Type, Component>();
        private static RpgStation _instance;
        #endregion
        #region Mono
        private void Awake()
        {
           Init();
           DontDestroyOnLoad(gameObject);
        }


        private void OnDestroy()
        {
            foreach (var value in _systemsMap.Select(entry => (BaseSystem) entry.Value).Where(value => value))
            {
                value.Dispose();
            }
        }
        #endregion
        

        private void Init()
        {
            _instance = this;
            Addressables.InitializeAsync();
            RootGameObject = gameObject;
            var list = ReflectionUtils.FindDerivedClasses(typeof(BaseSystem));
            //add to mapping
            foreach (var localType in list)
            {
                var addedComponent = gameObject.AddComponent(localType);
                _systemsMap.Add(localType, addedComponent);
            }
            //init each
            foreach (var system in _systemsMap.Select(entry => (BaseSystem) entry.Value))
            {
                system.Init(this);
            }
            Aqm = GetSystem<ThreadQueueSystem>();

        }

        public T GetSystem<T>() where T : BaseSystem
        {
            _systemsMap.TryGetValue(typeof(T), out var found);
            return (T)found;
        }
    
 
        public static T GetSystemStatic<T>() where T : BaseSystem
        {
            if (_instance == null)
            {
                return null;
            }

            _instance._systemsMap.TryGetValue(typeof(T), out var found);
            return (T)found;
        }

    }

}

