using System.Collections.Generic;
using System.Linq;

namespace Station
{
    
    public class AreaContainerSystem : BaseSystem, IContainersHandler
    {
        private Dictionary<string, BaseItemContainer> _tempContainer = new Dictionary<string, BaseItemContainer>();
        private Dictionary<string, BaseItemContainer> _savedContainer;
        private AreaContainersSave _areaContainersSave;
        private SceneSystem _sceneSystem;
        #region OVERRIDES
        protected override void OnInit()
        {
            GameGlobalEvents.OnSceneInitialize.AddListener(OnSceneInitialize);
            GameGlobalEvents.OnEnterGame.AddListener(OnEnterGame);
            GameGlobalEvents.OnTriggerSceneSave.AddListener(OnTriggerSave);
        }

        protected override void OnDispose()
        {
            GameGlobalEvents.OnSceneInitialize.RemoveListener(OnSceneInitialize);
            GameGlobalEvents.OnEnterGame.RemoveListener(OnEnterGame);
            GameGlobalEvents.OnTriggerSceneSave.RemoveListener(OnTriggerSave);
        }

        public void AddContainer(ItemContainer container, bool saved)
        {
            if (saved)
            {
                _savedContainer.Add(container.GetId(), container);
            }
            else
            {
                _tempContainer.Add(container.GetId(), container);
            }


        }

        public BaseItemContainer GetContainer(string containerId)
        {
            if (_savedContainer.ContainsKey(containerId))
            {
                return _savedContainer[containerId];
            }
            
            if (_tempContainer.ContainsKey(containerId))
            {
                return _tempContainer[containerId];
            }

            return null;
        }

        
        #endregion
        
        private void OnApplicationQuit()
        {
            OnTriggerSave();
        }
        

        private void OnEnterGame()
        {
            var saveSystem = _station.GetSystem<SavingSystem>();
            _sceneSystem =  _station.GetSystem<SceneSystem>();
            _areaContainersSave = saveSystem.GetModule<AreaContainersSave>();
            _areaContainersSave.Initialize();
        }
        
        private void OnSceneInitialize()
        {
            var dbSystem = RpgStation.GetSystemStatic<DbSystem>();
            var itemDb = dbSystem.GetDb<ItemsDb>();
            _areaContainersSave.Load(_sceneSystem.GetCurrentDestination().SceneName);
            _savedContainer = new Dictionary<string, BaseItemContainer>();
            if (_areaContainersSave.Value == null)
            {
                _areaContainersSave.Value =  new ContainersListSave();
            }

            var save = _areaContainersSave.Value;
            //Load Existing
            foreach (var entry in save.Containers)
            {
                _savedContainer.Add(entry.Key, new ItemContainer(entry.Key, entry.Value, itemDb));
            }
        }
        
        private void OnTriggerSave()
        {
            var areaState = _savedContainer.ToDictionary(container => container.Key, container => container.Value.GetState());
            _areaContainersSave.Value.Containers = areaState;
            _areaContainersSave.Save();
        }
    }
}

