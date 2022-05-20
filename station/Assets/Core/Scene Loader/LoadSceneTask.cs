using System.Collections;
using System.Collections.Generic;
using Station.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Station
{
    public enum StateLoading
    {
        FadeIn,
        LoadingScene,
        InitializeScene,
        Ready
    }
    public class LoadSceneTask :  BasicTask<bool>
    {
        private AssetReference _loaderReference;
        private string _sceneName;
        private SceneLoader _loader;
        private StateLoading _state;
        private SceneType _loadedType;
        public LoadSceneTask(string sceneName, AssetReference loaderReference, SceneType sceneType)
        {
            Proxy = new ProxyWithRunner();
            _loadedType = sceneType;
            _sceneName = sceneName;
            _state = StateLoading.FadeIn;
            GameGlobalEvents.OnSceneInitialize.AddListener(OnSceneStartInitialize);
            _loaderReference = loaderReference;
        }
        
        protected override IEnumerator HandleExecute()
        {
           
            if (string.IsNullOrEmpty(_loaderReference.AssetGUID) == false)
            {
                _loader = SceneLoader.Instance;
                if (_loader == null)
                {
                    var handle = _loaderReference.InstantiateAsync();
                    yield return handle;
                    handle.Completed+= HandleOnCompleted;
              
                    while (_loader == null)
                    {
                        yield return null;
                    }
                }
                
                SendProgress(_loader.Progress);
                _loader.LoadScene(_sceneName, _loadedType);
               
                _state = StateLoading.LoadingScene;
                
                while (_state != StateLoading.InitializeScene)
                {
                    yield return null;
                }
                yield return null;
                _loader.UpdateProgressEvent(1);
            }

            yield return null;
            
            _state = StateLoading.Ready;
            FinishTask(true);
        }

        private void HandleOnCompleted(AsyncOperationHandle<GameObject> op)
        {
            _loader = op.Result.GetComponent<SceneLoader>();

        }
        
        private void OnSceneStartInitialize(SceneType loadedType)
        {
            _state = StateLoading.InitializeScene;
        }
    }
}