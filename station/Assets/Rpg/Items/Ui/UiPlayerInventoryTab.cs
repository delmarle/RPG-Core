using UnityEngine;

namespace Station
{
    public class UiPlayerInventoryTab  : UiElementBase, ICharacterSwitchable
    {
        
        [SerializeField] private UiCharacterSelectionListWidget _charSelection;
        [SerializeField] private CharacterInventorySwitcher characterInventorySwitcher = null;
        [SerializeField] private CharacterEquipmentSwitcher characterEquipmentSwitcher = null;
        [SerializeField] private UiCurrencyWidget _currencyWudget = null;
        [SerializeField] private CurrencyModel _mainCurrency;
        
        private PlayerInventorySystem _inventorySystem;
        private TeamSystem _teamSystem;
        private GenericUiList<string, UiContainerWidget> _containerUiList;
        
        #region [[ INITIALIZATION ]]

        protected override void Awake()
        {
            base.Awake();
            
            _charSelection.ApplyTarget(new ICharacterSwitchable[] {characterInventorySwitcher, characterEquipmentSwitcher});
            CacheComponents();
        }

        protected override void Start()
        {
            base.Start();
            characterInventorySwitcher.Initialize();
        }


        void CacheComponents()
        {
            _inventorySystem = GameInstance.GetSystem<PlayerInventorySystem>();
            _teamSystem = GameInstance.GetSystem<TeamSystem>();
        }
        #endregion

        public override void Show()
        {
            if(_inventorySystem == null)
            {Awake();}
            base.Show();
            _inventorySystem.OnContainerChanged.AddListener(OnContainerChanged);
            characterInventorySwitcher.RegisterAllEvents();
            _inventorySystem.GetContainer().OnCurrencyChanged.AddListener(OnUpdateCurrency);
            RefreshCurrencies();
        }
        
        public override void Hide()
        {
            _inventorySystem.OnContainerChanged.RemoveListener(OnContainerChanged);
            characterInventorySwitcher.UnRegisterAllEvents();
            base.Hide();
            _inventorySystem.GetContainer().OnCurrencyChanged.RemoveListener(OnUpdateCurrency);
        }

        public void ClosePanel()
        {
            UiSystem.HidePanel<UiPlayerInventoryTab>(true);
        }
        
        private void OnContainerChanged(string containerId)
        {
            
        }

        public void SwitchCharacter(BaseCharacter character)
        {
            
        }

        private void OnUpdateCurrency(BaseItemContainer.CurrencyChange changeType, CurrencyModel model, long updatedValue, long amount)
        {
            if (_mainCurrency != model) return;
            RefreshCurrencies();
        }
        private void RefreshCurrencies()
        {
            var handler = _inventorySystem.GetContainer();
            long amount = handler.GetCurrencyAmount(_mainCurrency);
            _currencyWudget.DisplayAmount(_mainCurrency, amount, false);
        }
    }

}
