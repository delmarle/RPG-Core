using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Station
{
    public class UiContainerSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region FIELDS
        private const string LOCKED = "locked";
        private const string UNLOCKED = "unlocked";
        private const string HAS_ITEM = "has_item";
        private const string EMPTY = "empty";
        [SerializeField] private TextMeshProUGUI _slotName = null;
        [SerializeField] private Image _bg = null;
        [SerializeField] private Image _icon = null;
        [SerializeField] private UiText _text = null;
        [SerializeField] private BaseAnimation _animation = null;

        private int _slotIndex;
        private BaseItemModel GetItemModel => _itemDb.GetEntry(_item?.ItemId);
        private bool HasItem => string.IsNullOrEmpty(_item?.ItemId) == false;
        private ItemStack _item;
        private ItemsDb _itemDb;
        private UiBaseContainer _uiContainer;
        
        //CONTAINER IDENTIFICATION
        private ContainerReference _containerReference;
        #endregion
        
        public void Setup(UiBaseContainer uiContainer, ItemsDb itemDb, ContainerReference containerReference)
        {
            _uiContainer = uiContainer;
            _itemDb = itemDb;
            _containerReference = containerReference;
            _animation.PlayState(UNLOCKED);
        }

        public void SetId(int slotIndex)
        {
            _slotIndex = slotIndex;
        }

        public void SetData(ItemStack data)
        {
           
            _item = data;
            if (HasItem)
            {
                gameObject.name = $"item [{_item.ItemId} slots id {_slotIndex}]";
                var itemEntry = GetItemModel;
                if (itemEntry != null)
                {
                    _icon.sprite = itemEntry.Icon;
                    _animation.PlayState(HAS_ITEM);
                    _text.text = _item.ItemCount.ToString();
                }
                else
                {
                    Debug.LogError($"this item does not exist");
                }
            }
            else
            {
                _icon.sprite = null;
                gameObject.name = $"item [empty]  slots id {_slotIndex}";
                _animation.PlayState(EMPTY);
                _text.text = string.Empty;
            }
        }

       #region DRAGGING
       public void OnBeginDrag(PointerEventData eventData)
       {
           if (HasItem == false) return;
           var dragDummy = _uiContainer.GetDragDummy();
           dragDummy.Initialize(_icon.sprite);
           dragDummy.SetPosition(transform.position);
           _animation.PlayState(LOCKED);
       }

       public void OnDrag(PointerEventData eventData)
       {
           if (HasItem == false) return;
           _uiContainer.GetDragDummy().SetPosition(eventData.position);
       }

       public void OnEndDrag(PointerEventData eventData)
       {
           if (HasItem == false) return;
           _animation.PlayState(UNLOCKED);
           if (eventData.pointerEnter)
           {
               var destinationSlot = eventData.pointerEnter.GetComponent<UiContainerSlot>();

               if (destinationSlot != null)
               {
                   var sourceContainer = _containerReference.GetContainer();
                   var destinationHandler = destinationSlot._containerReference;
                   var destinationContainer = destinationHandler.GetContainer();
                   if (sourceContainer == destinationHandler.GetContainer())
                   {
                       //has item already
                   }
                   else
                   {
                   }
                  
                   if (destinationContainer.CanAddItem(_item.ItemId))
                   {
                     //  destinationHandler.  
                     sourceContainer.TryMoveSlot(_slotIndex, destinationContainer, destinationSlot._slotIndex);
                   }
               }
           }
		
           _uiContainer.GetDragDummy().Reset();
         
       }

       public void SetBg(Sprite sprite)
       {
           if (_bg)
           {
               _bg.sprite = sprite;
           }
       }
       
       public void SetSlotName(string slotName)
       {
           if (_slotName)
           {
               _slotName.text = slotName;
           }
       }

       

       #endregion
    }
}

