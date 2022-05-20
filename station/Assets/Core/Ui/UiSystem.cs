using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Station
{
    public class UiSystem : BaseSystem
    {
        #region FIELDS
        //PANELS
        private UiElementBase _openedElement;
        private UiElementBase _defaultElement;
        private Dictionary<Type, UiPanel> _cachedPanels = new Dictionary<Type, UiPanel>();
        
        //UNIQUE POPUPS
        private Dictionary<string, UiPopup> _uniquePopups = new  Dictionary<string, UiPopup>();
        
        private static UiSystem _instance;
        #endregion
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

        protected override void OnDataBaseReady()
        {
            CacheAllUniquePopups();
        }

        private void CacheAllUniquePopups()
        {
            var settingsDb = GameInstance.GetDb<GameSettingsDb>();
            foreach (var popup in settingsDb.Get()._cachedUniquePopup)
            {
                var instancePopup = (UiPopup)Instantiate(popup);
                DontDestroyOnLoad(instancePopup);
                _uniquePopups.Add(instancePopup.PopupUniqueId, instancePopup);
            }
        }
        
        private void ClearCache()
        {
            _openedElement = null;
            _defaultElement = null;
            _cachedPanels = new Dictionary<Type, UiPanel>();
        }

        //PANELS CAN ONLY SHOW ONE AT A TIME
        #region PANELS
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
            if (_cachedPanels.ContainsValue(element))
            {
                _cachedPanels.Remove(element.GetType());
            }
        }

        private T GetPanel<T>() where T : UiPanel
        {
            if (_cachedPanels.ContainsKey(typeof(T)))
            {
                return _cachedPanels[typeof(T)] as T;
            }

            return default;
        }

        private T ShowPanel<T>() where T : UiPanel
        {
            var panel = GetPanel<T>();
            if (panel != null)
            {
                if (panel == _openedElement)
                {
                    return _openedElement as T;
                }
                if (_openedElement)
                {
                    _openedElement.Hide();
                }
                panel.Show();
                return panel;
            }

            return default;
        }
        
        private  void ShowPanel(Type panelType) 
        {
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
                
                panel.Show();
                _openedElement = panel;
            }
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
            ShowPanel(_defaultElement.GetType());
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

        
        public static T GetCachedPanel<T>() where T : UiPanel
        {
            return _instance.GetPanel<T>();
        }
        public static T OpenPanel<T>() where T : UiPanel
        {
            return _instance.ShowPanel<T>();
        }

        public static void OpenPanel(Type typeName)
        {
            _instance.ShowPanel(typeName);
        }


        public static void HidePanel<T>(bool showDefault)
        {
            _instance.Hide(typeof(T), showDefault);
        }
        
        public static void HidePanel(Type typeName)
        {
            _instance.Hide(typeName, false);
        }
        #endregion
        
        #endregion
        
        //can show over others
        #region UNIQUE POPUPS

        private T _getUniquePopup<T>(string popupId) where T : UiPopup
        {
            if (_uniquePopups.ContainsKey(popupId))
            {
                return (T)_uniquePopups[popupId];
            }

            return null;
        }
        
        private void _hideUniquePopup<T>(string popupId) where T : UiPopup
        {
            if (_uniquePopups.ContainsKey(popupId))
            {
                var instancePopup = (T)_uniquePopups[popupId];
                instancePopup.Hide();
            }
        }
        
        //STATIC VERSIONS
        public static void HideUniquePopup<T>(string popupId)where T : UiPopup
        {
            _instance._hideUniquePopup<T>(popupId);
        }

        public static T GetUniquePopup<T>(string popupId) where T : UiPopup
        {
            return _instance._getUniquePopup<T>(popupId);
        }

        #endregion
    }
}

