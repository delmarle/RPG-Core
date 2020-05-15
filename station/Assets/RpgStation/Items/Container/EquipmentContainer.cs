using System.Collections;
using System.Collections.Generic;
using Station;
using UnityEngine;

namespace Station
{
    
    public class EquipmentContainer : ItemContainer
    {
        //mapping layer for each slot
        public EquipmentContainer(string id, ContainerState state, ItemsDb itemsDb) : base(id, state, itemsDb)
        {
        }

        int PreferedEquipmentSlot(object item)
        {
            //where this equipement should go
            return 0;
        }
    }
}

//id
//equipment type allowed
//equipment type use extra slot when equiped ? (2H)


//equipment slots:
//main hand / off hand/ finger1 / legs / body/ head
//can receive
    //main hand: mh weapon
    //off hand: secondhand weapon, shields, trinkets

//equipment items type
//main hand weapon/rings/