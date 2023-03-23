using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class UiHudLowerBar : MonoBehaviour
    {
        [SerializeField] private UiCurrencyWidget _currencyWidget;
        [SerializeField] private CurrencyModel _mainCurrency;
        private PlayerInventorySystem _inventorySystem;
        
        #region Unity callbacks

        private void Awake()
        {
            _inventorySystem = GameInstance.GetSystem<PlayerInventorySystem>();
        }

        private void OnEnable()
        {
            _inventorySystem.GetContainer().OnCurrencyChanged.AddListener(OnUpdateCurrency);
            RefreshCurrencies();
        }

        private void OnDisable()
        {
            _inventorySystem.GetContainer().OnCurrencyChanged.RemoveListener(OnUpdateCurrency);
        }

        #endregion
        
        private void OnUpdateCurrency(BaseItemContainer.CurrencyChange arg1, CurrencyModel arg2, long arg3, long arg4)
        {
            RefreshCurrencies();
        }
        
        private void RefreshCurrencies()
        {
            var handler = _inventorySystem.GetContainer();
            long amount = handler.GetCurrencyAmount(_mainCurrency);
            _currencyWidget.DisplayAmount(_mainCurrency, amount, false);
        }
    }
}

