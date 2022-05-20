using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class DbSystem : BaseSystem
    {
        private Dictionary<Type, object> _dbMap = new Dictionary<Type, object>();
        
        protected override void OnInit()
        {
            var dbsToLoad = GameInstance.GetDbToLoad;
            foreach (var dbAsset in dbsToLoad)
            {
                _dbMap.Add(dbAsset.GetType(), dbAsset);
            }

            StartCoroutine(SetupDbs());
        }

        IEnumerator SetupDbs()
        {
            yield return null;
            SetupSpecificDBs();
            GameGlobalEvents.OnDataBaseLoaded?.Invoke();
        }

        private void SetupSpecificDBs()
        {
            var settings = GameInstance.GetDb<GameSettingsDb>();
            settings.Get().CacheData();
        }
        
        protected override void OnDispose()
        {
            
        }

        protected override void OnDataBaseReady()
        {
            
        }

        public T GetDb<T>()
        {
            if (_dbMap != null)
            {
                if (_dbMap.ContainsKey(typeof(T)))
                {
                    return (T)_dbMap[typeof(T)];
                }
            }

         

            return default;
        }



    }

}
