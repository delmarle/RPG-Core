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
        Dictionary<string, UiEquipmentContainerWidget> playerMap = new Dictionary<string, UiEquipmentContainerWidget>();
        private void Awake()
        {
            if (inventorySystem == null)
            {
                inventorySystem = GameInstance.GetSystem<PlayerInventorySystem>();
            }
            _equipmentContainer.gameObject.SetActive(false);
        }

        public void SwitchCharacter(BaseCharacter character)
        {
            if (inventorySystem == null)
            {
                inventorySystem = GameInstance.GetSystem<PlayerInventorySystem>();
            }
            string playerId = character.GetCharacterId();
            if (playerMap.ContainsKey(playerId) == false)
            {
                var containerReference = new ContainerReference(PlayerInventorySystem.PLAYER_EQUIPMENT_KEY+playerId, inventorySystem);
                UiEquipmentContainerWidget uiContainer = Instantiate(_equipmentContainer,transform);
                playerMap.Add(playerId, uiContainer);
                uiContainer.Init(containerReference);
                uiContainer.UnregisterEvents();
                uiContainer.RegisterEvents();
                uiContainer.UpdateUiSlots();
            }

            foreach (var entry in playerMap)
            {
                entry.Value.gameObject.SetActive(entry.Key == playerId);
            }
        }
    }

}

