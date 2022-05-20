
using System.Collections.Generic;

namespace Station
{
    //some are already placed
    public class UiNotificationElement : UiElementBase
    {
        public List<ScriptableNotificationChannel> Channels = new List<ScriptableNotificationChannel>();
        private UiNotificationSystem _notificationSystem;
        
        protected override void Awake()
        {
            _notificationSystem = GameInstance.GetSystem<UiNotificationSystem>();
            base.Awake();
            foreach (var channel in Channels)
            {
                _notificationSystem.RegisterElement(channel, this);
            }
        }

        private void OnDestroy()
        {
            foreach (var channel in Channels)
            {
                _notificationSystem.UnRegisterElement(channel, this);
            }
        }

        public virtual void ReceiveNotification(Dictionary<string, object> data)
        {
            
        }
        
        
    }
}

