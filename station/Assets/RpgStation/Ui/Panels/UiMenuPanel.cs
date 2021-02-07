using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class UiMenuPanel : UiPanel
    {
        #region FIELDS

        [SerializeField] private UiCharacterTab _charaterTab;
        [SerializeField] private UiTabsButtonWidgets _tabsButtonWidgets;
        #endregion

        public override void Show()
        {
            base.Show();
            _tabsButtonWidgets.OnOpen();
         
        }
    }
}
