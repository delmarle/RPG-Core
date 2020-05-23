using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class CharacterEquipmentsTabs : CharacterTabSwitcher
    {
        [SerializeField] private UiEquipmentContainerWidget _equipmentContainer = null;
        private PlayerInventorySystem inventorySystem;
        
        private void Awake()
        {
            inventorySystem = RpgStation.GetSystemStatic<PlayerInventorySystem>();
        }

        public override void SwitchCharacter(BaseCharacter character)
        {
          
            //var container = inventorySystem.GetContainer(PlayerInventorySystem.PLAYER_EQUIPMENT_KEY+character.GetCharacterId());
            var containerReference = new ContainerReference(PlayerInventorySystem.PLAYER_EQUIPMENT_KEY+character.GetCharacterId(), inventorySystem);
            _equipmentContainer.Init(containerReference);
            _equipmentContainer.UnregisterEvents();
            _equipmentContainer.RegisterEvents();
            _equipmentContainer.UpdateUiSlots();
        }
    }

}

