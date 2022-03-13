using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class ClassicLootInteraction : LootInteractable
    {
        private ItemsSettingsDb _itemsSettingsDb;
        private UiContainerPopup _cachedContainerPopup;
        private string _containerIdReference;
        
        protected override void Setup()
        {
            _itemsSettingsDb = GameInstance.GetDb<ItemsSettingsDb>();
        }

        public override void SetContainerReference(string containerId)
        {
            base.SetContainerReference(containerId);
            _containerIdReference = containerId;
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
            var reference = new ContainerReference(_containerIdReference, GameInstance.GetSystem<AreaContainerSystem>());
            
            _cachedContainerPopup.Setup(reference, user, Config.FailNotificationChannels, Config.ResultNotificationChannels);
            _cachedContainerPopup.Show();
            
        }
    }
}