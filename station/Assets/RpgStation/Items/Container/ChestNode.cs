using System;
using System.Collections.Generic;
using Station.Data;
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

        public void OnLoadContainer(SceneType sceneType)
        {
            _chestNodeDb = GameInstance.GetDb<ChestNodesDb>();
            _itemsSettingsDb = GameInstance.GetDb<ItemsSettingsDb>();
            if (string.IsNullOrEmpty(ChestNodeModelId))
            {
                return;
            }

            var nodeModel = _chestNodeDb.GetEntry(ChestNodeModelId);
            var defaultItems = LootUtils.GenerateLootStack(nodeModel.LootTable);
            InitializeWithDefaultItems(Guid.NewGuid().ToString(), defaultItems, true);
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

        private void InitializeWithDefaultItems(string id, List<ItemStack> items, bool saved)
        {
            Initialize(id);
            var containerState = new ContainerState(8, items);
            var container = new ItemContainer(id, containerState, _itemDb);
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
                
                _cachedContainerPopup = UiSystem.GetUniquePopup<UiContainerPopup>(UiContainerPopup.POPUP_KEY, prefab);
            }
        
            CachePopup(_cachedContainerPopup);
            var reference = new ContainerReference(SaveId, GameInstance.GetSystem<AreaContainerSystem>());
            
            _cachedContainerPopup.Setup(reference, user, Config.FailNotificationChannels, Config.ResultNotificationChannels);
            _cachedContainerPopup.Show();
            
        }
        
        public override void OnCancelInteraction(BaseCharacter user)
        {
            Debug.Log("cancel");
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

