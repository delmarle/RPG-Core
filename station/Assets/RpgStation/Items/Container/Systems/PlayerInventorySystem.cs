using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class PlayerInventorySystem : BaseSystem, ICharacterInventoryHandler
    {
        #region FIELDS
        private ItemsDb _itemsDb;
        private ItemsSettingsDb _itemsSettingsDb;

        private ItemContainer _sharedContainer;
        private Dictionary<string, ItemContainer> _containers;
        #endregion
        #region initialization
        protected override void OnInit()
        {
            CacheDatabases();
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
            var itemsSettingsModel = _itemsSettingsDb.Get();
            if (itemsSettingsModel.ContainerSettings.PlayerInventoryType == PlayerInventoryType.Shared)
            {
                _sharedContainer = new ItemContainer();
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

