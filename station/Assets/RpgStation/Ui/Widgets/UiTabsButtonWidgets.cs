
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Station
{
    public class UiTabsButtonWidgets : MonoBehaviour
    {
        #region FIELDS
        [SerializeField] private UiTabData[] _tabsData;
        [SerializeField] private UnityEngine.UI.Button DefaultButton;
        private Dictionary<UnityEngine.UI.Button, string> _tabMap = new Dictionary<UnityEngine.UI.Button, string>();
        private UnityEngine.UI.Button _previousButton;
        #endregion

        private void Awake()
        {
            foreach (var tab in _tabsData)
            {
                _tabMap.Add(tab.button, tab.key);
                tab.button.onClick.AddListener(() => { OpenTab(tab.button); });
            }
        }

        private void OnDestroy()
        {
            foreach (var tab in _tabsData)
            {
                tab.button.onClick.RemoveListener(() => { OpenTab(tab.button); });
            }
        }

        public void OnOpen()
        {
            if (_previousButton == null && DefaultButton != null)
            {
                OpenTab(DefaultButton);
            }
        }
        public void OpenTab(UnityEngine.UI.Button btn)
        {
            string animationKey;
            if (_tabMap.ContainsKey(btn))
            {
                animationKey = _tabMap[btn];
                foreach (var tabData in _tabsData)
                {
                    bool isNext = tabData.button == btn;
                    tabData.button.interactable = !isNext;
                    if (tabData.uiElementTarget)
                    {
                        if (isNext)
                        {
                            if (tabData.uiElementTarget.IsVisible == false)
                            {
                                tabData.uiElementTarget.Show();
                            }
                        }
                        else
                        {
                            if (tabData.uiElementTarget.IsVisible)
                            {
                                tabData.uiElementTarget.Hide();
                            }
                        }
                    }
                  
                }
            }
            else
            {
                return;
            }

            _previousButton = btn;
        }
    }

    [Serializable]
    public class UiTabData
    {
        public string key;
        public UnityEngine.UI.Button button;
        public UiElementBase uiElementTarget;

    }
}

