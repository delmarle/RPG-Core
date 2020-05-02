using System;
using System.Collections.Generic;

namespace Station
{
    public class ItemContainer
    {
        #region fields
        private ContainerState _container;
        private Dictionary<string, List<int>> _slotMap;
        private string _id;//SetId container_player_name_xxx, scene_container_random

        #endregion
        public ContainerState GetState() => _container;
        public string GetId() => _id;
        public ItemContainer(string id, ContainerState state)
        {
            _id = id;
            _container = state;
        }
        public void AddSlots(int added)
        {
           // _container.Slots.Add();
                
        }
        //saving from external handler
        //max slots

        //init content
        //add items
        //remove items
        //events

        //ICharacterInventoryHandler
        //player inventory system : manage inventories for players
        //npc inventory system : allow to handle npc loots or their inventories
        

    }

    [Serializable]
    public class ItemStack
    {
        public string ItemId;
        public int ItemCount;
    }
}

