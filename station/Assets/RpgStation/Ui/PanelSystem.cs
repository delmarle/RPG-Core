using System;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class PanelSystem : BaseSystem
    {
        private UiPanelBase _openedPanel;
        private UiPanelBase _defaultPanel;
        private Dictionary<Type, UiPanelBase> _cachedPanels = new Dictionary<Type, UiPanelBase>();
        private static PanelSystem _instance;
        
        protected override void OnInit()
        {
            GameGlobalEvents.OnBeforeLeaveScene.AddListener(ClearCache);
            _instance = this;
        }

        protected override void OnDispose()
        {
            GameGlobalEvents.OnBeforeLeaveScene.RemoveListener(ClearCache);
            ClearCache();
            _instance = null;
        }
        
        
        private void ClearCache()
        {
            _openedPanel = null;
            _defaultPanel = null;
            _cachedPanels = new Dictionary<Type, UiPanelBase>();
        }

        private void Register(UiPanelBase panel)
        {
            if (_cachedPanels.ContainsValue(panel))
            {
                Debug.LogError("the panel was already registered: "+panel);
            }
            else
            {
                _cachedPanels.Add(panel.GetType(), panel);
            }
        }
        
        private void UnRegister(UiPanelBase panel)
        {
            if (_cachedPanels.ContainsValue(panel) == false)
            {
                Debug.LogError("the panel was not registered: "+panel);
            }
            else
            {
                _cachedPanels.Remove(panel.GetType());
            }
        }

        private void Show(Type panelType)
        {
            if (panelType == null)
            {
                return;
            }
            
            if (_cachedPanels.ContainsKey(panelType))
            {
                var panel = _cachedPanels[panelType];
                if (panel == _openedPanel)
                {
                    return;
                }

                if (_openedPanel)
                {
                    _openedPanel.Hide();
                }
                _openedPanel = panel;
                panel.Show();
            }
        }

        private void Hide(Type panel, bool showDefault)
        {
            if (_openedPanel && _openedPanel.GetType() == panel)
            {
                _openedPanel.Hide();
            }

            if (showDefault && _defaultPanel != null)
            {
                _defaultPanel.Show();
                _openedPanel = _defaultPanel;
            }
            else
            {
                _openedPanel = null;
            }

        }
        
        private void RegisterDefault(UiPanelBase panel)
        {
            if (_defaultPanel)
            {
                Debug.LogError("default panel duplicated:" + _defaultPanel);
            }

            _defaultPanel = panel;
        }

        #region STATIC CALLS

        public static void RegisterPanel(UiPanelBase panel)
        {
            _instance.Register(panel);
        }

        public static void RegisterDefaultPanel(UiPanelBase panel)
        {
            _instance.RegisterDefault(panel);
        }

        public static void UnRegisterPanel(UiPanelBase panel)
        {
            _instance.UnRegister(panel);
        }

        public static void OpenPanel<T>()
        {
            _instance.Show(typeof(T));
        }

        public static void HidePanel<T>(bool showDefault)
        {
            _instance.Hide(typeof(T), showDefault);
        }
        #endregion
    }
}

