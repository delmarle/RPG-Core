
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
      
        private PlayersSave _playersSave;
       
        private Dictionary<string, BaseItemContainer> _containers;
        private PlayerInventoryType _inventoryType;

        /// <summary>
        /// CURRENCIES
        /// </summary>
        private CurrencyHandler _currencyHandler;
        #endregion
        #region initialization
        protected override void OnInit()
        {
            GameGlobalEvents.OnEnterGame.AddListener(OnEnterGame);
        }

        protected override void OnDispose()
        {
           GameGlobalEvents.OnEnterGame.RemoveListener(OnEnterGame);
        }

        protected override void OnDataBaseReady()
        {
            
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
            _playersSave = saveSystem.GetModule<PlayersSave>();
            _containers = new Dictionary<string, BaseItemContainer>();
            _inventoryType = itemsSettingsModel.ContainerSettings.PlayerInventoryType;
            
            LoadPlayerInventories();
            LoadPlayerCurrencies();
            LoadPlayersEquipment();
        }

        private void LoadPlayerInventories()
        {
            var saveSystem = GameInstance.GetSystem<SavingSystem>();
            var playerItemsSaveModule = saveSystem.GetModule<PlayerInventorySave>();
            if (_inventoryType == PlayerInventoryType.Shared)
            {
                var save = playerItemsSaveModule?.Value ?? new ContainersListSave();

                var state = save.GetContainerById(PLAYER_INVENTORY_KEY);
                var sharedContainer = new ItemContainer(PLAYER_INVENTORY_KEY, state, _itemsDb);
                _containers.Add(PLAYER_INVENTORY_KEY, sharedContainer);
            }
            else
            {
                //TODO add one for each player
            }
        }

        private void LoadPlayerCurrencies()
        {
      /*
            var saveSystem = GameInstance.GetSystem<SavingSystem>();
            var saveModule = saveSystem.GetModule<GeneralPlayerSave>();
            _currencyHandler = new CurrencyHandler();
            var save = saveModule?.Value ?? new GeneralPlayerData();
            var state = save.CurrenciesStatus;
            _currencyHandler.Load(state);
            */
        }

        public List<IdIntegerValue> GetCurrenciesState()
        {
            if (_currencyHandler == null)
            {
                return new List<IdIntegerValue>();
            }
            return _currencyHandler.GenerateSaveState();
        }

        private void LoadPlayersEquipment()
        {
            var saveSystem = GameInstance.GetSystem<SavingSystem>();
            var playerItemsSaveModule = saveSystem.GetModule<PlayerInventorySave>();
            var save = playerItemsSaveModule?.Value ?? new ContainersListSave();
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

        public Dictionary<string, ContainerState> BuildPlayerInventorySaveState()
        {
            Dictionary<string, ContainerState> tempSave = new Dictionary<string, ContainerState>();
            if (_containers == null) return tempSave;

            tempSave = _containers.ToDictionary(container => container.Key, container => container.Value.GetState());
            return tempSave;
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

