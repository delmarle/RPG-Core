
using Station.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public sealed class SceneLoadingAnimation : MonoBehaviour 
    {
        #region [[ FIELDS ]]

        [SerializeField] private SceneLoader _sceneLoader = null;

        [SerializeField] private Slider _loadingBar = null;

        #endregion

        private void Awake()
        {
            GameGlobalEvents.OnSceneStartLoad.RemoveListener(OnStartLoadScene);
            GameGlobalEvents.OnSceneStartLoad.AddListener(OnStartLoadScene);
            GameGlobalEvents.OnSceneInitialize.RemoveListener(OnSceneInitialize);
            GameGlobalEvents.OnSceneInitialize.AddListener(OnSceneInitialize);
            GameGlobalEvents.OnSceneLoadObjects.RemoveListener(OnSceneReady);
            GameGlobalEvents.OnSceneLoadObjects.AddListener(OnSceneReady);
            _sceneLoader.OnLoadingProgress -= OnLoaderProgress;
            _sceneLoader.OnLoadingProgress += OnLoaderProgress;
    
            SetSliderValue(0);
        }


        private void OnDestroy()
        {
            GameGlobalEvents.OnSceneStartLoad.RemoveListener(OnStartLoadScene);
            GameGlobalEvents.OnSceneInitialize.RemoveListener(OnSceneInitialize);
            GameGlobalEvents.OnSceneLoadObjects.RemoveListener(OnSceneReady);
            _sceneLoader.OnLoadingProgress -= OnLoaderProgress;
        }

        private void OnStartLoadScene(SceneType sceneType)
        {
            SetSliderValue(0);
        }
        
        //75%
        private void OnSceneInitialize(SceneType sceneType)
        {
            SetSliderValue(0.75f);
        }
        
        //100%
        private void OnSceneReady(SceneType sceneType)
        {
            SetSliderValue(1);
        }

        private void OnLoaderProgress(float progress)
        {
            SetSliderValue(progress);
        }

        private void SetSliderValue(float progress)
        {
            _loadingBar.value = progress;
        }

    }
}
