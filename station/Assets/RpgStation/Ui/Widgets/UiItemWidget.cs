using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiItemWidget : MonoBehaviour
    {

        #region FIELDS

        [SerializeField] private RectTransform _root;
        [SerializeField] private TextMeshProUGUI _itemName = null;
        [SerializeField] private TextMeshProUGUI _itemDescription = null;
        [SerializeField] private Image _itemIcon = null;
        [SerializeField] private TextMeshProUGUI _itemRarity = null;
        [SerializeField] private TextMeshProUGUI _itemCategory = null;
        [SerializeField] private TextMeshProUGUI _itemCount = null;
        #endregion

        public void Setup(ItemStack stack)
        {
            if (_itemCount)
            {
                _itemCount.text = stack.ItemCount.ToString();
            }
            Setup(stack.ItemId);
        }
        
        public void Setup(string itemId)
        {
            var itemDb  = RpgStation.GetDb<ItemsDb>();

            var itemData = itemDb.GetEntry(itemId);
            
            
            if (_itemName)
            {
                _itemName.text = itemData.Name.GetValue();
            }
            
            if (_itemDescription)
            {
                _itemDescription.text = itemData.Description.GetValue();
            }
            
            if (_itemIcon)
            {
                _itemIcon.sprite = itemData.Icon;
            }

            if (_itemRarity)
            {
                var itemsRaritiesDb  = RpgStation.GetDb<ItemsRaritiesDb>();
                var rarityEntry = itemsRaritiesDb.GetEntry(itemData.RarityKey);
                _itemRarity.text = rarityEntry.Name.GetValue();
                _itemRarity.color = rarityEntry.Color;
            }

            if (_itemCategory)
            {
                var itemsCategoriesDb  = RpgStation.GetDb<ItemsCategoriesDb>();
                var categoryEntry = itemsCategoriesDb.GetEntry(itemData.CategoryKey);
                _itemCategory.text = categoryEntry.Name.GetValue();
            }
            gameObject.SetActive(true);
            _root.SetAsFirstSibling();
        }
        
        
    }
  
}
