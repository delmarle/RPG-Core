using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class CharacterEquipmentSwitcher : MonoBehaviour, ICharacterSwitchable
    {
        [SerializeField] private UiEquipmentContainerWidget _equipmentContainer = null;
        private PlayerInventorySystem inventorySystem;
        
        private void Awake()
        {
            inventorySystem = RpgStation.GetSystem<PlayerInventorySystem>();
        }

        public void SwitchCharacter(BaseCharacter character)
        {
            var containerReference = new ContainerReference(PlayerInventorySystem.PLAYER_EQUIPMENT_KEY+character.GetCharacterId(), inventorySystem);
            _equipmentContainer.Init(containerReference);
            _equipmentContainer.UnregisterEvents();
            _equipmentContainer.RegisterEvents();
            _equipmentContainer.UpdateUiSlots();
        }
    }

}

