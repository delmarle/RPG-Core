using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public static class InventoryUtils
    {
        public static void CreatePlayerInventory(List<ItemStack> detaultItems)
        {
            var savingSystem = GameInstance.GetSystem<SavingSystem>();
            var settingsDb = GameInstance.GetDb<ItemsSettingsDb>();
            var containerSettings = settingsDb.Get().ContainerSettings;
            var module = savingSystem.GetModule<PlayerInventorySave>();
            var inventoryList = new ContainersListSave();
            var inventory = new ContainerState(containerSettings.InitialPlayerInventorySize, detaultItems);
            if (containerSettings.PlayerInventoryType == PlayerInventoryType.Shared)
            {
                
            }
            else if (containerSettings.PlayerInventoryType == PlayerInventoryType.PerCharacter)
            {
            }

            inventoryList.Containers.Add(PlayerInventorySystem.PLAYER_INVENTORY_KEY, inventory);
            module.Value = inventoryList;
            module.Save();
        }

    }
}

