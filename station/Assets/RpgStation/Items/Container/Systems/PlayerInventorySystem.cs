
using System.Collections.Generic;
using System.Linq;

namespace Station
{
    public class PlayerInventorySystem : BaseSystem, IContainersHandler
    {
        #region FIELDS

        public StationEvent<string> OnContainerChanged = new StationEvent<string>();
        public const string PLAYER_INVENTORY_KEY = "player_inventory";
        public const string PLAYER_EQUIPMENT_KEY = "player_equipment_";
        private ItemsDb _itemsDb;
        private ItemsSettingsDb _itemsSettingsDb;
        private PlayerInventorySave _playerItemsSave;
        private PlayersSave _playersSave;
       
        private Dictionary<string, BaseItemContainer> _containers;
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
            _itemsDb = GameInstance.GetDb<ItemsDb>();
            _itemsSettingsDb = GameInstance.GetDb<ItemsSettingsDb>();
        }

        private void OnEnterGame()
        {
            CacheDatabases();
            var itemsSettingsModel = _itemsSettingsDb.Get();
            var saveSystem = GameInstance.GetSystem<SavingSystem>();
            _playerItemsSave = saveSystem.GetModule<PlayerInventorySave>();
            _playersSave = saveSystem.GetModule<PlayersSave>();
            _containers = new Dictionary<string, BaseItemContainer>();
            _inventoryType = itemsSettingsModel.ContainerSettings.PlayerInventoryType;

            LoadPlayerInventories();
            LoadPlayersEquipment();
        }

        private void LoadPlayerInventories()
        {
            if (_inventoryType == PlayerInventoryType.Shared)
            {
                var save = _playerItemsSave?.Value ?? new ContainersListSave();

                var state = save.GetContainerById(PLAYER_INVENTORY_KEY);
                var sharedContainer = new ItemContainer(PLAYER_INVENTORY_KEY, state, _itemsDb);
                _containers.Add(PLAYER_INVENTORY_KEY, sharedContainer);
            }
            else
            {
                //TODO add one for each player
            }
        }

        private void LoadPlayersEquipment()
        {
            
            var save = _playerItemsSave?.Value ?? new ContainersListSave();
            if (_playersSave?.Value != null)
            {
                foreach (var playerPair in _playersSave?.Value)
                {

                    string id = PLAYER_EQUIPMENT_KEY + playerPair.Key;
                    var equipmentState = save.GetContainerById(id);
                    var playerEquipmentContainer = new EquipmentContainer(id, equipmentState, _itemsDb);
                    _containers.Add(id, playerEquipmentContainer);
                }
            }
        }

        private void OnTriggerSave()
        {
            if (_containers == null) return;
            if (_playerItemsSave.Value == null) return;
            
            var tempSave = _containers.ToDictionary(container => container.Key, container => container.Value.GetState());
            _playerItemsSave.Value.Containers = tempSave;
            _playerItemsSave.Save();
        }
        #endregion

        public BaseItemContainer GetContainer(string containerId)
        {

            if (_containers.ContainsKey(containerId))
            {
                return _containers[containerId];
            }

            if (_inventoryType == PlayerInventoryType.Shared)
            {
                return _containers[PLAYER_INVENTORY_KEY];
            }


            return null;
        }
    }
}

