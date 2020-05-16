using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    
    public class UiEquipmentContainerWidget : MonoBehaviour
    {
#region FIELDS

        [SerializeField] private Transform[] _slotStaticPosition;

        [SerializeField] private UiDragSlotDummy _dragDummy = null;
        [SerializeField] private LayoutGroup _slotsLayoutGroup = null;
        [SerializeField] private Transform _slotUiPrefab = null;

        private GenericUiList<ItemStack, UiContainerSlot> _slotsList;//data, widget
        private ContainerReference _containerReference;
                
        private ItemContainer _container;
        private ItemsDb _itemDb;
        

#endregion
    }
}

