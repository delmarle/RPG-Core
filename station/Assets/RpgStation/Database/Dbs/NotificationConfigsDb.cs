using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Station
{
    [CreateAssetMenu(menuName = StationConst.BUILD_ASSET_CREATE_PATH+"Db/Notification configs")]
    public class NotificationConfigsDb : SingleFieldDb<NotificationConfigsModel>
    {
        public override string ObjectName()
        {
            return "Notification Settings";
        }
    }
}

