﻿using UnityEngine;

namespace Station
{
    public class UiPlayerInventoryTab  : UiElementBase, ICharacterSwitchable
    {
        
        [SerializeField] private UiCharacterSelectionListWidget _charSelection;
        [SerializeField] private CharacterInventorySwitcher characterInventorySwitcher = null;
        [SerializeField] private CharacterEquipmentSwitcher characterEquipmentSwitcher = null;
        
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
        }
        
        public override void Hide()
        {
            _inventorySystem.OnContainerChanged.RemoveListener(OnContainerChanged);
            characterInventorySwitcher.UnRegisterAllEvents();
            base.Hide();
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
    }

}
