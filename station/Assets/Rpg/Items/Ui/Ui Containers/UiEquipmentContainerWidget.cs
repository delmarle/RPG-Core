using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Station
{
    public class UiEquipmentContainerWidget : UiBaseContainer
    {
        [Serializable] public class LocalDictionary : SerializableDictionary<string, UiContainerSlot> {}
        [SerializeField]private LocalDictionary _slotMap = new LocalDictionary();
        
        public IDictionary<string, UiContainerSlot> Slots
        {
            get => _slotMap;
            set => _slotMap.CopyFrom (value);
        }

        protected override void CacheComponents()
        {
            var equipSlotDb = GameInstance.GetDb<EquipmentSlotsDb>();
            var slots = _slotMap.Values.ToArray();
            
            for (int i = 0; i < slots.Length; i++)
            {
                var entry = equipSlotDb.GetEntry(i);
                slots[i].SetBg(entry.Icon);
                slots[i].SetSlotName(entry.Name.GetValue());
                slots[i].SetId(i);
            }
        }
        
        public override void UpdateUiSlots()
        {
            _container = _containerReference.GetContainer();
            var list = _container?.GetState().Slots.Values.ToArray();
            int indexSlot = 0;
            foreach (var slot in Slots)
            {
                slot.Value.Setup(this, _itemDb, _containerReference);
                if (list != null) slot.Value.SetData(list[indexSlot]);
                indexSlot++;
            }
        }
    }

}

