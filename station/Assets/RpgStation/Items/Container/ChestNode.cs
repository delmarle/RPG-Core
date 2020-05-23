using System;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class ChestNode : Interactible
    {
        [SerializeField] private  List<ItemStack> _defaultItems = new List<ItemStack>();
        
        private AreaContainerSystem _containerSystem;
        private ItemsDb _itemDb;
        private ItemsSettingsDb _itemsSettingsDb;
        [HideInInspector]public bool StateSaved;
        [HideInInspector]public string _id;
        [HideInInspector]public string ChestNodeId;
        private UiContainerPopup _cachedContainerPopup;
        
        protected override void Setup()
        {
            GameGlobalEvents.OnSceneLoadObjects.AddListener(OnLoadContainer);
        }
        
        protected override void Dispose()
        {
            GameGlobalEvents.OnSceneLoadObjects.RemoveListener(OnLoadContainer); 
        }

        private void OnLoadContainer()
        {
            InitializeWithDefaultItems(Guid.NewGuid().ToString(), _defaultItems, true);
        }

        private void Initialize(string id)
        {
            _id = id;
            _containerSystem = RpgStation.GetSystemStatic<AreaContainerSystem>();
            var dbSystems = RpgStation.GetSystemStatic<DbSystem>();
            _itemDb = dbSystems.GetDb<ItemsDb>();
            _itemsSettingsDb = dbSystems.GetDb<ItemsSettingsDb>();
            SetUiName("Container");
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
            _cachedContainerPopup.Setup(new ContainerReference(_id, RpgStation.GetSystemStatic<AreaContainerSystem>()), user);
            _cachedContainerPopup.Show();
            
        }
        
        public override void OnCancelInteraction(BaseCharacter user)
        {
            Debug.Log("cancel");
            UiSystem.HideUniquePopup<UiContainerPopup>(UiContainerPopup.POPUP_KEY);
            _cachedContainerPopup.Hide();
            base.OnCancelInteraction(user);
        }
    }

}

