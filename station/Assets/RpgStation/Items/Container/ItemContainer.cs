using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class ItemContainer
    {
        #region fields
        private Dictionary<int, ItemStack> _slots;
        private Dictionary<string, List<int>> _slotMap;
        private string _id;//SetId container_player_name_xxx, scene_container_random

        #endregion

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

    public class ItemStack
    {
        public string ItemId;
        public int ItemCount;
    }
}

