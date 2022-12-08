using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Station
{
    public class UiContainerPopup : UiPopup
    {
        public const string POPUP_KEY = "container_popup";
        [SerializeField] private UiCurrencyWidget _currencies = null;
        [SerializeField] private UiContainerWidget _containerWidget = null;
        private ContainerReference _containerReference;
        private BaseCharacter _user;
        private List<ScriptableNotificationChannel> _failedChannels;
        private List<ScriptableNotificationChannel> _resultChannels;
        private CurrenciesDb _currencyDb;
        
        private void CacheComponents()
        {
            if (_currencyDb != null) return;
            
            _currencyDb = GameInstance.GetDb<CurrenciesDb>();
        }
        public void Setup(ContainerReference containerReference, BaseCharacter user, List<ScriptableNotificationChannel> failedChannels, List<ScriptableNotificationChannel> resultChannels)
        {
            CacheComponents();
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
            
            var currencyHandler = _containerReference.GetContainer().CurrencyContainer;
            currencyHandler.OnChanged.AddListener(OnCurrencyChange);
            RefreshCurrencies();
        }

        public override void Hide()
        {
            _containerWidget.UnregisterEvents();
            var currencyHandler = _containerReference.GetContainer().CurrencyContainer;
            currencyHandler.OnChanged.RemoveListener(OnCurrencyChange);
            
            base.Hide();
        }

        public void OnClickCollectSlots()
        {
            var playerInventorySystem = GameInstance.GetSystem<PlayerInventorySystem>();
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

        
        private void OnCurrencyChange(CurrencyContainer.CurrencyChange arg1, CurrencyModel arg2, long arg3, long arg4)
        {
            RefreshCurrencies();
        }
        private void RefreshCurrencies()
        {
            var currencyHandler = _containerReference.GetContainer().CurrencyContainer;
            if (currencyHandler.GetCurrencies.Any())
            {
                KeyValuePair<string, long> mainCurrency = currencyHandler.GetCurrencies.FirstOrDefault();
                var currencyModel = _currencyDb.GetEntry(mainCurrency.Key);
                _currencies.DisplayAmount(currencyModel,mainCurrency.Value, true);
            }
            else
            {
                //no currency
                _currencies.DisplayAmount(null, 0, true);
            }
        }
    }
}

