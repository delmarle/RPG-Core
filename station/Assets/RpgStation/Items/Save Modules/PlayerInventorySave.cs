using System;
using System.Collections.Generic;

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
        public Dictionary<int, ItemStack> Slots = new Dictionary<int, ItemStack>();
    }
}

