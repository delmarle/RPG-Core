using System;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class PlayerInventorySave : SaveModule<ContainersListSave>
    {
  
    }


    public class ContainersListSave
    {
        public Dictionary<string, ContainerState> Containers = new Dictionary<string, ContainerState>();

        public ContainerState GetContainerById(string id)
        {
            if (Containers!= null && Containers.ContainsKey(id))
            {
                return Containers[id];
            }

            return null;
        }
    }

    [Serializable]
    public class ContainerState
    {
        public ContainerState()
        {
        }

        public ContainerState(int size, List<ItemStack> defaultItems)
        {
            if (defaultItems.Count > size)
            {
                Debug.LogWarning("inventory dont have enough slots");
            }

            Slots  = new Dictionary<int, ItemStack>();
            for (int i = 0; i < size; i++)
            {
                Slots.Add(i,null);
            }

            for (int i = 0; i < defaultItems.Count; i++)
            {
                if (i <= Slots.Count)
                {
                    var itemToAdd = defaultItems[i];
                    Slots[i] = itemToAdd;
                }
            }
        }

        public ContainerState(Dictionary<int, ItemStack> save)
        {
            Slots = save;
        }

        public Dictionary<int, ItemStack> Slots;
    }
}

