using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiContainerWidget : MonoBehaviour
    {
        [SerializeField] private UiDragSlotDummy _dragDummy = null;
        [SerializeField] private LayoutGroup _slotsLayoutGroup = null;
        [SerializeField] private Transform _slotUiPrefab = null;

        private GenericUiList<ItemStack, UiContainerSlot> _slotsList;//data, widget
        private ContainerReference _containerReference;
        
        private ItemContainer _container;
        private ItemsDb _itemDb;

        private void Awake()
        {
            if (_slotsList == null)
            {
                _slotsList = new GenericUiList<ItemStack, UiContainerSlot>(_slotUiPrefab.gameObject, _slotsLayoutGroup, 300);
                for (int i = 0; i < _slotsList.GetEntries().Count; i++)
                {
                    _slotsList.GetComponentAt(i).SetId(i);
                }
            }
        }

        public void RegisterEvents()
        {
            _containerReference.GetContainer().OnContentChanged += UpdateUiSlots;
        }

        public void UnregisterEvents()
        {
            if (_containerReference != null)
            {
                _containerReference.GetContainer().OnContentChanged -= UpdateUiSlots;
            }
        }
        

        public void Init(ContainerReference reference)
        {
            UnregisterEvents();
            _containerReference = reference;
            var dbSystem = RpgStation.GetSystemStatic<DbSystem>();
            _itemDb = dbSystem.GetDb<ItemsDb>();
            if (_slotsList == null)
            {
                Awake();
            }
        }

        public void UpdateUiSlots()
        {
            _container = _containerReference.GetContainer();
            Dictionary<int, ItemStack>.ValueCollection list = _container?.GetState().Slots.Values;
            
            _slotsList.Generate(list, (entry, item) =>
            {
                item.Setup(this, _itemDb, _containerReference);
                item.SetData(entry);
            });
        }

        public UiDragSlotDummy GetDragDummy()
        {
            return _dragDummy;
        }
        
         

        public ContainerReference GetContainerReference()
        {
            return _containerReference;
        }
    }
}

