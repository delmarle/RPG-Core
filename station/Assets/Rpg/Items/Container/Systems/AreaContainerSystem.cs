using System.Collections.Generic;
using System.Linq;

namespace Station
{
    
    public class AreaContainerSystem : BaseSystem, IContainersHandler
    {
        private Dictionary<string, BaseItemContainer> _tempContainer = new Dictionary<string, BaseItemContainer>();
        private Dictionary<string, BaseItemContainer> _savedContainer;
        private AreaContainersSave _areaContainersSave;
        private RpgSceneSystem _sceneSystem;
        #region OVERRIDES
        protected override void OnInit()
        {
            GameGlobalEvents.OnSceneInitialize.AddListener(OnSceneInitialize);
            GameGlobalEvents.OnEnterGame.AddListener(OnEnterGame);
        }

        protected override void OnDispose()
        {
            GameGlobalEvents.OnSceneInitialize.RemoveListener(OnSceneInitialize);
            GameGlobalEvents.OnEnterGame.RemoveListener(OnEnterGame);
        }

        protected override void OnDataBaseReady()
        {
            
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

        public void CleanContainer(string containerId)
        {
            if (_tempContainer.ContainsKey(containerId))
            {
                _tempContainer.Remove(containerId);
            }

            if (_savedContainer.ContainsKey(containerId))
            {
                _savedContainer.Remove(containerId);
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
            var saveSystem = GameInstance.GetSystem<SavingSystem>();
            _sceneSystem =  GameInstance.GetSystem<RpgSceneSystem>();
            _areaContainersSave = saveSystem.GetModule<AreaContainersSave>();
            _areaContainersSave.Initialize();
        }
        
        private void OnSceneInitialize()
        {
          //  if (sceneType != SceneType.Area) return;
            
            var itemDb = GameInstance.GetDb<ItemsDb>();
            if (_sceneSystem.GetCurrentDestination() == null) return;
            
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
        
        //TODO get data from save data instead
        private void OnTriggerSave()
        {
            var areaState = _savedContainer?.ToDictionary(container => container.Key, container => container.Value.GetState());
            if (_areaContainersSave?.Value == null)
            {
                return;
            }

            _areaContainersSave.Value.Containers = areaState;
            _areaContainersSave.Save();
        }
    }
}

