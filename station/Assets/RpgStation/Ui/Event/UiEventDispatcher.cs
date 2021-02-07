using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    public class UiEventDispatcher : MonoBehaviour
    {
        [SerializeField] private UiEventData _eventData;

        public void SendEvent()
        {
            GameGlobalEvents.OnUiEvent.Invoke(_eventData);
        }
    }
}

