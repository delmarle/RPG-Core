using System;
using Station.Data;

namespace Station
{
    
    public partial class SceneSystem : BaseSystem
    {
        #region FIELDS

        protected ScenesDb _scenesDb;
        protected GameSettingsDb _settingsDb;
        private bool _isLoadingScene = false;
        public bool IsLoadingScene => _isLoadingScene;

        #endregion       




        protected override void OnInit()
        {
            Invoke("OnInitOverride", 0);
        }

        protected override void OnDispose()
        {
            Invoke("OnDisposeOverride", 0);
        }

        protected override void OnDataBaseReady()
        {
            _scenesDb = GameInstance.GetDb<ScenesDb>();
            _settingsDb = GameInstance.GetDb<GameSettingsDb>();
        }

        public void LoadNormalScene(string sceneName, SceneType sceneType, Action OnSceneLoadedCallback)
        {
            _isLoadingScene = true;
            var loadingScreen = _settingsDb.Get().LoadingScreen;
            LoadSceneTask task = new LoadSceneTask(sceneName, loadingScreen, sceneType);
             task.SetEndCallback((templateTask, b, arg3, arg4) =>
             {
                 OnSceneLoaded();
                 OnSceneLoadedCallback?.Invoke();
             });
            task.Execute();
        }

        private void OnSceneLoaded()
        {
            _isLoadingScene = false;
        }
    }

    public class TravelModel
    {
        public string SceneName;
    }
}

