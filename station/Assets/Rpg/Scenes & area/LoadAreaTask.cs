using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Station
{

    public class LoadAreaTask : BasicTask<bool>
    {
        private AssetReference _loaderReference;
        private TravelModel _model;
        private SceneLoader _loader;
        private StateLoading _state;
       

        public LoadAreaTask(TravelModel model, AssetReference loaderReference)
        {
            Proxy = new ProxyWithRunner();
            _model = model;
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
                _loader.LoadScene(_model.SceneName);
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
        
        private void OnSceneStartInitialize()
        {
            _state = StateLoading.InitializeScene;
        }
    }
}

