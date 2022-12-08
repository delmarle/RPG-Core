using System;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class ChestNode : Interactible
    {
        
        private AreaContainerSystem _containerSystem;
        private ItemsDb _itemDb;
        private ChestNodesDb _chestNodeDb;
        private ItemsSettingsDb _itemsSettingsDb;
        [HideInInspector]public bool StateSaved;
        [HideInInspector]public string SaveId;
        [HideInInspector]public string ChestNodeModelId;
        private UiContainerPopup _cachedContainerPopup;
        
        protected override void Setup()
        {
            GameGlobalEvents.OnSceneLoadObjects.AddListener(OnLoadContainer);
        }
        
        protected override void Dispose()
        {
            GameGlobalEvents.OnSceneLoadObjects.RemoveListener(OnLoadContainer); 
        }

        public void OnLoadContainer()
        {
            _chestNodeDb = GameInstance.GetDb<ChestNodesDb>();
            _itemsSettingsDb = GameInstance.GetDb<ItemsSettingsDb>();
            if (string.IsNullOrEmpty(ChestNodeModelId))
            {
                return;
            }

            var nodeModel = _chestNodeDb.GetEntry(ChestNodeModelId);
            var items = LootUtils.GenerateLootStack(nodeModel.LootTable);
            var currencies = LootUtils.GenerateCurrencies(nodeModel.LootTable);
            InitializeWithDefaultItems(Guid.NewGuid().ToString(), items, currencies, true);
        }

        private void Initialize(string saveId)
        {
            SaveId = saveId;
            _containerSystem = GameInstance.GetSystem<AreaContainerSystem>();
;
            _itemDb = GameInstance.GetDb<ItemsDb>();
            _chestNodeDb = GameInstance.GetDb<ChestNodesDb>();
            _itemsSettingsDb = GameInstance.GetDb<ItemsSettingsDb>();

            SetUiName(GetObjectName());
        }

        private void InitializeWithDefaultItems(string id, List<ItemStack> items, Dictionary<string, long> currencies, bool saved)
        {
            Initialize(id);
            var containerState = new ContainerState(8, items);
            containerState.Currencies = currencies;
            var container = new ItemContainer(id, containerState, _itemDb, currencies);
            _containerSystem.AddContainer(container, saved);
        }


        public override void Interact(BaseCharacter user)
        {
            base.Interact(user);
            if (_cachedContainerPopup == null)
            {
                var prefab = _itemsSettingsDb.Get().ContainerSettings.ContainerPopup;
                if (prefab == null)
                {
                    Debug.LogError($" missing prefab container popup");
                    return;
                }
                
                _cachedContainerPopup = UiSystem.GetUniquePopup<UiContainerPopup>(UiContainerPopup.POPUP_KEY);
            }
        
            CachePopup(_cachedContainerPopup);
            var reference = new ContainerReference(SaveId, GameInstance.GetSystem<AreaContainerSystem>());
            
            _cachedContainerPopup.Setup(reference, user, Config.FailNotificationChannels, Config.ResultNotificationChannels);
            _cachedContainerPopup.Show();
            
        }
        
        public override void OnCancelInteraction(BaseCharacter user)
        {
            UiSystem.HideUniquePopup<UiContainerPopup>(UiContainerPopup.POPUP_KEY);
            _cachedContainerPopup.Hide();
            base.OnCancelInteraction(user);
        }

        public override string GetObjectName()
        {
            var chestModel = _chestNodeDb.GetEntry(ChestNodeModelId);
            return chestModel.Name.GetValue();
        }
    }
}

