using System;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{

    public class UiNotificationSystem : BaseSystem
    {
        #region FIELDS

        private static UiNotificationSystem _instance;
        private UiNotificationChannelsDb _channelsDb;
        private Dictionary<string, List<UiNotificationElement>> _channelMaps = new Dictionary<string, List<UiNotificationElement>>();
        #endregion
       
        #region initialization & registration
        protected override void OnInit()
        {
            GameGlobalEvents.OnDataBaseLoaded.AddListener(OnDataBaseReady);
            _instance = this;
        }

    

        protected override void OnDispose()
        {
            GameGlobalEvents.OnDataBaseLoaded.RemoveListener(OnDataBaseReady);
            _instance = null;
        }

        private void OnDataBaseReady()
        {
            var dbSystem = RpgStation.GetSystemStatic<DbSystem>();
            _channelsDb = dbSystem.GetDb<UiNotificationChannelsDb>();
        }
        
        public void RegisterElement(string channel, UiNotificationElement element)
        {
            var channelModel = _channelsDb.GetChannelByName(channel);
            if (channelModel == null)
            {
                Debug.LogError($"the channel assigned to {element.name} called {channel} does not exist");
            }
            else
            {
                if (_channelMaps.ContainsKey(channel))
                {
                    _channelMaps[channel].Add(element);
                }
                else
                {
                    _channelMaps.Add(channel, new List<UiNotificationElement>{element});
                }
            }
        }
        
        public void UnRegisterElement(string channel, UiNotificationElement element)
        {
            if (_channelMaps.ContainsKey(channel))
            {
                if (_channelMaps[channel].Contains(element))
                {
                    _channelMaps[channel].Remove(element);
                }
                else
                {
                    Debug.LogError($"cant find this element in this channel");
                }

                _channelMaps[channel].Add(element);
            }
            else
            {
                Debug.LogError($"the channel assigned to {element.name} called {channel} does not exist");
            }
        }

        #endregion
        public void _ShowNotification(string channel, Dictionary<string, object> data)
        {
            if (_channelMaps.ContainsKey(channel))
            {
                var elements = _channelMaps[channel];
                foreach (var element in elements)
                {
                    element.ReceiveNotification(data);
                }
            }
        }
        
        public static void ShowNotification(string channel, Dictionary<string, object> data)
        {
           _instance._ShowNotification(channel,data);
        }
    }
    
    //db
    //channel
        //string channel id
        //UiNotificationElements[]
        [Serializable]
        public class UiChannelModel
        {
            public string Name;
            public List<UiNotificationElement> Elements = new List<UiNotificationElement>();
        }
}


