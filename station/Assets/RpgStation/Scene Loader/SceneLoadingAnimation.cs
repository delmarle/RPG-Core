
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
            GameGlobalEvents.OnSceneReady.RemoveListener(OnSceneReady);
            GameGlobalEvents.OnSceneReady.AddListener(OnSceneReady);
            _sceneLoader.OnLoadingProgress -= OnLoaderProgress;
            _sceneLoader.OnLoadingProgress += OnLoaderProgress;
    
            SetSliderValue(0);
        }


        private void OnDestroy()
        {
            GameGlobalEvents.OnSceneStartLoad.RemoveListener(OnStartLoadScene);
            GameGlobalEvents.OnSceneInitialize.RemoveListener(OnSceneInitialize);
            GameGlobalEvents.OnSceneReady.RemoveListener(OnSceneReady);
            _sceneLoader.OnLoadingProgress -= OnLoaderProgress;
        }

        private void OnStartLoadScene()
        {
            SetSliderValue(0);
        }
        
        //75%
        private void OnSceneInitialize()
        {
            SetSliderValue(0.75f);
        }
        
        //100%
        private void OnSceneReady()
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
