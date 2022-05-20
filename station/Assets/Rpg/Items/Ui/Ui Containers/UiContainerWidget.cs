using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiContainerWidget : UiBaseContainer
    {
        
        [SerializeField] private LayoutGroup _slotsLayoutGroup = null;
        [SerializeField] private Transform _slotUiPrefab = null;
        

        private GenericUiList<ItemStack, UiContainerSlot> _slotsList;//data, widget

        protected override void  CacheComponents()
        {
            _slotsList = new GenericUiList<ItemStack, UiContainerSlot>(_slotUiPrefab.gameObject, _slotsLayoutGroup, 300);
            for (int i = 0; i < _slotsList.GetEntries().Count; i++)
            {
                _slotsList.GetComponentAt(i).SetId(i);
            }
        }
        

        public override void UpdateUiSlots()
        {
            _container = _containerReference.GetContainer();
            Dictionary<int, ItemStack>.ValueCollection list = _container?.GetState().Slots.Values;
            
            _slotsList.Generate(list, (entry, item) =>
            {
                item.Setup(this, _itemDb, _containerReference);
                item.SetData(entry);
            });
        }
    }
}

