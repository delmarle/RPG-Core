using System;
using System.Collections.Generic;

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

        private void OnDatabaseLoaded(ITemplateTask<Dictionary<Type, object>> arg1, Dictionary<Type, object> loaded, Exception arg3, object arg4)
        {
            _dbMap = loaded;
            GameGlobalEvents.OnDataBaseLoaded?.Invoke();
        }

        protected override void OnDispose()
        {
            
        }

        public T GetDb<T>()
        {
            if (_dbMap.ContainsKey(typeof(T)))
            {
                return (T)_dbMap[typeof(T)];
            }

            return default;
        }



    }

}
