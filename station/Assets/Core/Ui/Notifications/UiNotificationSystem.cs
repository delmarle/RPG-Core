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
        private Dictionary<ScriptableNotificationChannel, List<UiNotificationElement>> _channelMaps = new Dictionary<ScriptableNotificationChannel, List<UiNotificationElement>>();
        #endregion
       
        #region initialization & registration
        protected override void OnInit()
        {
            _instance = this;
        }

        protected override void OnDispose()
        {
            _instance = null;
        }

        protected override void OnDataBaseReady()
        {
            _channelsDb = GameInstance.GetDb<UiNotificationChannelsDb>();
            foreach (var dbEntry in _channelsDb.Db)
            {
                var channel = dbEntry.Value.Identifier;
                foreach (var element in dbEntry.Value.Elements)
                {
                    
                    if (element == null)
                    {
                        Debug.LogError($"one element in the channel ${channel} is missing");
                    }
                    else
                    {
                        var instance = Instantiate(element);
                        DontDestroyOnLoad(instance);
                        RegisterElement(channel, instance);
                    }
                }
            }
        }


        public void RegisterElement(ScriptableNotificationChannel channel, UiNotificationElement element)
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
        
        public void UnRegisterElement(ScriptableNotificationChannel channel, UiNotificationElement element)
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
                Debug.LogError($"the channel assigned to {element?.name} called {channel} does not exist");
            }
        }

        #endregion

        private void _ShowNotification(ScriptableNotificationChannel channel, Dictionary<string, object> data)
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
        
        public static void ShowNotification(ScriptableNotificationChannel channel, Dictionary<string, object> data)
        {
            if (_instance == null) return;
            
           _instance._ShowNotification(channel,data);
        }
        
        public static void ShowNotification(List<ScriptableNotificationChannel> channels, Dictionary<string, object> data)
        {
            if (_instance == null) return;

            foreach (var channel in channels)
            {
                if (channel != null)
                {
                    _instance._ShowNotification(channel,data);
                }
            }
            
        }
    }
    
    //db
    //channel
        //string channel id
        //UiNotificationElements[]
        [Serializable]
        public class UiChannelModel
        {
            public ScriptableNotificationChannel Identifier;
            public List<UiNotificationElement> Elements = new List<UiNotificationElement>();
        }
        
        [Serializable]
        public class NotificationConfigsModel
        {
            public List<ScriptableNotificationChannel> UseItemFail = new List<ScriptableNotificationChannel>();
        }
}


