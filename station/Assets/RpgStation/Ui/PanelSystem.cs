using System;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class PanelSystem : BaseSystem
    {
        private UiElementBase _openedElement;
        private UiElementBase _defaultElement;
        private Dictionary<Type, UiPanel> _cachedPanels = new Dictionary<Type, UiPanel>();
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
            _openedElement = null;
            _defaultElement = null;
            _cachedPanels = new Dictionary<Type, UiPanel>();
        }

        private void Register(UiPanel element)
        {
            if (_cachedPanels.ContainsValue(element))
            {
                Debug.LogError("the panel was already registered: "+element);
            }
            else
            {
                _cachedPanels.Add(element.GetType(), element);
            }
        }
        
        private void UnRegister(UiPanel element)
        {
            if (_cachedPanels.ContainsValue(element) == false)
            {
                Debug.LogError("the panel was not registered: "+element);
            }
            else
            {
                _cachedPanels.Remove(element.GetType());
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
                if (panel == _openedElement)
                {
                    return;
                }

                if (_openedElement)
                {
                    _openedElement.Hide();
                }
                _openedElement = panel;
                panel.Show();
            }
        }

        private void Show()
        {
        }

        private void Hide(Type panel, bool showDefault)
        {
            if (_openedElement && _openedElement.GetType() == panel)
            {
                _openedElement.Hide();
            }

            if (showDefault && _defaultElement != null)
            {
                _defaultElement.Show();
                _openedElement = _defaultElement;
            }
            else
            {
                _openedElement = null;
            }

        }
        
        private void RegisterDefault(UiPanel element)
        {
            if (_defaultElement)
            {
                Debug.LogError("default panel duplicated:" + _defaultElement);
            }

            _defaultElement = element;
        }

        #region STATIC CALLS

        public static void RegisterPanel(UiPanel element)
        {
            _instance?.Register(element);
        }

        public static void RegisterDefaultPanel(UiPanel element)
        {
            _instance?.RegisterDefault(element);
        }

        public static void UnRegisterPanel(UiPanel element)
        {
            _instance?.UnRegister(element);
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

