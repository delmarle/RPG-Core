using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Station
{
    public class UiContainerPopup : UiPopup
    {
        public const string POPUP_KEY = "container_popup";
        [SerializeField] private UiContainerWidget _containerWidget = null;
        private ContainerReference _containerReference;
        private BaseCharacter _user;
        private List<ScriptableNotificationChannel> _failedChannels;
        private List<ScriptableNotificationChannel> _resultChannels;
        


        public void Setup(ContainerReference containerReference, BaseCharacter user, List<ScriptableNotificationChannel> failedChannels, List<ScriptableNotificationChannel> resultChannels)
        {
            _user = user;
            _failedChannels = failedChannels;
            _resultChannels = resultChannels;
            _containerReference = containerReference;
            _containerWidget.Init(containerReference);
        }
        public override void Show()
        {
            base.Show();
            _containerWidget.RegisterEvents();
            _containerWidget.UpdateUiSlots();
        }
        public override void Hide()
        {
            _containerWidget.UnregisterEvents();
            base.Hide();
        }

        public void OnClickCollectSlots()
        {
            var playerInventorySystem = RpgStation.GetSystem<PlayerInventorySystem>();
            var playerContainer = playerInventorySystem.GetContainer(_user.GetCharacterId());
            var sourceContainer = _containerReference.GetContainer();
            foreach (var slot in sourceContainer.GetState().Slots)
            {
                var cachedId = slot.Value.ItemId;
                var cachedAmount = slot.Value.ItemCount;
                if (slot.Value.HasItem())
                {
                    var result = playerContainer.TryMoveSlotToContainer(slot.Key, sourceContainer);
                    if (result == MoveItemToContainResult.ContainerIsFull)
                    {
                        if (_failedChannels.Any())
                        {
                            var msg = $"Your inventory is too full to loot this item";
                            var dict = new Dictionary<string, object> {{UiConstants.TEXT_MESSAGE, msg}};
                            UiNotificationSystem.ShowNotification(_failedChannels, dict);
                        }
                        return;
                    }
                    else
                    {
                        if (_resultChannels.Any())
                        {
                            var dict = new Dictionary<string, object> {{UiConstants.ITEM_KEY, cachedId}, {UiConstants.ITEM_AMOUNT, cachedAmount}};

                            UiNotificationSystem.ShowNotification(_resultChannels, dict);
                        }
                       
                      
                    }
                }
            }
            
     
        }
        
    }
}

