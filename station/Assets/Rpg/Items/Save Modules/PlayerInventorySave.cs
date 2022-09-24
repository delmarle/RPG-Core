﻿using System;
using System.Collections.Generic;

namespace Station
{
    public class PlayerInventorySave : SaveModule<ContainersListSave>
    {
        public override void FetchData()
        {
            if (Value == null) return;
            var playerInvSystem = GameInstance.GetSystem<PlayerInventorySystem>();
            Value.Containers = playerInvSystem.BuildPlayerInventorySaveState();
        }
    }


    [Serializable]
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

   
}

