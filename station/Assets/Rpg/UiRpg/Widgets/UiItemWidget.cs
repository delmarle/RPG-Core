using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Station
{
    public class UiItemWidget : PooledItem
    {

        #region FIELDS

      
        [Header("animation:")]
        [SerializeField] private Animation _animation;
        [SerializeField] private AnimationClip _showClip;
        [SerializeField] private AnimationClip _hideClip;
        [Header("fields:")]
        [SerializeField] private RectTransform _root;
        [SerializeField] private TextMeshProUGUI _itemName = null;
        [SerializeField] private TextMeshProUGUI _itemDescription = null;
        [SerializeField] private Image _itemIcon = null;
        [SerializeField] private TextMeshProUGUI _itemRarity = null;
        [SerializeField] private TextMeshProUGUI _itemCategory = null;
        [SerializeField] private TextMeshProUGUI _itemCount = null;

        #endregion

        public void Setup(string itemId, int itemCount)
        {
            if (_itemCount)
            {
                _itemCount.text = itemCount.ToString();
            }
            Setup(itemId);
        }
        
        public void Setup(string itemId)
        {
            var itemDb  = GameInstance.GetDb<ItemsDb>();

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
                var itemsRaritiesDb  = GameInstance.GetDb<ItemsRaritiesDb>();
                var rarityEntry = itemsRaritiesDb.GetEntry(itemData.RarityKey);
                _itemRarity.text = rarityEntry.Name.GetValue();
                _itemRarity.color = rarityEntry.Color;
            }

            if (_itemCategory)
            {
                var itemsCategoriesDb  = GameInstance.GetDb<ItemsCategoriesDb>();
                var categoryEntry = itemsCategoriesDb.GetEntry(itemData.CategoryKey);
                _itemCategory.text = categoryEntry.Name.GetValue();
            }

            DoShow();
        }

        public void DoShow()
        {
            gameObject.SetActive(true);
            _root.SetAsLastSibling();
            _animation.clip = _showClip;
            _animation.Play();
        }
        
        public void DoHide()
        {
            _animation.clip = _hideClip;
            _animation.Play();
        }
    }
  
}
