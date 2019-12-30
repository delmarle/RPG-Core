using System;
using System.Collections;
using System.Collections.Generic;
using Station;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace Station
{
    public class LoadDatabasesTask : BasicTask<Dictionary<Type, object>>
    {
        private Dictionary<Type, object> _cachedConfigs = new Dictionary<Type, object>();
        private List<Type> TypesToLoad;
        
        public LoadDatabasesTask(List<Type> dbTypes)
        {
            TypesToLoad = dbTypes;
           
        }

        protected override IEnumerator HandleExecute()
        {
            Debug.Log(TypesToLoad.Count+ " Dbs will be loaded");
            foreach (var typeToLoad in TypesToLoad)
            {
                AsyncOperationHandle<ScriptableObject> handle = Addressables.LoadAssetAsync<ScriptableObject>(typeToLoad.Name);
                handle.Completed += HandleOnCompleted;
               
                yield return handle;
            }
        }

        private void HandleOnCompleted(AsyncOperationHandle<ScriptableObject> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var so = handle.Result;
                _cachedConfigs.Add(so.GetType(), so);

                if (_cachedConfigs.Count == TypesToLoad.Count)
                {
                    Debug.Log(_cachedConfigs.Count+ " Dbs found");
                    FinishTask(_cachedConfigs);
                }
            }
            else
            {
                FinishTaskWithError("one data base could not be found ");
            }

          
            
        }
    }
}

