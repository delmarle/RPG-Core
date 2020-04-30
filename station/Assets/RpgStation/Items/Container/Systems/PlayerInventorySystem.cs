using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class PlayerInventorySystem : BaseSystem, ICharacterInventoryHandler
    {
        #region FIELDS

        public const string PLAYER_INVENTORY_KEY = "player_inventory";
        private ItemsDb _itemsDb;
        private ItemsSettingsDb _itemsSettingsDb;
        private PlayerInventorySave _playerItemsSave;

        private ItemContainer _sharedContainer;
        private Dictionary<string, ItemContainer> _containers;
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

        private void CacheDatabases()
        {
            var dbSystem = _station.GetSystem<DbSystem>();
            _itemsDb = dbSystem.GetDb<ItemsDb>();
            _itemsSettingsDb = dbSystem.GetDb<ItemsSettingsDb>();
           
          
        }

        private void OnEnterGame()
        {
            Debug.Log("entering game and initializing inventory");
            CacheDatabases();
            var itemsSettingsModel = _itemsSettingsDb.Get();
            var saveSystem = _station.GetSystem<SavingSystem>();
            _playerItemsSave = saveSystem.GetModule<PlayerInventorySave>();
            if (itemsSettingsModel.ContainerSettings.PlayerInventoryType == PlayerInventoryType.Shared)
            {
                var save = _playerItemsSave.Value;
                if (save == null)
                {
                    save = new ContainersListSave();
                }

                var state = save.GetContainerById(PLAYER_INVENTORY_KEY);
                _sharedContainer = new ItemContainer(PLAYER_INVENTORY_KEY, state);
            }
            else
            {
                _containers = new Dictionary<string, ItemContainer>();
            }
        }
        #endregion

        public void AddItems(BaseCharacter owner, ItemStack[] itemsAdded)
        {
            
        }

        public void RemoveItems(BaseCharacter owner, ItemStack[] itemsRemoved)
        {
            
        }

        public ItemStack[] GetItems(BaseCharacter owner)
        {
            return null;
        }
    }
}

