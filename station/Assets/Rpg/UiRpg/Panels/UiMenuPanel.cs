using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class UiMenuPanel : UiPanel
    {
        #region FIELDS

        [SerializeField] private UiCharacterTab _charaterTab;
        [SerializeField] private UiPlayerInventoryTab _inventoryTab;
        #endregion

        public override void Show()
        {
            Debug.Log($"show {name}");
            base.Show();
            _charaterTab.Show();
            _inventoryTab.Show();
        }

        public override void Hide()
        {
            base.Hide();
            _charaterTab.Hide();
            _inventoryTab.Hide();
        }
    }
}
