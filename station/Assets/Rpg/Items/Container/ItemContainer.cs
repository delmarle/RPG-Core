using System;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public abstract class BaseItemContainer
    {
        #region CURRENCIES

        public CurrencyContainer CurrencyContainer;

        #endregion
        private const int INVALID_NUMBER = -1;
        public StationAction OnContentChanged;
        protected ContainerState _container;
        public ContainerState GetState() => _container;
        protected string _id;
        public string GetId() => _id;
        protected ItemsDb itemDb;
        
        public abstract bool ItemAllowed(int slot, BaseItemModel itemModel);
        public abstract bool CanAddItem(ItemStack stack);
        
        public void ClearSlot(int id)
        {
            _container.Slots[id].Reset();
            OnContentChanged?.Invoke();
        }
        
        public void AddItemCount(int id, int count)
        {
            _container.Slots[id].ItemCount +=count;
            OnContentChanged?.Invoke();
        }
       
        public void CopySlotToLocalSlot(int id, BaseItemContainer destinationContainer, int destinationSlotId, bool updateEvent = true)
        {
            var item = new ItemStack(_container.Slots[id]);
            destinationContainer._container.Slots[destinationSlotId].Set(item);
            _container.Slots[id].Reset();
            if (updateEvent)
            {
                OnContentChanged?.Invoke();
            }
        }

        public void SwapSlots(int id, BaseItemContainer to, int toId)
        {
            var fromItem = _container.Slots[id];
            var toItem = to._container.Slots[toId];
            fromItem.Swap(toItem);
            OnContentChanged?.Invoke();
        }
        
        protected int FindSlotWithItem(string itemName)
        {
            for (int i = 0; i < _container.Slots.Count; i++)
            {
                if(_container.Slots[i].ItemId == itemName)
                    return i;
            }
            
            return INVALID_NUMBER;
        }
        
        protected int FindFirstFreeSlot()
        {
            for (int i = 0; i < _container.Slots.Count; i++)
            {
                if(string.IsNullOrEmpty(_container.Slots[i].ItemId))
                    return i;
            }
            
            return INVALID_NUMBER;
        }
        
        public void AddItem(ItemStack entry)
        {
            var hasItem = FindSlotWithItem(entry.ItemId);
            if (hasItem >= 0)
            {
                AddItemCount(hasItem, entry.ItemCount);
            }
            else
            {
                int freeSlot = FindFirstFreeSlot();
                if (freeSlot >= 0)
                {
                    _container.Slots[freeSlot] = entry;
                }
            }
        }

        public int RemoveItem(string itemName, int amountToRemove)
        {
            for (int i = 0; i < _container.Slots.Count; i++)
            {
                if (_container.Slots[i].ItemId == itemName)
                {
                    var itemCount = _container.Slots[i].ItemCount;

                    if (amountToRemove > itemCount)
                    {
                        _container.Slots[i].Reset();
                   
                        return itemCount;
                    }
                    else
                    {
                        
                        _container.Slots[i].ItemCount -= amountToRemove;
                        if(_container.Slots[i].ItemCount == 0)
                            _container.Slots[i].Reset(); 
                        
                        return amountToRemove;
                    }
                }
            }

            return 0;
        }
        
        public Dictionary<int, string> GetItems()
        {
            var dict = new Dictionary<int, string>();
            for (int i = 0; i < _container.Slots.Count; i++)
            {
                dict.Add(i, _container.Slots[i].ItemId);
            }
            return dict;
        }
        public void TryMoveSlot(int fromId, BaseItemContainer to, int toId)
        {
            var itemToMove = _container.Slots[fromId];
            var destinationItem = to._container.Slots[toId];
            if (string.IsNullOrEmpty(itemToMove.ItemId)) return;

            var resolvedItemToMove = itemDb.GetEntry(itemToMove.ItemId);
           
            //Check: Contain item
            if (destinationItem != null && destinationItem.ItemCount > 0)
            {
                var resolvedDestinationItem = itemDb.GetEntry(destinationItem.ItemId);
                //Check: same slot
                if (this == to && fromId == toId)
                {
                    return;
                }

                //Check: Items can stack
                if (resolvedDestinationItem == resolvedItemToMove && resolvedItemToMove.Stackable)
                {
                    var count = _container.Slots[fromId].ItemCount;
                    ClearSlot(fromId);
                    to.AddItemCount(toId,count);
                }
                else
                {
                    //Check: Can item be swapped
                    if (to.ItemAllowed(toId, resolvedItemToMove)
                        && ItemAllowed(fromId, resolvedDestinationItem))
                    {
                        //can slot contain this item 
                        SwapSlots(fromId, to, toId);
                    }
                    else
                    {
                        //items cannot be swapped
                        return;
                    }
                }
            }
            else
            {
                //Check: Can item be moved
                if (to.ItemAllowed(toId, resolvedItemToMove))
                {
                    //TODO
                    //can slot contain this item 
                    
                    CopySlotToLocalSlot(fromId, to, toId);
                }
                else
                {
                    //items cannot be moved
                    return;
                }
            }

            //UPDATE THE UIs LISTENING
            OnContentChanged?.Invoke();
            if (to != this)
            {
                to.OnContentChanged?.Invoke();  
            }
            
        }

        public MoveItemToContainResult TryMoveSlotToContainer(int sourceSlotId, BaseItemContainer source)
        {
            if (source._container.Slots.ContainsKey(sourceSlotId))
            {
                var slotState = source._container.Slots[sourceSlotId];
                if (slotState.ItemCount > 0)
                {
                    int slotWithSameItem = FindSlotWithItem(slotState.ItemId);
                    int freeSlotId = FindFirstFreeSlot();
                    if (slotWithSameItem != INVALID_NUMBER)
                    {
                        //there is a slot with same item
                        int amount = source._container.Slots[sourceSlotId].ItemCount;
                        //TODO limit handling
                        source._container.Slots[sourceSlotId].Reset();
                        _container.Slots[slotWithSameItem].ItemCount += amount;
                    }
                    else
                    {
                        
                        if (freeSlotId != INVALID_NUMBER)
                        {
                            source.CopySlotToLocalSlot(sourceSlotId, this, freeSlotId, false);
                        }
                        else
                        {
                            //container is full
                            return MoveItemToContainResult.ContainerIsFull;
                        }
                    }
                }
                else
                {
                    //There is no item in this slot
                }
            }
            source.OnContentChanged?.Invoke();
            OnContentChanged?.Invoke();
            return MoveItemToContainResult.Complete;
        }

        public MoveItemToContainResult TryAddAllItemsFromContainer(BaseItemContainer source)
        {
            foreach (var sourceSlot in source.GetState().Slots)
            {
                var result = TryMoveSlotToContainer(sourceSlot.Key, source);
                if (result == MoveItemToContainResult.ContainerIsFull)
                {
                    source.OnContentChanged?.Invoke();
                    OnContentChanged?.Invoke();
                    return MoveItemToContainResult.ContainerIsFull;
                }
            }
            source.OnContentChanged?.Invoke();
            OnContentChanged?.Invoke();
            return MoveItemToContainResult.Complete;
        }
    }

    public enum MoveItemToContainResult
    {
        Complete,
        ContainerIsFull
    }
    public class ItemContainer: BaseItemContainer
    {
        
        public ItemContainer(string id, ContainerState state, ItemsDb itemsDb, Dictionary<string, long> currencies)
        {
            _id = id;
            _container = state;
            itemDb = itemsDb;
            CurrencyContainer = new CurrencyContainer(currencies);
        }
        

        public override bool ItemAllowed(int slot, BaseItemModel itemModel)
        {
            return true;
        }

        public override bool CanAddItem(ItemStack stack)
        {
            for (int i = 0; i < _container.Slots.Count; i++)
            {
                if (_container.Slots[i].ItemId == stack.ItemId)
                {
                    return true;
                }

                if (string.IsNullOrEmpty(_container.Slots[i].ItemId))
                {
                    return true;
                }
            }
            
            return false;
        }
    }

    [Serializable]
    public class ItemStack
    {
        public string ItemId;
        public int ItemCount;
        
        public ItemStack()
        {
            ItemId = String.Empty;
            ItemCount = 0;
        }
        public ItemStack(ItemStack item)
        {
            ItemId = item.ItemId;
            ItemCount = item.ItemCount;
        }
        
        public ItemStack(string itemId, int count)
        {
            ItemId = itemId;
            ItemCount = count;
        }
        public void Set(ItemStack item)
        {
            ItemId = item.ItemId;
            ItemCount = item.ItemCount;
        }

        public void Reset()
        {
            ItemId = string.Empty;
            ItemCount = 0;
        }

        public bool HasItem()
        {
            return string.IsNullOrEmpty(ItemId) == false;
            
        }

        public void Swap(ItemStack other)
        {
            string otherItemId = other.ItemId;
            int otherItemCount = other.ItemCount;

            other.ItemId = ItemId;
            other.ItemCount = ItemCount;
            ItemId = otherItemId;
            ItemCount = otherItemCount;
        }
    }
    
    [Serializable]
    public class ContainerState
    {
        public Dictionary<string, long> Currencies;
        public Dictionary<int, ItemStack> Slots;
        public ContainerState()
        {
        }

        public ContainerState(int size, List<ItemStack> defaultItems)
        {
            var itemDb = GameInstance.GetDb<ItemsDb>();
            var freeSlotRequired = CalculateFreeSlotRequired(defaultItems);
            if (freeSlotRequired > size)
            {
                Debug.LogWarning("inventory dont have enough slots");
            }

            Slots  = new Dictionary<int, ItemStack>();
            for (int i = 0; i < size; i++)
            {
                Slots.Add(i,new ItemStack());
            }

            var generatedList = GenerateSlotsFromData(defaultItems);
            for (int i = 0; i < generatedList.Count; i++)
            {
                if (i <= Slots.Count)
                {
                    var itemToAdd = generatedList[i];
                    Slots[i] = new ItemStack(itemToAdd);
                }
            }
        }

        private int CalculateFreeSlotRequired(List<ItemStack> defaultItems)
        {
            var itemDb = GameInstance.GetDb<ItemsDb>();
            int count = 0;

            foreach (var itemStack in defaultItems)
            {
               var itemData = itemDb.GetEntry(itemStack.ItemId);
               if (itemData.Stackable)
               {
                   count += 1;
               }
               else
               {
                   count += itemStack.ItemCount;
               }
            }
            return count;
        }

        private List<ItemStack> GenerateSlotsFromData(List<ItemStack> defaultItems)
        {
            var itemDb = GameInstance.GetDb<ItemsDb>();
            var list = new List<ItemStack>();
            foreach (var itemStack in defaultItems)
            {
                int amountToAdd = itemStack.ItemCount;
                var itemData = itemDb.GetEntry(itemStack.ItemId);
                if (itemData.Stackable)
                {
                    if (amountToAdd <= itemData.MaxStackSize)
                    {
                        list.Add(itemStack);
                    }
                    else
                    {
                        while (amountToAdd>0)
                        {
                            int amount = amountToAdd > itemData.MaxStackSize? itemData.MaxStackSize : amountToAdd;
                            var createdStack =  new ItemStack(itemStack.ItemId, amount);
                            list.Add(createdStack);
                            amountToAdd -= amount;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < amountToAdd; i++)
                    {
                        var createdStack =  new ItemStack(itemStack.ItemId, 1);
                        list.Add(createdStack);
                    }
                }
            }
            return list;
        }
        public ContainerState(Dictionary<int, ItemStack> save)
        {
            Slots = save;
        }
    }
}

