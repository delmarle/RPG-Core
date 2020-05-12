
using System.Collections.Generic;
using System.Linq;

namespace Station
{
    public class PlayerInventorySystem : BaseSystem, IContainersHandler
    {
        #region FIELDS

        public StationEvent<string> OnContainerChanged = new StationEvent<string>();
        public const string PLAYER_INVENTORY_KEY = "player_inventory";
        private ItemsDb _itemsDb;
        private ItemsSettingsDb _itemsSettingsDb;
        private PlayerInventorySave _playerItemsSave;
       
        private Dictionary<string, ItemContainer> _containers;
        private PlayerInventoryType _inventoryType;
        #endregion
        #region initialization
        protected override void OnInit()
        {
            GameGlobalEvents.OnEnterGame.AddListener(OnEnterGame);
            GameGlobalEvents.OnTriggerSceneSave.AddListener(OnTriggerSave);
        }

        protected override void OnDispose()
        {
           GameGlobalEvents.OnEnterGame.RemoveListener(OnEnterGame);
           GameGlobalEvents.OnTriggerSceneSave.RemoveListener(OnTriggerSave);
        }

        private void OnApplicationQuit()
        {
            OnTriggerSave();
        }

        private void CacheDatabases()
        {
            var dbSystem = _station.GetSystem<DbSystem>();
            _itemsDb = dbSystem.GetDb<ItemsDb>();
            _itemsSettingsDb = dbSystem.GetDb<ItemsSettingsDb>();
        }

        private void OnEnterGame()
        {
            CacheDatabases();
            var itemsSettingsModel = _itemsSettingsDb.Get();
            var saveSystem = _station.GetSystem<SavingSystem>();
            _playerItemsSave = saveSystem.GetModule<PlayerInventorySave>();
            _containers = new Dictionary<string, ItemContainer>();
            _inventoryType = itemsSettingsModel.ContainerSettings.PlayerInventoryType;
            
            if (_inventoryType == PlayerInventoryType.Shared)
            {
                var save = _playerItemsSave.Value ?? new ContainersListSave();

                var state = save.GetContainerById(PLAYER_INVENTORY_KEY);
                var sharedContainer = new ItemContainer(PLAYER_INVENTORY_KEY, state, _itemsDb);
                _containers.Add(PLAYER_INVENTORY_KEY, sharedContainer);
            }
            else
            {
               //TODO add one for each player
            }
        }
        
        private void OnTriggerSave()
        {
            var tempSave = _containers.ToDictionary(container => container.Key, container => container.Value.GetState());
            _playerItemsSave.Value.Containers = tempSave;
            _playerItemsSave.Save();
        }
        #endregion
        

        public ItemContainer GetContainer(string containerId)
        {
            if (_inventoryType == PlayerInventoryType.Shared)
            {
                return _containers[PLAYER_INVENTORY_KEY];
            }


            return _containers.ContainsKey(containerId) ? _containers[containerId] : null;
        }
    }
}

