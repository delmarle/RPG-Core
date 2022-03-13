using System;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class DbSystem : BaseSystem
    {
        private Dictionary<Type, object> _dbMap = new Dictionary<Type, object>();
        
        protected override void OnInit()
        {
            var founds = ReflectionUtils.FindAllDbTypes();
            LoadDatabasesTask loadTask = new LoadDatabasesTask(founds);
            loadTask.SetEndCallback(OnDatabaseLoaded);
            loadTask.Execute();
        }

        private void OnDatabaseLoaded(ITemplateTask<Dictionary<Type, object>> arg1, Dictionary<Type, object> loaded, Exception error, object arg4)
        {
            if (error != null)
            {
                Debug.LogError($" failed to load data base: {error.Message}");
            }
            else
            {
                _dbMap = loaded;
                BaseCharacter.CacheAllDb();
                SetupSpecificDBs();
                GameGlobalEvents.OnDataBaseLoaded?.Invoke();
            }
        }

        private void SetupSpecificDBs()
        {
            var settings = GameInstance.GetDb<GameSettingsDb>();
            settings.Get().CacheData();
        }
        
        protected override void OnDispose()
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
