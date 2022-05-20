using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class UiEventReceiver : MonoBehaviour
    { 
        public List<UiEventReceiverData> Events = new List<UiEventReceiverData>();
        private Dictionary<UiEventData, UiEventReceiverData> map = new Dictionary<UiEventData, UiEventReceiverData>();

        private void Awake()
        {
            foreach (var ev in Events)
            {
                map.Add(ev.EventData, ev);
            }
        }
        private void OnEnable()
        {
            GameGlobalEvents.OnUiEvent.AddListener(OnReceiveEvent);
        }

        private void OnDisable()
        {
            GameGlobalEvents.OnUiEvent.RemoveListener(OnReceiveEvent);
        }

        private void OnReceiveEvent(UiEventData data)
        {
            if (map.ContainsKey(data))
            {
                map[data].PlayEvent();
            }
        }
    }

    [Serializable]
    public class UiEventReceiverData
    {
        public UiEventData EventData;
        public List<UiPanel> _panelToShow = new List<UiPanel>();
        public List<UiPanel> _panelToHide = new List<UiPanel>();

        public void PlayEvent()
        {
            foreach (var panel in _panelToHide)
            {
                if (panel != null)
                {
                    UiSystem.HidePanel(panel.GetType());
                }
            }
            
            foreach (var panel in _panelToShow)
            {
                if (panel != null)
                {
                    UiSystem.OpenPanel(panel.GetType());
                }
            }
        }
    }
}

