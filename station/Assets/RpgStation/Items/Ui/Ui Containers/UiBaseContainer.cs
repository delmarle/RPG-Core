using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class UiBaseContainer : MonoBehaviour
    {
        [SerializeField] protected UiDragSlotDummy _dragDummy = null;
        protected ContainerReference _containerReference;
        protected BaseItemContainer _container;
        protected ItemsDb _itemDb;
        private bool Initialized;
        
        public UiDragSlotDummy GetDragDummy()
        {
            return _dragDummy;
        }

        public ContainerReference GetContainerReference()
        {
            return _containerReference;
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
            if (Initialized == false)
            {
                CacheComponents();
                Initialized = true;
            }
        }

        protected virtual void CacheComponents()
        {
        }

        public virtual void UpdateUiSlots()
        {
        }
    }

}
